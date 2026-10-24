using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools
{
    [CreateAssetMenu(fileName = "PrefabPackerConfig", menuName = "Larje/Core/Tools/Prefab Packer Config")]
    public sealed class PrefabPackerConfig : ScriptableObject
    {
        [SerializeField] private List<PackablePrefab> prefabs = new List<PackablePrefab>();

        public PackablePrefab Find(PackablePrefabId id)
        {
            return prefabs.Find(x => x != null && x.Id == id);
        }
    }
}
