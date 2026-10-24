using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Audio
{
    public sealed class Sound
    {
        private int _loops = 1;
        private float _elapsed;
        private float _baseVolume = 1f;
        private float _basePitch = 1f;
        private bool _finished;
        private object _target;
        private Func<float, float> _volume;
        private Func<float, float> _pitch;
        private Func<float, float> _spatialBlend;
        private Func<float, Vector3> _position;
        private SoundEntry _entry;
        private AudioSource _source;
        private Func<SoundChannelId, float> _channelVolume;
        private readonly List<Action> _onComplete = new List<Action>();
        private readonly List<Action> _onStop = new List<Action>();

        public SoundId Id => _entry.Id;
        public object Target => _target;
        public bool IsPlaying => !_finished;
        public AudioSource Source => _source;

        internal Sound(SoundEntry entry, AudioSource source, Func<SoundChannelId, float> channelVolume)
        {
            _entry = entry;
            _source = source;
            _channelVolume = channelVolume;
            _baseVolume = entry.Volume;
            _basePitch = entry.PickPitch();
            _spatialBlend = t => entry.SpatialBlend;

            _source.clip = entry.PickClip();
            _source.outputAudioMixerGroup = entry.MixerGroup;
            _source.loop = false;
            Apply();
            _source.Play();
        }

        public Sound SetLoop(int loops)
        {
            _loops = loops;
            return this;
        }

        public Sound SetLoop(bool loop)
        {
            _loops = loop ? -1 : 1;
            return this;
        }

        public Sound SetVolume(float volume)
        {
            _volume = t => volume;
            Apply();
            return this;
        }

        public Sound SetVolume(Func<float, float> volume)
        {
            _volume = volume;
            Apply();
            return this;
        }

        public Sound SetPitch(float pitch)
        {
            _pitch = t => pitch;
            Apply();
            return this;
        }

        public Sound SetPitch(Func<float, float> pitch)
        {
            _pitch = pitch;
            Apply();
            return this;
        }

        public Sound SetSpatialBlend(float spatialBlend)
        {
            _spatialBlend = t => spatialBlend;
            Apply();
            return this;
        }

        public Sound SetSpatialBlend(Func<float, float> spatialBlend)
        {
            _spatialBlend = spatialBlend;
            Apply();
            return this;
        }

        public Sound SetPosition(Vector3 position)
        {
            _position = t => position;
            Apply();
            return this;
        }

        public Sound SetPosition(Func<float, Vector3> position)
        {
            _position = position;
            Apply();
            return this;
        }

        public Sound SetTarget(object target)
        {
            _target = target;
            return this;
        }

        public Sound OnComplete(Action callback)
        {
            _onComplete.Add(callback);
            return this;
        }

        public Sound OnStop(Action callback)
        {
            _onStop.Add(callback);
            return this;
        }

        internal bool Tick(float deltaTime)
        {
            if (_finished)
            {
                return false;
            }

            if (_source == null || _source.clip == null)
            {
                Stop();
                return false;
            }

            Apply();
            if (_source.isPlaying || _source.time > 0f)
            {
                _elapsed += deltaTime;
            }

            if (!_source.isPlaying && AudioListener.pause == false)
            {
                CompleteLoop();
            }

            return !_finished;
        }

        internal void Stop()
        {
            if (_finished)
            {
                return;
            }

            _finished = true;
            if (_source != null)
            {
                _source.Stop();
            }

            foreach (Action callback in _onStop)
            {
                callback();
            }
        }

        private void CompleteLoop()
        {
            _elapsed = 0f;
            if (_loops > 0)
            {
                _loops--;
            }

            if (_loops == 0)
            {
                foreach (Action callback in _onComplete)
                {
                    callback();
                }

                Stop();
                return;
            }

            if (_entry.RandomizeOnLoop)
            {
                _source.clip = _entry.PickClip();
            }

            _source.Play();
        }

        private void Apply()
        {
            if (_source == null || _source.clip == null)
            {
                return;
            }

            float t = _source.clip.length > 0f ? Mathf.Clamp01(_source.time / _source.clip.length) : 0f;
            float volume = _volume != null ? _volume(t) : 1f;
            float pitch = _pitch != null ? _pitch(t) : 1f;

            _source.volume = volume * _baseVolume * _channelVolume(_entry.Channel);
            _source.pitch = pitch * _basePitch;
            _source.spatialBlend = _spatialBlend(t);
            if (_position != null)
            {
                _source.transform.position = _position(t);
            }
        }
    }
}
