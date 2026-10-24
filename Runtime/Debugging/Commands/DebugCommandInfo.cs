using System.Collections.Generic;
using System.Reflection;

namespace Larje.Core.Debugging
{
    public sealed class DebugCommandInfo
    {
        public readonly string Name;
        public readonly string Group;
        public readonly string Description;
        public readonly IReadOnlyList<DebugParameterInfo> Parameters;

        internal readonly object Target;
        internal readonly MethodInfo Method;

        internal DebugCommandInfo(string name, string group, string description, IReadOnlyList<DebugParameterInfo> parameters, object target, MethodInfo method)
        {
            Name = name;
            Group = group;
            Description = description;
            Parameters = parameters;
            Target = target;
            Method = method;
        }
    }
}
