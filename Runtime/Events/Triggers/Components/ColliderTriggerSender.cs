using System.Collections.Generic;
using Larje.Core.Extensions;
using UnityEngine;

namespace Larje.Core.Events
{
    [RequireComponent(typeof(Collider))]
    public sealed class ColliderTriggerSender : TriggerSender
    {
        [Header("Filter")]
        [SerializeField] private bool useLayerMask;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private bool useTag;
        [SerializeField] private string requiredTag = "Player";

        private readonly HashSet<Collider> _inside = new HashSet<Collider>();

        private void OnTriggerEnter(Collider other)
        {
            if (Accepts(other) && _inside.Add(other))
            {
                Value = 1f;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_inside.Remove(other))
            {
                Value = _inside.Count > 0 ? 1f : 0f;
            }
        }

        private void FixedUpdate()
        {
            if (_inside.RemoveWhere(x => x == null || !x.enabled || !x.gameObject.activeInHierarchy) > 0)
            {
                Value = _inside.Count > 0 ? 1f : 0f;
            }
        }

        private bool Accepts(Collider other)
        {
            if (useLayerMask && !layerMask.HasLayer(other.gameObject.layer))
            {
                return false;
            }

            return !useTag || other.CompareTag(requiredTag);
        }
    }
}
