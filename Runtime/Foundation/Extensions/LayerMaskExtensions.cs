using UnityEngine;

namespace Larje.Core.Extensions
{
    public static class LayerMaskExtensions
    {
        public static bool HasLayer(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }
    }
}
