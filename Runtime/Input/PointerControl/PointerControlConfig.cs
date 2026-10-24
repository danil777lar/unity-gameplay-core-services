using UnityEngine;

namespace Larje.Core.Input
{
    [CreateAssetMenu(fileName = "PointerControlConfig", menuName = "Larje/Core/Input/Pointer Control Config")]
    public sealed class PointerControlConfig : ScriptableObject
    {
        [SerializeField, Min(1)] private int dragSmoothing = 3;
        [SerializeField] private float holdTime = 1f;
        [SerializeField] private float dragSensitivity = 0.01f;
        [SerializeField] private float raycastDistance = 1000f;
        [SerializeField] private LayerMask mask = ~0;

        public int DragSmoothing => dragSmoothing;
        public float HoldTime => holdTime;
        public float DragSensitivity => dragSensitivity;
        public float RaycastDistance => raycastDistance;
        public LayerMask Mask => mask;
    }
}
