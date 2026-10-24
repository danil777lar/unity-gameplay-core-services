using UnityEngine;

namespace Larje.Core.Extensions
{
    public static class FloatExtensions
    {
        public static bool SafeCompare(this float comparable, float other, float range)
        {
            return Mathf.Abs(comparable - other) <= range;
        }
    }
}
