using System;
using System.Collections.Generic;

namespace Larje.Core.CompositeProperties
{
    [Serializable]
    public sealed class FloatProduct
    {
        private readonly List<Func<float>> _sources = new List<Func<float>>();

        public float Value
        {
            get
            {
                float product = 1f;
                foreach (Func<float> source in _sources)
                {
                    product *= source();
                }

                return product;
            }
        }

        public void AddValue(Func<float> source)
        {
            if (!_sources.Contains(source))
            {
                _sources.Add(source);
            }
        }

        public void RemoveValue(Func<float> source)
        {
            _sources.Remove(source);
        }
    }
}
