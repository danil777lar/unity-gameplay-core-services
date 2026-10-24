using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Audio
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Larje/Core/Audio/Sound Config")]
    public sealed class SoundConfig : ScriptableObject, IConfigValidatable
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

        public void Validate(List<string> problems)
        {
            HashSet<SoundId> ids = new HashSet<SoundId>();
            foreach (SoundEntry entry in sounds)
            {
                if (entry.Id == null)
                {
                    problems.Add("Sound entry has no SoundId");
                }
                else if (!ids.Add(entry.Id))
                {
                    problems.Add($"Sound '{entry.Id.name}' is configured several times");
                }

                if (entry.PickClip() == null)
                {
                    problems.Add($"Sound '{(entry.Id != null ? entry.Id.name : "None")}' has no clips");
                }
            }
        }
    }
}
