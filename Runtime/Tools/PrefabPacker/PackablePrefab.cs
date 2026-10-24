using System;
using UnityEngine;

namespace Larje.Core.Tools
{
    public sealed class PackablePrefab : MonoBehaviour
    {
        [SerializeField] private PackablePrefabId id;

        public PackablePrefabId Id => id;

        public PackedData Pack()
        {
            return new PackedData
            {
                Active = gameObject.activeSelf,
                Parent = transform.parent,
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = transform.localScale
            };
        }

        public void Unpack(PackedData data)
        {
            transform.SetParent(data.Parent);
            transform.SetPositionAndRotation(data.Position, data.Rotation);
            transform.localScale = data.Scale;
            gameObject.SetActive(data.Active);
        }

        [Serializable]
        public sealed class PackedData
        {
            public bool Active;
            public Transform Parent;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;
        }
    }
}
