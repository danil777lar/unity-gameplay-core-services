using System;

namespace Larje.Core.Saves
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class SaveSectionAttribute : Attribute
    {
        public readonly string Key;
        public readonly SaveScope Scope;

        public SaveSectionAttribute(string key, SaveScope scope = SaveScope.Game)
        {
            Key = key;
            Scope = scope;
        }
    }
}
