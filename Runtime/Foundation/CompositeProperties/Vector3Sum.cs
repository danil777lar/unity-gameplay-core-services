using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.CompositeProperties
{
    [Serializable]
    public sealed class Vector3Sum
    {
        private readonly List<Func<Vector3>> _sources = new List<Func<Vector3>>();

        public Vector3 Value
        {
            get
            {
                Vector3 sum = Vector3.zero;
                foreach (Func<Vector3> source in _sources)
                {
                    sum += source();
                }

                return sum;
            }
        }

        public void AddValue(Func<Vector3> source)
        {
            if (!_sources.Contains(source))
            {
                _sources.Add(source);
            }
        }

        public void RemoveValue(Func<Vector3> source)
        {
            _sources.Remove(source);
        }
    }
}
