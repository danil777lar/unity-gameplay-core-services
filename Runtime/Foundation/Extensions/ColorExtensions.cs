using UnityEngine;

namespace Larje.Core.Extensions
{
    public static class ColorExtensions
    {
        public static Color SetAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static Color SetSaturation(this Color color, float saturation)
        {
            float gray = (color.r + color.g + color.b) / 3f;
            Color grayColor = new Color(gray, gray, gray, color.a);
            return Color.LerpUnclamped(grayColor, color, saturation);
        }
    }
}
