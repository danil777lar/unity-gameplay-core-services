using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Monetization
{
    [CreateAssetMenu(fileName = "DiscountQueueConfig", menuName = "Larje/Core/Monetization/Discount Queue Config")]
    public sealed class DiscountQueueConfig : ScriptableObject
    {
        [SerializeField] private float checkInterval = 5f;
        [SerializeField] private float discountDurationMinutes = 60f;
        [SerializeField] private List<ProductConfig> queue = new List<ProductConfig>();

        public float CheckInterval => checkInterval;
        public float DiscountDurationMinutes => discountDurationMinutes;
        public IReadOnlyList<ProductConfig> Queue => queue;
    }
}
