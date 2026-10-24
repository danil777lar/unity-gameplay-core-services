using System;
using System.Collections.Generic;

namespace Larje.Core.Saves
{
    public interface ISaveService
    {
        string CurrentSlot { get; }

        event Action Saving;
        event Action Loaded;

        T Get<T>() where T : class, new();
        void SaveGame(string slot = null);
        bool LoadGame(string slot = null);
        void NewGame(string slot = null);
        void SaveSystem();
        void DeleteSlot(string slot);
        void DeleteAll();
        IReadOnlyList<SaveMeta> GetSaves();
        void AddMetaProvider(ISaveMetaProvider provider);
        void RemoveMetaProvider(ISaveMetaProvider provider);
        IReadOnlyList<string> GetSectionKeys(SaveScope scope);
        string ExportSection(SaveScope scope, string key);
        bool ImportSection(SaveScope scope, string key, string json);
    }
}
