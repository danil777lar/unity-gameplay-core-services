using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Larje.Core.Saves
{
    public sealed class FileSaveStorage : ISaveStorage
    {
        private const string EXTENSION = ".json";

        private readonly string _root;

        public string Root => _root;

        public FileSaveStorage() : this(Path.Combine(Application.persistentDataPath, "Saves"))
        {
        }

        public FileSaveStorage(string root)
        {
            _root = root;
        }

        public bool TryRead(string file, out string content)
        {
            string path = GetPath(file);
            content = null;
            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                content = File.ReadAllText(path);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"FileSaveStorage: failed to read {path}: {exception.Message}");
                return false;
            }
        }

        public void Write(string file, string content)
        {
            Directory.CreateDirectory(_root);
            string path = GetPath(file);
            string temp = path + ".tmp";
            File.WriteAllText(temp, content);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.Move(temp, path);
        }

        public void Delete(string file)
        {
            string path = GetPath(file);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void DeleteAll()
        {
            if (Directory.Exists(_root))
            {
                Directory.Delete(_root, true);
            }
        }

        public IReadOnlyList<string> GetFiles()
        {
            List<string> files = new List<string>();
            if (!Directory.Exists(_root))
            {
                return files;
            }

            foreach (string path in Directory.GetFiles(_root, "*" + EXTENSION))
            {
                files.Add(Path.GetFileNameWithoutExtension(path));
            }

            return files;
        }

        private string GetPath(string file)
        {
            return Path.Combine(_root, file + EXTENSION);
        }
    }
}
