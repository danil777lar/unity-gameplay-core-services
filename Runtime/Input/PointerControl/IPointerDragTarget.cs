using UnityEngine;

namespace Larje.Core.Input
{
    public interface IPointerDragTarget
    {
        bool IsDraggable { get; }
        Transform Transform { get; }
    }
}
