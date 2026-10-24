using UnityEngine;

namespace Larje.Core.Extensions
{
    public static class StringExtensions
    {
        public static string LerpLinear(this string from, string to, float t)
        {
            int length = Mathf.Max(from.Length, to.Length);
            int current = Mathf.FloorToInt(Mathf.Lerp(0, length, t));

            char[] output = new char[length];
            for (int i = 0; i < length; i++)
            {
                if (i <= current)
                {
                    output[i] = i < to.Length ? to[i] : ' ';
                }
                else
                {
                    output[i] = i < from.Length ? from[i] : ' ';
                }
            }

            return new string(output);
        }
    }
}
