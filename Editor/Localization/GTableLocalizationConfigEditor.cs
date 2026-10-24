using System.IO;
using Larje.Core.Localization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Larje.Core.Editor.Localization
{
    [CustomEditor(typeof(GTableLocalizationConfig))]
    public sealed class GTableLocalizationConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10f);
            if (GUILayout.Button("Download Tables To Cache"))
            {
                DownloadCache((GTableLocalizationConfig)target);
            }
        }

        public static void DownloadCache(GTableLocalizationConfig config)
        {
            string folder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(config));
            foreach (string table in config.Tables)
            {
                using (UnityWebRequest request = UnityWebRequest.Get(config.GetTableUrl(table)))
                {
                    UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                    while (!operation.isDone)
                    {
                        EditorUtility.DisplayProgressBar("Localization", table, request.downloadProgress);
                    }

                    EditorUtility.ClearProgressBar();
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogWarning($"GTable: failed to download '{table}': {request.error}");
                    }
                    else
                    {
                        string path = Path.Combine(folder, $"{config.name}_{table}.json").Replace('\\', '/');
                        File.WriteAllText(path, request.downloadHandler.text);
                        AssetDatabase.ImportAsset(path);
                        config.SetCache(table, AssetDatabase.LoadAssetAtPath<TextAsset>(path));
                    }
                }
            }

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }
    }
}
