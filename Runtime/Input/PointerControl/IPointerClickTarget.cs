using UnityEngine;

namespace Larje.Core.Input
{
    public interface IPointerClickTarget
    {
        PointerCommand OnPointerClick(RaycastHit hit);
    }
}
