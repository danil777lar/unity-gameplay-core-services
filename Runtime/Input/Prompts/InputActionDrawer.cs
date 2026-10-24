using UnityEngine;
using UnityEngine.InputSystem;

namespace Larje.Core.Input
{
    public abstract class InputActionDrawer : ScriptableObject
    {
        public abstract object Draw(InputAction action);
    }
}
