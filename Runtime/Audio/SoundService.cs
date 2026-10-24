using System;
using System.Collections.Generic;
using System.Threading;
using Larje.Core.Saves;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Larje.Core.Audio
{
    public sealed class SoundService : ISoundService, IInitializableService, IDisposable
    {
        private const string ROOT_NAME = "[Larje Sounds]";

        private Transform _root;
        private readonly SoundConfig _config;
        private readonly ISaveService _saves;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();
        private readonly List<Sound> _playing = new List<Sound>();
        private readonly Stack<AudioSource> _pool = new Stack<AudioSource>();

        public IReadOnlyList<Sound> Playing => _playing;
        public SoundConfig Config => _config;

        public bool Enabled
        {
            get => Settings.Enabled;
            set
            {
                Settings.Enabled = value;
                AudioListener.volume = value ? 1f : 0f;
                _saves.SaveSystem();
            }
        }

        private AudioSettingsData Settings => _saves.Get<AudioSettingsData>();

        public SoundService(SoundConfig config, ISaveService saves)
        {
            _config = config;
            _saves = saves;
        }

        public void Initialize()
        {
            GameObject root = new GameObject(ROOT_NAME);
            Object.DontDestroyOnLoad(root);
            _root = root.transform;

            for (int i = 0; i < _config.Prewarm; i++)
            {
                Release(CreateSource());
            }

            AudioListener.volume = Settings.Enabled ? 1f : 0f;
            ServiceLoop.Run(Tick, _lifetime.Token);
        }

        public void Dispose()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
            StopAll();
            if (_root != null)
            {
                Object.Destroy(_root.gameObject);
            }
        }

        public Sound Play(SoundId id)
        {
            SoundEntry entry = _config.Find(id);
            if (entry == null)
            {
                Debug.LogWarning($"SoundService: sound '{id}' not configured");
                return null;
            }

            AudioSource source = Acquire();
            Sound sound = new Sound(entry, source, GetChannelVolume);
            sound.OnStop(() => OnSoundStopped(sound));
            _playing.Add(sound);
            return sound;
        }

        public void Stop(Sound sound)
        {
            if (sound != null)
            {
                sound.Stop();
            }
        }

        public void StopAll(object target)
        {
            foreach (Sound sound in _playing.ToArray())
            {
                if (sound.Target == target)
                {
                    sound.Stop();
                }
            }
        }

        public void StopAll()
        {
            foreach (Sound sound in _playing.ToArray())
            {
                sound.Stop();
            }
        }

        public float GetChannelVolume(SoundChannelId channel)
        {
            SoundChannelId resolved = channel != null ? channel : _config.DefaultChannel;
            if (resolved == null)
            {
                return 1f;
            }

            return Settings.ChannelVolumes.TryGetValue(resolved.Key, out float volume) ? volume : 1f;
        }

        public void SetChannelVolume(SoundChannelId channel, float volume)
        {
            if (channel == null)
            {
                return;
            }

            Settings.ChannelVolumes[channel.Key] = Mathf.Clamp01(volume);
            _saves.SaveSystem();
        }

        private void Tick(float deltaTime)
        {
            for (int i = _playing.Count - 1; i >= 0; i--)
            {
                if (i < _playing.Count)
                {
                    _playing[i].Tick(deltaTime);
                }
            }
        }

        private void OnSoundStopped(Sound sound)
        {
            _playing.Remove(sound);
            if (sound.Source != null)
            {
                Release(sound.Source);
            }
        }

        private AudioSource Acquire()
        {
            AudioSource source = _pool.Count > 0 ? _pool.Pop() : CreateSource();
            source.gameObject.SetActive(true);
            return source;
        }

        private void Release(AudioSource source)
        {
            if (_root == null)
            {
                return;
            }

            source.Stop();
            source.clip = null;
            source.transform.SetParent(_root, false);
            source.gameObject.SetActive(false);
            _pool.Push(source);
        }

        private AudioSource CreateSource()
        {
            GameObject sourceObject = new GameObject("Sound");
            sourceObject.transform.SetParent(_root, false);
            AudioSource source = sourceObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }
    }
}
