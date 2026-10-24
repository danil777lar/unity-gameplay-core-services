using System;
using System.Collections.Generic;
using Larje.Core.Saves.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Larje.Core.Saves
{
    internal sealed class SaveContainer
    {
        private const string META_KEY = "meta";
        private const string SECTIONS_KEY = "sections";

        private readonly Dictionary<string, JToken> _raw = new Dictionary<string, JToken>();
        private readonly Dictionary<string, object> _live = new Dictionary<string, object>();

        public IEnumerable<string> Keys
        {
            get
            {
                HashSet<string> keys = new HashSet<string>(_raw.Keys);
                keys.UnionWith(_live.Keys);
                return keys;
            }
        }

        public T Get<T>(string key) where T : class, new()
        {
            if (_live.TryGetValue(key, out object live) && live is T typed)
            {
                return typed;
            }

            T section = null;
            if (_raw.TryGetValue(key, out JToken token))
            {
                try
                {
                    section = token.ToObject<T>(SaveJson.Serializer);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"SaveContainer: failed to read section '{key}': {exception.Message}");
                }
            }

            section ??= new T();
            _live[key] = section;
            return section;
        }

        public string Export(string key)
        {
            Flush();
            return _raw.TryGetValue(key, out JToken token) ? token.ToString(Formatting.Indented) : null;
        }

        public bool Import(string key, string json)
        {
            try
            {
                _raw[key] = JToken.Parse(json);
                _live.Remove(key);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"SaveContainer: failed to import section '{key}': {exception.Message}");
                return false;
            }
        }

        public void Clear()
        {
            _raw.Clear();
            _live.Clear();
        }

        public string Serialize(SaveMeta meta)
        {
            Flush();
            JObject sections = new JObject();
            foreach (KeyValuePair<string, JToken> pair in _raw)
            {
                sections[pair.Key] = pair.Value;
            }

            JObject root = new JObject();
            if (meta != null)
            {
                root[META_KEY] = JToken.FromObject(meta, SaveJson.Serializer);
            }

            root[SECTIONS_KEY] = sections;
            return root.ToString(Formatting.None);
        }

        public bool Deserialize(string json)
        {
            try
            {
                JObject root = JObject.Parse(json);
                Clear();
                if (root[SECTIONS_KEY] is JObject sections)
                {
                    foreach (KeyValuePair<string, JToken> pair in sections)
                    {
                        _raw[pair.Key] = pair.Value;
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"SaveContainer: failed to parse save: {exception.Message}");
                return false;
            }
        }

        public static SaveMeta ReadMeta(string json)
        {
            try
            {
                JObject root = JObject.Parse(json);
                return root[META_KEY]?.ToObject<SaveMeta>(SaveJson.Serializer);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void Flush()
        {
            foreach (KeyValuePair<string, object> pair in _live)
            {
                _raw[pair.Key] = JToken.FromObject(pair.Value, SaveJson.Serializer);
            }
        }
    }
}
