using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.CompositeProperties
{
    [Serializable]
    public sealed class BoolComposite
    {
        [SerializeField] private bool defaultValue = true;
        [SerializeField] private Operation operation = Operation.And;
        [SerializeField] private bool lastValue;

        private readonly List<Func<bool>> _sources = new List<Func<bool>>();

        public bool Value
        {
            get
            {
                bool result = defaultValue;
                foreach (Func<bool> source in _sources)
                {
                    if (operation == Operation.And)
                    {
                        result &= source();
                    }
                    else
                    {
                        result |= source();
                    }
                }

                lastValue = result;
                return result;
            }
        }

        public BoolComposite()
        {
        }

        public BoolComposite(bool defaultValue, Operation operation)
        {
            this.defaultValue = defaultValue;
            this.operation = operation;
        }

        public void AddValue(Func<bool> source)
        {
            if (!_sources.Contains(source))
            {
                _sources.Add(source);
            }
        }

        public void RemoveValue(Func<bool> source)
        {
            _sources.Remove(source);
        }

        public enum Operation
        {
            And,
            Or
        }
    }
}
