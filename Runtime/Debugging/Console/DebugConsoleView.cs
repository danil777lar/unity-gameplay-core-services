using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Larje.Core.Debugging
{
    public sealed class DebugConsoleView
    {
        private const int VISIBLE_LOGS = 200;

        private static readonly Color s_background = new Color(0.08f, 0.08f, 0.1f, 0.95f);
        private static readonly Color s_panel = new Color(0.14f, 0.14f, 0.17f, 1f);
        private static readonly Color s_accent = new Color(0.36f, 0.6f, 1f, 1f);

        private int _logsVersion = -1;
        private readonly IDebugCommandRegistry _registry;
        private readonly DebugLogBuffer _logs;
        private readonly DebugConsoleSettingsData _settings;
        private readonly VisualElement _panel;
        private readonly Label _overlay;
        private readonly Button _toggle;
        private readonly ScrollView _commands;
        private readonly ScrollView _logList;
        private readonly VisualElement _settingsView;
        private readonly List<Button> _tabs = new List<Button>();

        public bool IsOpen => _panel.style.display == DisplayStyle.Flex;

        public event Action CloseRequested;
        public event Action ToggleRequested;
        public event Action SettingsChanged;

        public DebugConsoleView(VisualElement root, IDebugCommandRegistry registry, DebugLogBuffer logs, DebugConsoleSettingsData settings, bool showToggle)
        {
            _registry = registry;
            _logs = logs;
            _settings = settings;

            root.style.position = Position.Absolute;
            root.style.left = 0f;
            root.style.right = 0f;
            root.style.top = 0f;
            root.style.bottom = 0f;
            root.pickingMode = PickingMode.Ignore;

            _overlay = new Label();
            _overlay.pickingMode = PickingMode.Ignore;
            _overlay.style.position = Position.Absolute;
            _overlay.style.left = 8f;
            _overlay.style.top = 8f;
            _overlay.style.color = Color.white;
            _overlay.style.backgroundColor = new Color(0f, 0f, 0f, 0.4f);
            _overlay.style.unityFontStyleAndWeight = FontStyle.Bold;
            root.Add(_overlay);

            _toggle = new Button(() => ToggleRequested?.Invoke()) { text = "DBG" };
            _toggle.style.position = Position.Absolute;
            _toggle.style.right = 8f;
            _toggle.style.bottom = 8f;
            _toggle.style.opacity = 0.6f;
            _toggle.style.display = showToggle ? DisplayStyle.Flex : DisplayStyle.None;
            root.Add(_toggle);

            _panel = new VisualElement();
            _panel.style.position = Position.Absolute;
            _panel.style.left = 0f;
            _panel.style.right = 0f;
            _panel.style.top = 0f;
            _panel.style.bottom = 0f;
            _panel.style.backgroundColor = s_background;
            _panel.style.display = DisplayStyle.None;
            root.Add(_panel);

            VisualElement header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.backgroundColor = s_panel;
            _panel.Add(header);

            _commands = new ScrollView();
            _commands.style.flexGrow = 1f;
            _logList = new ScrollView();
            _logList.style.flexGrow = 1f;
            _settingsView = new VisualElement();
            _settingsView.style.flexGrow = 1f;
            _settingsView.style.paddingLeft = 12f;
            _settingsView.style.paddingTop = 12f;

            AddTab(header, "Commands", _commands);
            AddTab(header, "Logs", _logList);
            AddTab(header, "Settings", _settingsView);

            VisualElement spacer = new VisualElement();
            spacer.style.flexGrow = 1f;
            header.Add(spacer);
            header.Add(new Button(() => CloseRequested?.Invoke()) { text = "Close" });

            _panel.Add(_commands);
            _panel.Add(_logList);
            _panel.Add(_settingsView);

            BuildCommands();
            BuildSettings();
            SelectTab(0);
            ApplyOverlaySettings();
        }

        public void SetOpen(bool open)
        {
            _panel.style.display = open ? DisplayStyle.Flex : DisplayStyle.None;
            _toggle.style.visibility = open ? Visibility.Hidden : Visibility.Visible;
            if (open)
            {
                BuildCommands();
            }
        }

        public void Tick(string overlayText)
        {
            if (_settings.OverlayVisible)
            {
                _overlay.text = overlayText;
            }

            if (IsOpen && _logList.style.display == DisplayStyle.Flex && _logs.Version != _logsVersion)
            {
                RebuildLogs();
            }
        }

        private void AddTab(VisualElement header, string title, VisualElement content)
        {
            int index = _tabs.Count;
            Button tab = new Button(() => SelectTab(index)) { text = title };
            tab.userData = content;
            _tabs.Add(tab);
            header.Add(tab);
        }

        private void SelectTab(int index)
        {
            for (int i = 0; i < _tabs.Count; i++)
            {
                VisualElement content = (VisualElement)_tabs[i].userData;
                content.style.display = i == index ? DisplayStyle.Flex : DisplayStyle.None;
                _tabs[i].style.borderBottomColor = i == index ? s_accent : Color.clear;
                _tabs[i].style.borderBottomWidth = 2f;
            }

            _logsVersion = -1;
        }

        private void BuildCommands()
        {
            _commands.Clear();
            Dictionary<string, Foldout> groups = new Dictionary<string, Foldout>();
            foreach (DebugCommandInfo command in _registry.Commands)
            {
                if (!groups.TryGetValue(command.Group, out Foldout group))
                {
                    group = new Foldout { text = command.Group, value = false };
                    group.style.backgroundColor = s_panel;
                    group.style.marginBottom = 4f;
                    groups[command.Group] = group;
                    _commands.Add(group);
                }

                group.Add(BuildCommand(command));
            }

            if (_registry.Commands.Count == 0)
            {
                _commands.Add(new Label("No commands registered"));
            }
        }

        private VisualElement BuildCommand(DebugCommandInfo command)
        {
            VisualElement row = new VisualElement();
            row.style.paddingLeft = 8f;
            row.style.paddingBottom = 8f;

            Label title = new Label(command.Name);
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            row.Add(title);
            if (!string.IsNullOrEmpty(command.Description))
            {
                Label description = new Label(command.Description);
                description.style.opacity = 0.7f;
                row.Add(description);
            }

            List<Func<string>> readers = new List<Func<string>>();
            foreach (DebugParameterInfo parameter in command.Parameters)
            {
                readers.Add(AddParameterField(row, parameter));
            }

            Label result = new Label();
            Button run = new Button(() =>
            {
                Dictionary<string, string> arguments = new Dictionary<string, string>();
                for (int i = 0; i < command.Parameters.Count; i++)
                {
                    arguments[command.Parameters[i].Name] = readers[i]();
                }

                DebugCommandResult outcome = _registry.Execute(command.Name, arguments);
                result.text = outcome.Message;
                result.style.color = outcome.Success ? new Color(0.6f, 1f, 0.6f) : new Color(1f, 0.5f, 0.5f);
            })
            { text = "Run" };
            run.style.alignSelf = Align.FlexStart;
            row.Add(run);
            row.Add(result);
            return row;
        }

        private Func<string> AddParameterField(VisualElement row, DebugParameterInfo parameter)
        {
            string label = parameter.Name + " (" + parameter.TypeName + ")";
            string initial = parameter.HasDefault && parameter.DefaultValue != null ? parameter.DefaultValue.ToString() : string.Empty;
            IReadOnlyList<string> options = parameter.GetOptions();
            if (options.Count > 0)
            {
                List<string> choices = new List<string>(options);
                DropdownField dropdown = new DropdownField(label, choices, Mathf.Max(0, choices.IndexOf(initial)));
                row.Add(dropdown);
                return () => dropdown.value;
            }

            TextField field = new TextField(label) { value = initial };
            row.Add(field);
            return () => field.value;
        }

        private void BuildSettings()
        {
            Toggle overlay = new Toggle("Show Overlay") { value = _settings.OverlayVisible };
            overlay.RegisterValueChangedCallback(evt =>
            {
                _settings.OverlayVisible = evt.newValue;
                ApplyOverlaySettings();
                SettingsChanged?.Invoke();
            });
            _settingsView.Add(overlay);

            Slider opacity = new Slider("Overlay Opacity", 0.1f, 1f) { value = _settings.OverlayOpacity };
            opacity.RegisterValueChangedCallback(evt =>
            {
                _settings.OverlayOpacity = evt.newValue;
                ApplyOverlaySettings();
                SettingsChanged?.Invoke();
            });
            _settingsView.Add(opacity);

            _settingsView.Add(new Button(() =>
            {
                _logs.Clear();
                _logsVersion = -1;
            })
            { text = "Clear Logs" });
        }

        private void ApplyOverlaySettings()
        {
            _overlay.style.display = _settings.OverlayVisible ? DisplayStyle.Flex : DisplayStyle.None;
            _overlay.style.opacity = _settings.OverlayOpacity;
        }

        private void RebuildLogs()
        {
            _logsVersion = _logs.Version;
            _logList.Clear();
            foreach (DebugLogEntry entry in _logs.GetEntries(VISIBLE_LOGS))
            {
                Label line = new Label($"[{entry.Time:0.00}] {entry.Message}");
                line.style.whiteSpace = WhiteSpace.Normal;
                line.style.color = GetColor(entry.Type);
                line.tooltip = entry.StackTrace;
                _logList.Add(line);
            }

            _logList.schedule.Execute(() => _logList.scrollOffset = new Vector2(0f, float.MaxValue));
        }

        private Color GetColor(LogType type)
        {
            switch (type)
            {
                case LogType.Warning:
                    return new Color(1f, 0.85f, 0.4f);
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    return new Color(1f, 0.5f, 0.5f);
                default:
                    return new Color(0.9f, 0.9f, 0.9f);
            }
        }
    }
}
