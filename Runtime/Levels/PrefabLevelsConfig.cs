using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Levels
{
    [CreateAssetMenu(fileName = "PrefabLevelsConfig", menuName = "Larje/Core/Levels/Prefab Levels Config")]
    public sealed class PrefabLevelsConfig : ScriptableObject, IConfigValidatable
    {
        [SerializeField] private List<Entry> levels = new List<Entry>();

        public IReadOnlyList<Entry> Levels => levels;

        [Serializable]
        public sealed class Entry
        {
            public GameObject Prefab;
            public bool AddToRandom = true;
        }

        public void Validate(List<string> problems)
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].Prefab == null)
                {
                    problems.Add($"Level {i} has no prefab");
                }
                else if (levels[i].Prefab.GetComponent<LevelProcessor>() == null)
                {
                    problems.Add($"Level prefab '{levels[i].Prefab.name}' has no LevelProcessor");
                }
            }
        }
    }
}
