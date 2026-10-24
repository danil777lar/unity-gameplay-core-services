using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Audio
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Larje/Core/Audio/Sound Config")]
    public sealed class SoundConfig : ScriptableObject
    {
        [Header("Channels")]
        [SerializeField] private SoundChannelId defaultChannel;
        [SerializeField] private List<SoundChannelId> channels = new List<SoundChannelId>();

        [Header("Pool")]
        [SerializeField] private int prewarm = 8;

        [Header("Sounds")]
        [SerializeField] private List<SoundEntry> sounds = new List<SoundEntry>();

        public SoundChannelId DefaultChannel => defaultChannel;
        public IReadOnlyList<SoundChannelId> Channels => channels;
        public int Prewarm => prewarm;
        public IReadOnlyList<SoundEntry> Sounds => sounds;

        public SoundEntry Find(SoundId id)
        {
            return sounds.Find(x => x.Id == id);
        }

        private void OnValidate()
        {
            sounds.ForEach(x => x.Validate());
        }
    }
}
