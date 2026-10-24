using System;
using System.Collections.Generic;
using System.IO;
using Larje.Core.Ids;
using UnityEditor;
using UnityEngine;

namespace Larje.Core.Editor.Ids
{
    public static class LarjeIdAssets
    {
        private const string DEFAULT_FOLDER = "Assets/Configs/Ids";

        public static List<LarjeId> FindAll(Type idType)
        {
            List<LarjeId> ids = new List<LarjeId>();
            foreach (string guid in AssetDatabase.FindAssets("t:" + idType.Name))
            {
                LarjeId id = AssetDatabase.LoadAssetAtPath<LarjeId>(AssetDatabase.GUIDToAssetPath(guid));
                if (id != null && idType.IsInstanceOfType(id))
                {
                    ids.Add(id);
                }
            }

            ids.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
            return ids;
        }

        public static LarjeId Create(Type idType, string idName, string folder = DEFAULT_FOLDER)
        {
            string typeFolder = Path.Combine(folder, idType.Name).Replace('\\', '/');
            Directory.CreateDirectory(typeFolder);

            string path = AssetDatabase.GenerateUniqueAssetPath(typeFolder + "/" + idName + ".asset");
            LarjeId id = (LarjeId)ScriptableObject.CreateInstance(idType);
            id.name = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(id, path);
            AssetDatabase.SaveAssets();
            return id;
        }

        public static LarjeId CreateWithDialog(Type idType)
        {
            string path = EditorUtility.SaveFilePanelInProject("Create " + idType.Name, idType.Name, "asset", string.Empty, DEFAULT_FOLDER);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            LarjeId id = (LarjeId)ScriptableObject.CreateInstance(idType);
            AssetDatabase.CreateAsset(id, path);
            AssetDatabase.SaveAssets();
            return id;
        }
    }
}
