using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tools
{
    public sealed class PrefabPacker : MonoBehaviour
    {
        [SerializeField] private PrefabPackerConfig config;
        [SerializeField] private List<PackedObject> packed = new List<PackedObject>();

        public bool IsPacked => packed.Count > 0;

        public event Action Unpacked;

        [ContextMenu("Pack")]
        public int Pack()
        {
            if (IsPacked)
            {
                return 0;
            }

            foreach (PackablePrefab prefab in GetComponentsInChildren<PackablePrefab>(true))
            {
                packed.Add(new PackedObject { Name = prefab.name, Id = prefab.Id, Data = prefab.Pack() });
                DestroyImmediate(prefab.gameObject);
            }

            return packed.Count;
        }

        [ContextMenu("Unpack")]
        public int Unpack()
        {
            int count = 0;
            foreach (PackedObject entry in packed)
            {
                PackablePrefab prefab = config.Find(entry.Id);
                if (prefab != null)
                {
                    PackablePrefab instance = Spawn(prefab);
                    instance.name = entry.Name;
                    instance.Unpack(entry.Data);
                    count++;
                }
            }

            packed.Clear();
            return count;
        }

        private void Start()
        {
            if (IsPacked)
            {
                Unpack();
                Unpacked?.Invoke();
            }
        }

        private PackablePrefab Spawn(PackablePrefab prefab)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return (PackablePrefab)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
            }
#endif
            return Instantiate(prefab);
        }

        [Serializable]
        private sealed class PackedObject
        {
            [HideInInspector] public string Name;
            public PackablePrefabId Id;
            public PackablePrefab.PackedData Data;
        }
    }
}
