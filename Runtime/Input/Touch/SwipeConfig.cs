using UnityEngine;

namespace Larje.Core.Input
{
    [CreateAssetMenu(fileName = "SwipeConfig", menuName = "Larje/Core/Input/Swipe Config")]
    public sealed class SwipeConfig : ScriptableObject
    {
        [SerializeField] private bool enabled = true;
        [SerializeField, Range(0f, 1f)] private float minLength = 0.1f;
        [SerializeField, Range(0f, 1f)] private float maxLength = 0.4f;
        [SerializeField, Range(0f, 180f)] private float maxAngle = 45f;

        public bool Enabled => enabled;
        public float MinLength => minLength;
        public float MaxLength => maxLength;
        public float MaxAngle => maxAngle;
    }
}
