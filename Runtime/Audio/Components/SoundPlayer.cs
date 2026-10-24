using UnityEngine;

namespace Larje.Core.Audio
{
    public sealed class SoundPlayer : MonoBehaviour
    {
        [Header("Sound")]
        [SerializeField] private SoundId sound;
        [SerializeField] private bool playOnEnable;
        [SerializeField] private int loops = 1;

        [Header("Playback")]
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField, Range(0f, 2f)] private float pitch = 1f;
        [SerializeField, Range(0f, 1f)] private float spatialBlend;
        [SerializeField] private bool followTransform = true;

        private bool _started;
        private ISoundService _sounds;

#if LARJE_ZENJECT
        [Zenject.Inject]
#endif
        public void Construct(ISoundService sounds)
        {
            _sounds = sounds;
        }

        [ContextMenu("Play")]
        public void Play()
        {
            if (_sounds == null || sound == null)
            {
                return;
            }

            Sound played = _sounds.Play(sound);
            if (played == null)
            {
                return;
            }

            played.SetTarget(this)
                .SetLoop(loops)
                .SetVolume(volume)
                .SetPitch(pitch)
                .SetSpatialBlend(spatialBlend);

            if (followTransform)
            {
                played.SetPosition(t => transform.position);
            }
            else
            {
                played.SetPosition(transform.position);
            }
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            if (_sounds != null)
            {
                _sounds.StopAll(this);
            }
        }

        private void Start()
        {
            _started = true;
            if (playOnEnable)
            {
                Play();
            }
        }

        private void OnEnable()
        {
            if (_started && playOnEnable)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            Stop();
        }
    }
}
