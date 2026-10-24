using System.Collections.Generic;
using Larje.Core.Locations;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Larje.Core.Editor.Locations
{
    public sealed class LocationArgPreviewWindow : EditorWindow
    {
        [SerializeField] private List<LocationArgId> args = new List<LocationArgId>();

        private SerializedObject _serialized;

        [MenuItem("Tools/Larje/Location Arg Preview")]
        public static void Open()
        {
            GetWindow<LocationArgPreviewWindow>("Location Args");
        }

        public static int ApplyToScene(IReadOnlyList<LocationArgId> args)
        {
#if UNITY_6000_4_OR_NEWER
            LocationArgActivator[] activators = FindObjectsByType<LocationArgActivator>(FindObjectsInactive.Include);
#else
            LocationArgActivator[] activators = FindObjectsByType<LocationArgActivator>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#endif
            Undo.IncrementCurrentGroup();
            int group = Undo.GetCurrentGroup();
            foreach (LocationArgActivator activator in activators)
            {
                Undo.RegisterFullObjectHierarchyUndo(activator.gameObject, "Apply Location Args");
                activator.Apply(args);
                EditorUtility.SetDirty(activator.gameObject);
            }

            Undo.CollapseUndoOperations(group);
            EditorSceneManager.MarkAllScenesDirty();
            return activators.Length;
        }

        private void OnEnable()
        {
            _serialized = new SerializedObject(this);
        }

        private void OnGUI()
        {
            _serialized.Update();
            EditorGUILayout.PropertyField(_serialized.FindProperty(nameof(args)), true);
            _serialized.ApplyModifiedProperties();

            GUILayout.Space(10f);
            if (GUILayout.Button("Apply To Scene"))
            {
                ApplyToScene(args);
            }
        }
    }
}
