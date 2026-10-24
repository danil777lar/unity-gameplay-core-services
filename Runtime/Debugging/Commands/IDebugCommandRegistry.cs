using System.Collections.Generic;

namespace Larje.Core.Debugging
{
    public interface IDebugCommandRegistry
    {
        IReadOnlyList<DebugCommandInfo> Commands { get; }

        void Register(object provider);
        void Unregister(object provider);
        DebugCommandInfo Find(string name);
        DebugCommandResult Execute(string name, IReadOnlyDictionary<string, string> arguments);
        DebugCommandResult Execute(DebugCommandInfo command, object[] arguments);
    }
}
