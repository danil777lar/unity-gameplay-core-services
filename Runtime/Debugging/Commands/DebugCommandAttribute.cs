using System;

namespace Larje.Core.Debugging
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class DebugCommandAttribute : Attribute
    {
        public readonly string Group;
        public readonly string Description;
        public readonly string Name;

        public DebugCommandAttribute(string group, string description = "", string name = null)
        {
            Group = group;
            Description = description;
            Name = name;
        }
    }
}
