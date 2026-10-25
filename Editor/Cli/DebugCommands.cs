using System;
using System.Collections.Generic;
using Larje.Core.Debugging;
using Unity.Pipeline.Commands;
using UnityEditor;

namespace Larje.Core.Editor.Cli
{
    public static class DebugCommands
    {
        private const string GROUP = "larje.debug";
        private const string LIST_DESCRIPTION = "Lists Larje debug commands available in Play Mode (saves, currency, UI, levels, ads, audio, events, game state and project-specific commands). Returns name, group, description and typed parameters with allowed options. Requires Play Mode.";
        private const string RUN_DESCRIPTION = "Runs a Larje debug command in Play Mode by name. Arguments are a JSON object of parameter name to string value, e.g. --args '{\"amount\":\"100\"}'; omitted optional parameters use defaults. Numbers use invariant culture, vectors '(x, y, z)', ids accept the id key or asset name. Use larje.debug list first.";

        [CliGroupCommand(GROUP, LIST_DESCRIPTION, Tags = new[] { "larje", "runtime" })]
        public static object List(
            [CliArg("group", "Group filter, e.g. Saves, Economy, UI, Levels")] string group = null)
        {
            IDebugCommandRegistry registry = RequireRegistry();
            List<object> commands = new List<object>();
            foreach (DebugCommandInfo command in registry.Commands)
            {
                if (string.IsNullOrEmpty(group) || string.Equals(group, command.Group, StringComparison.OrdinalIgnoreCase))
                {
                    commands.Add(Describe(command));
                }
            }

            return commands;
        }

        [CliGroupCommand(GROUP, RUN_DESCRIPTION, Tags = new[] { "larje", "runtime" })]
        public static object Run(
            [CliArg("name", "Command name from larje.debug list", Required = true)] string name,
            [CliArg("args", "Arguments as a JSON object of name to value")] Dictionary<string, string> args = null)
        {
            IDebugCommandRegistry registry = RequireRegistry();
            DebugCommandResult result = registry.Execute(name, args ?? new Dictionary<string, string>());
            if (!result.Success)
            {
                throw new InvalidOperationException(result.Message);
            }

            return result.Message;
        }

        private static IDebugCommandRegistry RequireRegistry()
        {
            IDebugCommandRegistry registry = DebugCommandRegistry.Current;
            if (!EditorApplication.isPlaying || registry == null)
            {
                throw new InvalidOperationException("Debug commands are available only in Play Mode with a DebugCommandRegistry created by the game");
            }

            return registry;
        }

        private static object Describe(DebugCommandInfo command)
        {
            List<object> parameters = new List<object>();
            foreach (DebugParameterInfo parameter in command.Parameters)
            {
                parameters.Add(new
                {
                    name = parameter.Name,
                    type = parameter.TypeName,
                    optional = parameter.HasDefault,
                    defaultValue = parameter.HasDefault && parameter.DefaultValue != null ? parameter.DefaultValue.ToString() : null,
                    options = parameter.GetOptions()
                });
            }

            return new { name = command.Name, group = command.Group, description = command.Description, parameters };
        }
    }
}
