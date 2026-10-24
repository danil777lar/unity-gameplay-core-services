using UnityEngine;
using UnityEngine.InputSystem;

namespace Larje.Core.Input
{
    [CreateAssetMenu(fileName = "InputActionDrawerText", menuName = "Larje/Core/Input/Drawers/Text")]
    public sealed class InputActionDrawerText : InputActionDrawer
    {
        [SerializeField] private string leftModifier;
        [SerializeField] private string rightModifier;

        public override object Draw(InputAction action)
        {
            string title = string.Empty;
            if (action != null && action.bindings.Count > 0)
            {
                title = action.GetBindingDisplayString(0);
            }

            return leftModifier + title + rightModifier;
        }
    }
}
