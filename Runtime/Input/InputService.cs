using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Larje.Core.Input
{
    public sealed class InputService : IInputService, IInitializableService, IDisposable
    {
        private readonly InputConfig _config;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();
        private readonly Dictionary<string, List<Condition>> _conditions = new Dictionary<string, List<Condition>>();

        public InputActionAsset Actions => _config.Actions;
        public InputAction Back => Resolve(_config.Back);
        public InputAction Debug => Resolve(_config.Debug);
        public InputAction Move => Resolve(_config.Move);
        public InputAction Run => Resolve(_config.Run);
        public InputAction Pointer => Resolve(_config.Pointer);
        public Vector2 Movement => Move != null ? Move.ReadValue<Vector2>() : Vector2.zero;

        public InputService(InputConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            if (_config.Actions == null)
            {
                UnityEngine.Debug.LogWarning("InputService: InputConfig has no InputActionAsset");
                return;
            }

            UpdateMaps(0f);
            ServiceLoop.Run(UpdateMaps, _lifetime.Token, Cysharp.Threading.Tasks.PlayerLoopTiming.EarlyUpdate);
        }

        public void Dispose()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
            if (_config.Actions != null)
            {
                _config.Actions.Disable();
            }
        }

        public InputAction Find(string actionPath)
        {
            return _config.Actions != null ? _config.Actions.FindAction(actionPath) : null;
        }

        public IDisposable AddCondition(string map, Func<bool> condition, InputConditionOperation operation = InputConditionOperation.And, string name = "")
        {
            if (!_conditions.TryGetValue(map, out List<Condition> list))
            {
                list = new List<Condition>();
                _conditions[map] = list;
            }

            Condition entry = new Condition(condition, operation, name, list);
            list.Add(entry);
            return entry;
        }

        public IReadOnlyList<InputMapState> GetMapStates()
        {
            List<InputMapState> states = new List<InputMapState>();
            if (_config.Actions == null)
            {
                return states;
            }

            foreach (InputActionMap map in _config.Actions.actionMaps)
            {
                int count = _conditions.TryGetValue(map.name, out List<Condition> list) ? list.Count : 0;
                states.Add(new InputMapState(map.name, map.enabled, count));
            }

            return states;
        }

        private InputAction Resolve(InputActionReference reference)
        {
            return reference != null ? reference.action : null;
        }

        private void UpdateMaps(float deltaTime)
        {
            foreach (InputActionMap map in _config.Actions.actionMaps)
            {
                bool enabled = Evaluate(map.name);
                if (enabled && !map.enabled)
                {
                    map.Enable();
                }
                else if (!enabled && map.enabled)
                {
                    map.Disable();
                }
            }
        }

        private bool Evaluate(string map)
        {
            bool enabled = _config.IsEnabledByDefault(map);
            if (!_conditions.TryGetValue(map, out List<Condition> list))
            {
                return enabled;
            }

            foreach (Condition condition in list)
            {
                if (condition.Operation == InputConditionOperation.Or)
                {
                    enabled |= condition.Evaluate();
                }
            }

            foreach (Condition condition in list)
            {
                if (condition.Operation == InputConditionOperation.And)
                {
                    enabled &= condition.Evaluate();
                }
            }

            return enabled;
        }

        private sealed class Condition : IDisposable
        {
            public readonly InputConditionOperation Operation;
            public readonly string Name;

            private readonly Func<bool> _predicate;
            private List<Condition> _owner;

            public Condition(Func<bool> predicate, InputConditionOperation operation, string name, List<Condition> owner)
            {
                _predicate = predicate;
                Operation = operation;
                Name = name;
                _owner = owner;
            }

            public bool Evaluate()
            {
                return _predicate();
            }

            public void Dispose()
            {
                _owner?.Remove(this);
                _owner = null;
            }
        }
    }
}
