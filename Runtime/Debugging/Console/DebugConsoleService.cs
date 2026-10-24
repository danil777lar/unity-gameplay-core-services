using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Larje.Core.Input;
using Larje.Core.Saves;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Larje.Core.Debugging
{
    public sealed class DebugConsoleService : IDebugConsoleService, IInitializableService, IDisposable
    {
        private const string ROOT_NAME = "[Debug Console]";
        private const string DEFAULT_THEME = "LarjeDebugTheme";

        private bool _enabled;
        private GameObject _root;
        private PanelSettings _panelSettings;
        private DebugConsoleView _view;
        private DebugOverlayStats _stats;
        private IDisposable _inputCondition;
        private readonly DebugConsoleConfig _config;
        private readonly IDebugCommandRegistry _registry;
        private readonly DebugLogBuffer _logs;
        private readonly ISaveService _saves;
        private readonly IInputService _input;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();

        public bool IsEnabled => _enabled;
        public bool IsOpen => _view != null && _view.IsOpen;

        public DebugConsoleService(DebugConsoleConfig config, IDebugCommandRegistry registry, DebugLogBuffer logs, ISaveService saves, IInputService input)
        {
            _config = config;
            _registry = registry;
            _logs = logs;
            _saves = saves;
            _input = input;
        }

        public void Initialize()
        {
            _enabled = _config.IsEnabled;
            if (!_enabled)
            {
                return;
            }

            _root = new GameObject(ROOT_NAME);
            Object.DontDestroyOnLoad(_root);

            _panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            _panelSettings.themeStyleSheet = _config.Theme != null ? _config.Theme : Resources.Load<ThemeStyleSheet>(DEFAULT_THEME);
            _panelSettings.sortingOrder = _config.SortingOrder;
            _panelSettings.scaleMode = PanelScaleMode.ConstantPhysicalSize;
            _panelSettings.scale = _config.Scale;

            UIDocument document = _root.AddComponent<UIDocument>();
            document.panelSettings = _panelSettings;

            DebugConsoleSettingsData settings = _saves.Get<DebugConsoleSettingsData>();
            _view = new DebugConsoleView(document.rootVisualElement, _registry, _logs, settings, _config.ShowToggleButton);
            _view.CloseRequested += Close;
            _view.ToggleRequested += Toggle;
            _view.SettingsChanged += _saves.SaveSystem;
            _stats = new DebugOverlayStats();

            if (_input.Debug != null)
            {
                _input.Debug.performed += OnDebugPerformed;
            }

            if (!string.IsNullOrEmpty(_config.BlockedInputMap))
            {
                _inputCondition = _input.AddCondition(_config.BlockedInputMap, () => !IsOpen, InputConditionOperation.And, "Debug Console");
            }

            ServiceLoop.Run(Tick, _lifetime.Token, PlayerLoopTiming.Update, true);
        }

        public void Dispose()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
            _inputCondition?.Dispose();
            _stats?.Dispose();
            if (_enabled && _input.Debug != null)
            {
                _input.Debug.performed -= OnDebugPerformed;
            }

            if (_root != null)
            {
                Object.Destroy(_root);
            }

            if (_panelSettings != null)
            {
                Object.Destroy(_panelSettings);
            }
        }

        public void Open()
        {
            _view?.SetOpen(true);
        }

        public void Close()
        {
            _view?.SetOpen(false);
        }

        public void Toggle()
        {
            if (IsOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        private void OnDebugPerformed(InputAction.CallbackContext context)
        {
            Toggle();
        }

        private void Tick(float deltaTime)
        {
            _view.Tick(_stats.Build(deltaTime));
        }
    }
}
