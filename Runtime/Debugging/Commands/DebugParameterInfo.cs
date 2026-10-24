using System;
using System.Collections.Generic;

namespace Larje.Core.Debugging
{
    public sealed class DebugParameterInfo
    {
        public readonly string Name;
        public readonly Type Type;
        public readonly bool HasDefault;
        public readonly object DefaultValue;

        public string TypeName => Type.Name;

        public DebugParameterInfo(string name, Type type, bool hasDefault, object defaultValue)
        {
            Name = name;
            Type = type;
            HasDefault = hasDefault;
            DefaultValue = defaultValue;
        }

        public IReadOnlyList<string> GetOptions()
        {
            return DebugValueParser.GetOptions(Type);
        }
    }
}
