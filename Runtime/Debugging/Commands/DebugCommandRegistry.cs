using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Larje.Core.Debugging
{
    public sealed class DebugCommandRegistry : IDebugCommandRegistry
    {
        private const BindingFlags METHOD_FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly List<DebugCommandInfo> _commands = new List<DebugCommandInfo>();

        public IReadOnlyList<DebugCommandInfo> Commands => _commands;

        public void Register(object provider)
        {
            if (provider == null)
            {
                return;
            }

            Unregister(provider);
            foreach (MethodInfo method in provider.GetType().GetMethods(METHOD_FLAGS))
            {
                DebugCommandAttribute attribute = method.GetCustomAttribute<DebugCommandAttribute>();
                if (attribute != null)
                {
                    _commands.Add(Create(provider, method, attribute));
                }
            }

            _commands.Sort((a, b) =>
            {
                int group = string.CompareOrdinal(a.Group, b.Group);
                return group != 0 ? group : string.CompareOrdinal(a.Name, b.Name);
            });
        }

        public void Unregister(object provider)
        {
            _commands.RemoveAll(x => ReferenceEquals(x.Target, provider));
        }

        public DebugCommandInfo Find(string name)
        {
            return _commands.Find(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public DebugCommandResult Execute(string name, IReadOnlyDictionary<string, string> arguments)
        {
            DebugCommandInfo command = Find(name);
            if (command == null)
            {
                return DebugCommandResult.Fail($"Unknown command '{name}'");
            }

            object[] values = new object[command.Parameters.Count];
            for (int i = 0; i < command.Parameters.Count; i++)
            {
                DebugParameterInfo parameter = command.Parameters[i];
                if (arguments != null && arguments.TryGetValue(parameter.Name, out string raw))
                {
                    if (!DebugValueParser.TryParse(raw, parameter.Type, out object parsed))
                    {
                        return DebugCommandResult.Fail($"Cannot parse '{raw}' as {parameter.TypeName} for '{parameter.Name}'");
                    }

                    values[i] = parsed;
                }
                else if (parameter.HasDefault)
                {
                    values[i] = parameter.DefaultValue;
                }
                else
                {
                    return DebugCommandResult.Fail($"Missing argument '{parameter.Name}' ({parameter.TypeName})");
                }
            }

            return Execute(command, values);
        }

        public DebugCommandResult Execute(DebugCommandInfo command, object[] arguments)
        {
            try
            {
                object returned = command.Method.Invoke(command.Method.IsStatic ? null : command.Target, arguments);
                string message = returned != null ? returned.ToString() : "OK";
                return new DebugCommandResult(true, message, returned);
            }
            catch (TargetInvocationException exception)
            {
                Exception inner = exception.InnerException ?? exception;
                Debug.LogException(inner);
                return DebugCommandResult.Fail(inner.Message);
            }
            catch (ArgumentException exception)
            {
                return DebugCommandResult.Fail(exception.Message);
            }
        }

        private DebugCommandInfo Create(object provider, MethodInfo method, DebugCommandAttribute attribute)
        {
            ParameterInfo[] parameters = method.GetParameters();
            List<DebugParameterInfo> infos = new List<DebugParameterInfo>(parameters.Length);
            foreach (ParameterInfo parameter in parameters)
            {
                infos.Add(new DebugParameterInfo(parameter.Name, parameter.ParameterType, parameter.HasDefaultValue, parameter.HasDefaultValue ? parameter.DefaultValue : null));
            }

            string name = string.IsNullOrEmpty(attribute.Name) ? method.Name : attribute.Name;
            return new DebugCommandInfo(name, attribute.Group, attribute.Description, infos, provider, method);
        }
    }
}
