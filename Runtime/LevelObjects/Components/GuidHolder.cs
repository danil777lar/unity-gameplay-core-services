using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.LevelObjects
{
    [DisallowMultipleComponent]
    public sealed class GuidHolder : MonoBehaviour
    {
        private static readonly List<GuidHolder> s_holders = new List<GuidHolder>();

        [SerializeField] private string guid;

        public string Guid => guid;

        public static bool TryFind<T>(string guid, out T component) where T : Component
        {
            GuidHolder holder = s_holders.Find(x => x.guid == guid);
            component = holder != null ? holder.GetComponentInChildren<T>() : null;
            return component != null;
        }

        [ContextMenu("Generate Guid")]
        public void Generate()
        {
            guid = System.Guid.NewGuid().ToString("N");
        }

        public void SetGuid(string target)
        {
            guid = target;
        }

        private void Awake()
        {
            s_holders.Add(this);
        }

        private void OnDestroy()
        {
            s_holders.Remove(this);
        }

        private void Reset()
        {
            Generate();
        }
    }
}
