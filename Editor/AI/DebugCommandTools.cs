using System.Collections.Generic;
using Larje.Core.Debugging;
using Unity.AI.MCP.Editor.Helpers;
using Unity.AI.MCP.Editor.ToolRegistry;
using UnityEditor;

namespace Larje.Core.Editor.AI
{
    public static class DebugCommandTools
    {
        private const string LIST_DESCRIPTION = "Lists Larje debug commands available in Play Mode (saves, currency, UI, levels, ads, audio, events, game state and project-specific commands). Returns name, group, description and typed parameters with allowed options. Requires Play Mode.";
        private const string RUN_DESCRIPTION = "Runs a Larje debug command in Play Mode by name with string arguments. Use Larje.ListDebugCommands first to get names, parameter names and allowed options. Id parameters accept the id asset key or name.";

        [McpTool("Larje.ListDebugCommands", LIST_DESCRIPTION, "List Larje Debug Commands", EnabledByDefault = true, Groups = new[] { "larje", "larje-runtime" })]
        public static object List(ListParams parameters)
        {
            IDebugCommandRegistry registry = DebugCommandRegistry.Current;
            if (!EditorApplication.isPlaying || registry == null)
            {
                return Response.Error("Debug commands are available only in Play Mode with a DebugCommandRegistry created by the game");
            }

            List<object> commands = new List<object>();
            foreach (DebugCommandInfo command in registry.Commands)
            {
                if (string.IsNullOrEmpty(parameters.Group) || string.Equals(parameters.Group, command.Group, System.StringComparison.OrdinalIgnoreCase))
                {
                    commands.Add(Describe(command));
                }
            }

            return Response.Success($"{commands.Count} commands", commands);
        }

        [McpTool("Larje.RunDebugCommand", RUN_DESCRIPTION, "Run Larje Debug Command", EnabledByDefault = true, Groups = new[] { "larje", "larje-runtime" })]
        public static object Run(RunParams parameters)
        {
            IDebugCommandRegistry registry = DebugCommandRegistry.Current;
            if (!EditorApplication.isPlaying || registry == null)
            {
                return Response.Error("Debug commands are available only in Play Mode");
            }

            Dictionary<string, string> arguments = new Dictionary<string, string>();
            if (parameters.Arguments != null)
            {
                foreach (CommandArgument argument in parameters.Arguments)
                {
                    arguments[argument.Name] = argument.Value;
                }
            }

            DebugCommandResult result = registry.Execute(parameters.Command, arguments);
            return result.Success ? Response.Success(result.Message) : Response.Error(result.Message);
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

        public sealed class ListParams
        {
            [McpDescription("Optional group filter, e.g. Saves, Economy, UI, Levels")]
            public string Group { get; set; }
        }

        public sealed class RunParams
        {
            [McpDescription("Command name from Larje.ListDebugCommands", Required = true)]
            public string Command { get; set; }

            [McpDescription("Arguments as name/value pairs; omitted optional arguments use defaults")]
            public List<CommandArgument> Arguments { get; set; }
        }

        public sealed class CommandArgument
        {
            [McpDescription("Parameter name", Required = true)]
            public string Name { get; set; }

            [McpDescription("Value as string: numbers use invariant culture, vectors '(x, y, z)', ids use key or asset name", Required = true)]
            public string Value { get; set; }
        }
    }
}
