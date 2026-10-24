using System.Collections.Generic;

namespace Larje.Core.Debugging
{
    public static class DebugOverlayLog
    {
        private const int MAX_PER_GROUP = 20;

        private static readonly Dictionary<string, List<string>> s_groups = new Dictionary<string, List<string>>();
        private static readonly HashSet<string> s_hidden = new HashSet<string>();

        public static IEnumerable<string> Groups => s_groups.Keys;

        public static void Write(string group, string message)
        {
            if (!s_groups.TryGetValue(group, out List<string> messages))
            {
                messages = new List<string>();
                s_groups[group] = messages;
            }

            messages.Add(message);
            if (messages.Count > MAX_PER_GROUP)
            {
                messages.RemoveAt(0);
            }
        }

        public static bool IsVisible(string group)
        {
            return !s_hidden.Contains(group);
        }

        public static void SetVisible(string group, bool visible)
        {
            if (visible)
            {
                s_hidden.Remove(group);
            }
            else
            {
                s_hidden.Add(group);
            }
        }

        public static IReadOnlyList<string> Consume(string group)
        {
            if (!s_groups.TryGetValue(group, out List<string> messages))
            {
                return new string[0];
            }

            string[] copy = messages.ToArray();
            messages.Clear();
            return copy;
        }
    }
}
