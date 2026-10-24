using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Larje.Core.Audio
{
    [Serializable]
    public sealed class SoundEntry
    {
        [SerializeField, HideInInspector] private string inspectorName;

        [Header("Identity")]
        [SerializeField] private SoundId id;
        [SerializeField] private SoundChannelId channel;

        [Header("Clips")]
        [SerializeField] private AudioClip[] clips = new AudioClip[0];
        [SerializeField] private bool randomizeOnLoop = true;

        [Header("Playback")]
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField] private Vector2 pitch = Vector2.one;
        [SerializeField, Range(0f, 1f)] private float spatialBlend;
        [SerializeField] private AudioMixerGroup mixerGroup;

        public SoundId Id => id;
        public SoundChannelId Channel => channel;
        public bool RandomizeOnLoop => randomizeOnLoop;
        public float Volume => volume;
        public float SpatialBlend => spatialBlend;
        public AudioMixerGroup MixerGroup => mixerGroup;

        public AudioClip PickClip()
        {
            if (clips.Length == 0)
            {
                return null;
            }

            return clips[UnityEngine.Random.Range(0, clips.Length)];
        }

        public float PickPitch()
        {
            return UnityEngine.Random.Range(pitch.x, pitch.y);
        }

        public void Validate()
        {
            inspectorName = id != null ? id.name : "None";
        }
    }
}
