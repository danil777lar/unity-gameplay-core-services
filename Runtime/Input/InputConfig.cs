using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Larje.Core.Input
{
    [CreateAssetMenu(fileName = "InputConfig", menuName = "Larje/Core/Input/Input Config")]
    public sealed class InputConfig : ScriptableObject
    {
        [Header("Asset")]
        [SerializeField] private InputActionAsset actions;
        [SerializeField] private List<MapSettings> maps = new List<MapSettings>();

        [Header("UI Actions")]
        [SerializeField] private InputActionReference back;
        [SerializeField] private InputActionReference debug;

        [Header("Player Actions")]
        [SerializeField] private InputActionReference move;
        [SerializeField] private InputActionReference run;
        [SerializeField] private InputActionReference pointer;

        public InputActionAsset Actions => actions;
        public IReadOnlyList<MapSettings> Maps => maps;
        public InputActionReference Back => back;
        public InputActionReference Debug => debug;
        public InputActionReference Move => move;
        public InputActionReference Run => run;
        public InputActionReference Pointer => pointer;

        public bool IsEnabledByDefault(string map)
        {
            MapSettings settings = maps.Find(x => x.Name == map);
            return settings == null || settings.EnabledByDefault;
        }

        [Serializable]
        public sealed class MapSettings
        {
            public string Name;
            public bool EnabledByDefault = true;
        }
    }
}
