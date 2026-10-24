using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Larje.Core.Input
{
    public interface IInputService
    {
        InputActionAsset Actions { get; }
        InputAction Back { get; }
        InputAction Debug { get; }
        InputAction Move { get; }
        InputAction Run { get; }
        InputAction Pointer { get; }
        Vector2 Movement { get; }

        InputAction Find(string actionPath);
        IDisposable AddCondition(string map, Func<bool> condition, InputConditionOperation operation = InputConditionOperation.And, string name = "");
        IReadOnlyList<InputMapState> GetMapStates();
    }
}
