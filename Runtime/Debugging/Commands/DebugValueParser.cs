using System;
using System.Collections.Generic;
using System.Globalization;
using Larje.Core.Extensions;
using Larje.Core.Ids;
using UnityEngine;

namespace Larje.Core.Debugging
{
    public static class DebugValueParser
    {
        private static readonly IReadOnlyList<string> s_noOptions = new string[0];

        public static bool TryParse(string text, Type type, out object result)
        {
            result = null;
            try
            {
                if (type == typeof(string))
                {
                    result = text;
                    return true;
                }

                if (type == typeof(bool))
                {
                    result = text == "1" || string.Equals(text, "true", StringComparison.OrdinalIgnoreCase);
                    return true;
                }

                if (type.IsEnum)
                {
                    result = Enum.Parse(type, text, true);
                    return true;
                }

                if (type == typeof(Vector3))
                {
                    result = Vector3Extensions.Parse(text);
                    return true;
                }

                if (type == typeof(Vector2))
                {
                    Vector3 parsed = Vector3Extensions.Parse(text + ",0");
                    result = new Vector2(parsed.x, parsed.y);
                    return true;
                }

                if (typeof(LarjeId).IsAssignableFrom(type))
                {
                    result = FindId(type, text);
                    return result != null || string.IsNullOrEmpty(text);
                }

                if (type.IsPrimitive || type == typeof(decimal))
                {
                    result = Convert.ChangeType(text, type, CultureInfo.InvariantCulture);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public static IReadOnlyList<string> GetOptions(Type type)
        {
            if (type.IsEnum)
            {
                return Enum.GetNames(type);
            }

            if (type == typeof(bool))
            {
                return new[] { "false", "true" };
            }

            if (typeof(LarjeId).IsAssignableFrom(type))
            {
                List<string> keys = new List<string>();
                foreach (UnityEngine.Object found in Resources.FindObjectsOfTypeAll(type))
                {
                    keys.Add(((LarjeId)found).Key);
                }

                keys.Sort(StringComparer.Ordinal);
                return keys;
            }

            return s_noOptions;
        }

        public static LarjeId FindId(Type type, string key)
        {
            foreach (UnityEngine.Object found in Resources.FindObjectsOfTypeAll(type))
            {
                LarjeId id = (LarjeId)found;
                if (id.Key == key || id.name == key)
                {
                    return id;
                }
            }

            return null;
        }
    }
}
