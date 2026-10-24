using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Larje.Core.Editor.Tools
{
    public static class TransformClipboard
    {
        private static readonly List<Entry> s_buffer = new List<Entry>();

        public static int Count => s_buffer.Count;

        public static int Copy(IReadOnlyList<Transform> sources)
        {
            s_buffer.Clear();
            foreach (Transform source in sources)
            {
                s_buffer.Add(new Entry
                {
                    Name = source.name,
                    LocalPosition = source.localPosition,
                    LocalRotation = source.localRotation,
                    LocalScale = source.localScale,
                    WorldPosition = source.position,
                    WorldRotation = source.rotation
                });
            }

            return s_buffer.Count;
        }

        public static int Paste(IReadOnlyList<Transform> targets, PasteOptions options)
        {
            Undo.RecordObjects(ToArray(targets), "Paste Transforms");
            int pasted = 0;
            for (int i = 0; i < targets.Count; i++)
            {
                if (TryFind(targets[i], i, options.MatchByName, out Entry entry))
                {
                    Apply(targets[i], entry, options);
                    EditorUtility.SetDirty(targets[i]);
                    pasted++;
                }
            }

            return pasted;
        }

        public static void Clear()
        {
            s_buffer.Clear();
        }

        private static bool TryFind(Transform target, int index, bool byName, out Entry entry)
        {
            if (byName)
            {
                int found = s_buffer.FindIndex(x => x.Name == target.name);
                entry = found >= 0 ? s_buffer[found] : default;
                return found >= 0;
            }

            entry = index < s_buffer.Count ? s_buffer[index] : default;
            return index < s_buffer.Count;
        }

        private static void Apply(Transform target, Entry entry, PasteOptions options)
        {
            if (options.WorldSpace)
            {
                if (options.Position)
                {
                    target.position = entry.WorldPosition;
                }

                if (options.Rotation)
                {
                    target.rotation = entry.WorldRotation;
                }

                return;
            }

            if (options.Position)
            {
                target.localPosition = entry.LocalPosition;
            }

            if (options.Rotation)
            {
                target.localRotation = entry.LocalRotation;
            }

            if (options.Scale)
            {
                target.localScale = entry.LocalScale;
            }
        }

        private static UnityEngine.Object[] ToArray(IReadOnlyList<Transform> targets)
        {
            UnityEngine.Object[] result = new UnityEngine.Object[targets.Count];
            for (int i = 0; i < targets.Count; i++)
            {
                result[i] = targets[i];
            }

            return result;
        }

        [Serializable]
        public struct PasteOptions
        {
            public bool WorldSpace;
            public bool MatchByName;
            public bool Position;
            public bool Rotation;
            public bool Scale;
        }

        private struct Entry
        {
            public string Name;
            public Vector3 LocalPosition;
            public Quaternion LocalRotation;
            public Vector3 LocalScale;
            public Vector3 WorldPosition;
            public Quaternion WorldRotation;
        }
    }
}
