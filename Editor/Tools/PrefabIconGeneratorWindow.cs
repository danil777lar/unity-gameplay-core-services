using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Larje.Core.Editor.Tools
{
    public sealed class PrefabIconGeneratorWindow : EditorWindow
    {
        private bool _addToAddressables;
        private string _addressablesGroup = string.Empty;
        private Vector2 _scroll;
        private PrefabIconRenderer.Settings _settings = PrefabIconRenderer.Settings.Default;

        [MenuItem("Tools/Larje/Prefab Icon Generator")]
        public static void Open()
        {
            GetWindow<PrefabIconGeneratorWindow>("Prefab Icons");
        }

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            DrawOutput();
            DrawResolution();
            DrawCamera();
            DrawBackground();
            DrawAddressables();
            DrawGenerate();
            EditorGUILayout.EndScrollView();
        }

        private void DrawOutput()
        {
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                _settings.OutputFolder = EditorGUILayout.TextField("Folder", _settings.OutputFolder);
                if (GUILayout.Button("...", GUILayout.Width(28f)))
                {
                    string picked = EditorUtility.OpenFolderPanel("Output Folder", "Assets", string.Empty);
                    string project = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                    if (!string.IsNullOrEmpty(picked) && picked.StartsWith(project))
                    {
                        _settings.OutputFolder = picked.Substring(project.Length + 1).Replace('\\', '/');
                    }
                }
            }

            _settings.FilePrefix = EditorGUILayout.TextField("File Prefix", _settings.FilePrefix);
        }

        private void DrawResolution()
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Resolution", EditorStyles.boldLabel);
            _settings.Width = Mathf.Max(1, EditorGUILayout.IntField("Width", _settings.Width));
            _settings.Height = Mathf.Max(1, EditorGUILayout.IntField("Height", _settings.Height));
            using (new EditorGUILayout.HorizontalScope())
            {
                int[] presets = { 64, 128, 256, 512 };
                foreach (int preset in presets)
                {
                    if (GUILayout.Button(preset.ToString()))
                    {
                        _settings.Width = preset;
                        _settings.Height = preset;
                    }
                }
            }
        }

        private void DrawCamera()
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Camera", EditorStyles.boldLabel);
            _settings.CameraAngle = EditorGUILayout.Vector3Field("Angle", _settings.CameraAngle);
            _settings.Padding = EditorGUILayout.Slider("Padding", _settings.Padding, 0f, 0.5f);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Isometric"))
                {
                    _settings.CameraAngle = new Vector3(30f, -45f, 0f);
                }

                if (GUILayout.Button("Top-Down"))
                {
                    _settings.CameraAngle = new Vector3(90f, 0f, 0f);
                }

                if (GUILayout.Button("Front"))
                {
                    _settings.CameraAngle = Vector3.zero;
                }

                if (GUILayout.Button("Side"))
                {
                    _settings.CameraAngle = new Vector3(0f, -90f, 0f);
                }
            }
        }

        private void DrawBackground()
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Background", EditorStyles.boldLabel);
            _settings.Transparent = EditorGUILayout.Toggle("Transparent", _settings.Transparent);
            using (new EditorGUI.DisabledScope(_settings.Transparent))
            {
                _settings.Background = EditorGUILayout.ColorField("Color", _settings.Background);
            }
        }

        private void DrawAddressables()
        {
            if (!AddressablesEditorUtility.Available)
            {
                return;
            }

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Addressables", EditorStyles.boldLabel);
            _addToAddressables = EditorGUILayout.Toggle("Add To Addressables", _addToAddressables);
            using (new EditorGUI.DisabledScope(!_addToAddressables))
            {
                _addressablesGroup = EditorGUILayout.TextField("Group", _addressablesGroup);
            }
        }

        private void DrawGenerate()
        {
            EditorGUILayout.Space(10f);
            List<GameObject> prefabs = GetSelectedPrefabs();
            using (new EditorGUI.DisabledScope(prefabs.Count == 0))
            {
                if (GUILayout.Button($"Generate ({prefabs.Count})", GUILayout.Height(36f)))
                {
                    Generate(prefabs);
                }
            }

            if (prefabs.Count == 0)
            {
                EditorGUILayout.HelpBox("Select prefab assets in the Project window.", MessageType.Info);
            }
        }

        private void Generate(List<GameObject> prefabs)
        {
            int done = 0;
            try
            {
                foreach (GameObject prefab in prefabs)
                {
                    EditorUtility.DisplayProgressBar("Prefab Icons", prefab.name, (float)done / prefabs.Count);
                    string path = PrefabIconRenderer.Render(prefab, _settings);
                    if (_addToAddressables)
                    {
                        AddressablesEditorUtility.AddToGroup(path, _addressablesGroup);
                    }

                    done++;
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            ShowNotification(new GUIContent($"Generated {done}"));
        }

        private List<GameObject> GetSelectedPrefabs()
        {
            List<GameObject> prefabs = new List<GameObject>();
            foreach (Object selected in Selection.objects)
            {
                if (selected is GameObject gameObject && PrefabUtility.IsPartOfPrefabAsset(gameObject))
                {
                    prefabs.Add(gameObject);
                }
            }

            return prefabs;
        }
    }
}
