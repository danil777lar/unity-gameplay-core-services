using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Localization
{
    [CreateAssetMenu(fileName = "GTableLocalizationConfig", menuName = "Larje/Core/Localization/GTable Localization Config")]
    public sealed class GTableLocalizationConfig : ScriptableObject
    {
        [Header("Source")]
        [SerializeField] private bool loadFromWeb = true;
        [SerializeField] private string url;
        [SerializeField] private List<string> tables = new List<string>();
        [SerializeField] private float timeout = 10f;

        [Header("Languages")]
        [SerializeField] private string defaultLanguage = "en";
        [SerializeField] private string editorLanguageOverride;

        [Header("Cache")]
        [SerializeField] private List<TableCache> cache = new List<TableCache>();

        public bool LoadFromWeb => loadFromWeb;
        public string Url => url;
        public IReadOnlyList<string> Tables => tables;
        public float Timeout => timeout;
        public string DefaultLanguage => defaultLanguage;
        public string EditorLanguageOverride => editorLanguageOverride;
        public IReadOnlyList<TableCache> Cache => cache;

        public string GetTableUrl(string table)
        {
            return $"{url}?name={UnityEngine.Networking.UnityWebRequest.EscapeURL(table)}";
        }

        public void SetCache(string table, TextAsset asset)
        {
            TableCache entry = cache.Find(x => x.Table == table);
            if (entry == null)
            {
                entry = new TableCache { Table = table };
                cache.Add(entry);
            }

            entry.Asset = asset;
        }

        [Serializable]
        public sealed class TableCache
        {
            public string Table;
            public TextAsset Asset;
        }
    }
}
