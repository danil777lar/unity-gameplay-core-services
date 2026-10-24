using System;
using UnityEngine;

namespace Larje.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ConstDropdownAttribute : PropertyAttribute
    {
        public readonly Type Source;

        public ConstDropdownAttribute(Type source)
        {
            Source = source;
        }
    }
}
