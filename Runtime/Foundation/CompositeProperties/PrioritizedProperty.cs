using System;
using System.Collections.Generic;

namespace Larje.Core.CompositeProperties
{
    [Serializable]
    public sealed class PrioritizedProperty<T>
    {
        private readonly Dictionary<Func<T>, Func<int>> _sources = new Dictionary<Func<T>, Func<int>>();

        public bool TryGetValue(out T result)
        {
            Func<T> best = null;
            int bestPriority = int.MinValue;
            foreach (KeyValuePair<Func<T>, Func<int>> pair in _sources)
            {
                int priority = pair.Value();
                if (best == null || priority > bestPriority)
                {
                    best = pair.Key;
                    bestPriority = priority;
                }
            }

            if (best == null)
            {
                result = default;
                return false;
            }

            result = best();
            return true;
        }

        public void AddValue(Func<T> source, Func<int> priority)
        {
            if (!_sources.ContainsKey(source))
            {
                _sources.Add(source, priority);
            }
        }

        public void RemoveValue(Func<T> source)
        {
            _sources.Remove(source);
        }
    }
}
