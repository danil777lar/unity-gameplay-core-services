using UnityEngine;

namespace Larje.Core.Audio
{
    public sealed class SystemVolumeService : ISystemVolumeService
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private const int FLAG_SHOW_UI = 1;

        private int _stream;
        private AndroidJavaObject _audioManager;
#endif

        public bool Supported
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }

        public float GetVolume()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject manager = GetAudioManager();
            int max = manager.Call<int>("getStreamMaxVolume", _stream);
            return max > 0 ? manager.Call<int>("getStreamVolume", _stream) / (float)max : 0f;
#else
            return 1f;
#endif
        }

        public void SetVolume(float volume)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaObject manager = GetAudioManager();
            int max = manager.Call<int>("getStreamMaxVolume", _stream);
            manager.Call("setStreamVolume", _stream, Mathf.RoundToInt(Mathf.Clamp01(volume) * max), FLAG_SHOW_UI);
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private AndroidJavaObject GetAudioManager()
        {
            if (_audioManager != null)
            {
                return _audioManager;
            }

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaClass managerClass = new AndroidJavaClass("android.media.AudioManager"))
            using (AndroidJavaClass contextClass = new AndroidJavaClass("android.content.Context"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
                _stream = managerClass.GetStatic<int>("STREAM_MUSIC");
                _audioManager = context.Call<AndroidJavaObject>("getSystemService", contextClass.GetStatic<string>("AUDIO_SERVICE"));
            }

            return _audioManager;
        }
#endif
    }
}
