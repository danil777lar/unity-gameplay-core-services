using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Saves
{
    public sealed class PlayerPrefsSaveStorage : ISaveStorage
    {
        private const string PREFIX = "larje_save_";
        private const string INDEX_KEY = PREFIX + "index";

        public bool TryRead(string file, out string content)
        {
            string key = PREFIX + file;
            content = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : null;
            return content != null;
        }

        public void Write(string file, string content)
        {
            PlayerPrefs.SetString(PREFIX + file, content);
            List<string> index = ReadIndex();
            if (!index.Contains(file))
            {
                index.Add(file);
                WriteIndex(index);
            }

            PlayerPrefs.Save();
        }

        public void Delete(string file)
        {
            PlayerPrefs.DeleteKey(PREFIX + file);
            List<string> index = ReadIndex();
            if (index.Remove(file))
            {
                WriteIndex(index);
            }

            PlayerPrefs.Save();
        }

        public void DeleteAll()
        {
            foreach (string file in ReadIndex())
            {
                PlayerPrefs.DeleteKey(PREFIX + file);
            }

            PlayerPrefs.DeleteKey(INDEX_KEY);
            PlayerPrefs.Save();
        }

        public IReadOnlyList<string> GetFiles()
        {
            return ReadIndex();
        }

        private List<string> ReadIndex()
        {
            string raw = PlayerPrefs.GetString(INDEX_KEY, string.Empty);
            return new List<string>(raw.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private void WriteIndex(List<string> index)
        {
            PlayerPrefs.SetString(INDEX_KEY, string.Join("\n", index));
        }
    }
}
