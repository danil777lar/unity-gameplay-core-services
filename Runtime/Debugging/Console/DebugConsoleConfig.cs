using UnityEngine;
using UnityEngine.UIElements;

namespace Larje.Core.Debugging
{
    [CreateAssetMenu(fileName = "DebugConsoleConfig", menuName = "Larje/Core/Debugging/Debug Console Config")]
    public sealed class DebugConsoleConfig : ScriptableObject
    {
        [Header("Availability")]
        [SerializeField] private bool enabledInEditor = true;
        [SerializeField] private bool enabledInDevelopmentBuild = true;
        [SerializeField] private bool enabledInReleaseBuild;

        [Header("Controls")]
        [SerializeField] private bool showToggleButton = true;
        [SerializeField] private string blockedInputMap = "Player";

        [Header("Look")]
        [SerializeField] private ThemeStyleSheet theme;
        [SerializeField] private int sortingOrder = 30000;
        [SerializeField] private float scale = 1f;

        public bool ShowToggleButton => showToggleButton;
        public string BlockedInputMap => blockedInputMap;
        public ThemeStyleSheet Theme => theme;
        public int SortingOrder => sortingOrder;
        public float Scale => scale;

        public bool IsEnabled
        {
            get
            {
                if (Application.isEditor)
                {
                    return enabledInEditor;
                }

                return Debug.isDebugBuild ? enabledInDevelopmentBuild : enabledInReleaseBuild;
            }
        }
    }
}
