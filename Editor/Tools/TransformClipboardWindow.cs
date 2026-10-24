using UnityEditor;
using UnityEngine;

namespace Larje.Core.Editor.Tools
{
    public sealed class TransformClipboardWindow : EditorWindow
    {
        private TransformClipboard.PasteOptions _options = new TransformClipboard.PasteOptions
        {
            Position = true,
            Rotation = true,
            Scale = true
        };

        [MenuItem("Tools/Larje/Transform Clipboard")]
        public static void Open()
        {
            GetWindow<TransformClipboardWindow>("Transform Clipboard");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Copy", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Copy Selection", GUILayout.Height(28f)))
                {
                    int copied = TransformClipboard.Copy(Selection.transforms);
                    ShowNotification(new GUIContent($"Copied {copied}"));
                }

                if (GUILayout.Button("Clear", GUILayout.Height(28f)))
                {
                    TransformClipboard.Clear();
                }
            }

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Paste", EditorStyles.boldLabel);
            _options.WorldSpace = EditorGUILayout.Toggle("World Space", _options.WorldSpace);
            _options.MatchByName = EditorGUILayout.Toggle("Match By Name", _options.MatchByName);
            _options.Position = EditorGUILayout.ToggleLeft("Position", _options.Position);
            _options.Rotation = EditorGUILayout.ToggleLeft("Rotation", _options.Rotation);
            using (new EditorGUI.DisabledScope(_options.WorldSpace))
            {
                _options.Scale = EditorGUILayout.ToggleLeft("Scale", _options.Scale);
            }

            using (new EditorGUI.DisabledScope(TransformClipboard.Count == 0))
            {
                if (GUILayout.Button("Paste To Selection", GUILayout.Height(32f)))
                {
                    int pasted = TransformClipboard.Paste(Selection.transforms, _options);
                    ShowNotification(new GUIContent($"Pasted {pasted}"));
                }
            }

            EditorGUILayout.HelpBox($"Buffer: {TransformClipboard.Count}", MessageType.Info);
        }
    }
}
