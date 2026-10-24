using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

namespace Larje.Core.Saves
{
    public sealed class SaveService : ISaveService, IInitializableService, IDisposable
    {
        private const string SYSTEM_FILE = "system";
        private const string SLOT_PREFIX = "slot_";

        private string _currentSlot;
        private readonly SaveConfig _config;
        private readonly ISaveStorage _storage;
        private readonly SaveContainer _game = new SaveContainer();
        private readonly SaveContainer _system = new SaveContainer();
        private readonly List<ISaveMetaProvider> _metaProviders = new List<ISaveMetaProvider>();
        private readonly Dictionary<Type, SaveSectionAttribute> _sections = new Dictionary<Type, SaveSectionAttribute>();

        public string CurrentSlot => _currentSlot;

        public event Action Saving;
        public event Action Loaded;

        public SaveService(SaveConfig config, ISaveStorage storage)
        {
            _config = config;
            _storage = storage;
            _currentSlot = config.DefaultSlot;
        }

        public void Initialize()
        {
            Reload(_config.LoadGameOnInitialize);
            Application.quitting += OnQuitting;
            Application.focusChanged += OnFocusChanged;
        }

        public void Reload(bool loadGame = true)
        {
            _system.Clear();
            if (_storage.TryRead(SYSTEM_FILE, out string systemJson))
            {
                _system.Deserialize(Decode(systemJson));
            }

            if (loadGame && !LoadGame(_currentSlot))
            {
                NewGame(_currentSlot);
            }
        }

        public void Dispose()
        {
            Application.quitting -= OnQuitting;
            Application.focusChanged -= OnFocusChanged;
        }

        public T Get<T>() where T : class, new()
        {
            SaveSectionAttribute section = GetSection(typeof(T));
            return GetContainer(section.Scope).Get<T>(section.Key);
        }

        public void SaveGame(string slot = null)
        {
            if (!string.IsNullOrEmpty(slot))
            {
                _currentSlot = slot;
            }

            Saving?.Invoke();
            _storage.Write(SLOT_PREFIX + _currentSlot, Encode(_game.Serialize(CreateMeta())));
            SaveSystem();
        }

        public bool LoadGame(string slot = null)
        {
            string target = string.IsNullOrEmpty(slot) ? _currentSlot : slot;
            if (!_storage.TryRead(SLOT_PREFIX + target, out string json) || !_game.Deserialize(Decode(json)))
            {
                return false;
            }

            _currentSlot = target;
            Loaded?.Invoke();
            return true;
        }

        public void NewGame(string slot = null)
        {
            if (!string.IsNullOrEmpty(slot))
            {
                _currentSlot = slot;
            }

            _game.Clear();
            Loaded?.Invoke();
        }

        public void SaveSystem()
        {
            _storage.Write(SYSTEM_FILE, Encode(_system.Serialize(null)));
        }

        public void DeleteSlot(string slot)
        {
            _storage.Delete(SLOT_PREFIX + slot);
        }

        public void DeleteAll()
        {
            _storage.DeleteAll();
            _system.Clear();
            NewGame(_config.DefaultSlot);
        }

        public IReadOnlyList<SaveMeta> GetSaves()
        {
            List<SaveMeta> saves = new List<SaveMeta>();
            foreach (string file in _storage.GetFiles())
            {
                if (file.StartsWith(SLOT_PREFIX) && _storage.TryRead(file, out string json))
                {
                    SaveMeta meta = SaveContainer.ReadMeta(Decode(json));
                    if (meta != null)
                    {
                        saves.Add(meta);
                    }
                }
            }

            return saves;
        }

        public void AddMetaProvider(ISaveMetaProvider provider)
        {
            if (!_metaProviders.Contains(provider))
            {
                _metaProviders.Add(provider);
            }
        }

        public void RemoveMetaProvider(ISaveMetaProvider provider)
        {
            _metaProviders.Remove(provider);
        }

        public IReadOnlyList<string> GetSectionKeys(SaveScope scope)
        {
            return new List<string>(GetContainer(scope).Keys);
        }

        public string ExportSection(SaveScope scope, string key)
        {
            return GetContainer(scope).Export(key);
        }

        public bool ImportSection(SaveScope scope, string key, string json)
        {
            bool imported = GetContainer(scope).Import(key, json);
            if (imported && scope == SaveScope.Game)
            {
                Loaded?.Invoke();
            }

            return imported;
        }

        private SaveContainer GetContainer(SaveScope scope)
        {
            return scope == SaveScope.System ? _system : _game;
        }

        private SaveSectionAttribute GetSection(Type type)
        {
            if (!_sections.TryGetValue(type, out SaveSectionAttribute section))
            {
                section = type.GetCustomAttribute<SaveSectionAttribute>() ?? new SaveSectionAttribute(type.Name);
                _sections[type] = section;
            }

            return section;
        }

        private SaveMeta CreateMeta()
        {
            SaveMeta meta = new SaveMeta
            {
                Slot = _currentSlot,
                Date = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                Version = _config.Version
            };

            foreach (ISaveMetaProvider provider in _metaProviders)
            {
                provider.WriteMeta(meta);
            }

            return meta;
        }

        private string Encode(string json)
        {
            return _config.Encrypt ? AesUtility.Encrypt(json, _config.AesKey, _config.AesIv) : json;
        }

        private string Decode(string content)
        {
            if (!_config.Encrypt || content.StartsWith("{"))
            {
                return content;
            }

            return AesUtility.Decrypt(content, _config.AesKey, _config.AesIv);
        }

        private void OnQuitting()
        {
            if (_config.SaveOnQuit)
            {
                SaveGame();
            }
        }

        private void OnFocusChanged(bool focused)
        {
            if (!focused && _config.SaveOnPause)
            {
                SaveGame();
            }
        }
    }
}
