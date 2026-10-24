using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Larje.Core.Levels
{
    [CreateAssetMenu(fileName = "AddressableLevelsConfig", menuName = "Larje/Core/Levels/Addressable Levels Config")]
    public sealed class AddressableLevelsConfig : ScriptableObject
    {
        [SerializeField] private List<Entry> levels = new List<Entry>();

        public IReadOnlyList<Entry> Levels => levels;

        [Serializable]
        public sealed class Entry
        {
            public AssetReferenceGameObject Prefab;
            public bool AddToRandom = true;
        }
    }
}
