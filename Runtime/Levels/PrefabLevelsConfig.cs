using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Levels
{
    [CreateAssetMenu(fileName = "PrefabLevelsConfig", menuName = "Larje/Core/Levels/Prefab Levels Config")]
    public sealed class PrefabLevelsConfig : ScriptableObject
    {
        [SerializeField] private List<Entry> levels = new List<Entry>();

        public IReadOnlyList<Entry> Levels => levels;

        [Serializable]
        public sealed class Entry
        {
            public GameObject Prefab;
            public bool AddToRandom = true;
        }
    }
}
