using System;
using System.Collections.Generic;
using Larje.Core.Saves;

namespace Larje.Core.Economy
{
    public sealed class ItemsService : IItemsService
    {
        private static readonly IReadOnlyList<ItemDefinition> s_empty = new ItemDefinition[0];

        private readonly ItemsConfig _config;
        private readonly ISaveService _saves;

        public ItemsConfig Config => _config;

        public event Action<ItemCategoryId> CurrentChanged;
        public event Action<ItemCategoryId, string> Unlocked;

        public ItemsService(ItemsConfig config, ISaveService saves)
        {
            _config = config;
            _saves = saves;
        }

        public void Unlock(ItemCategoryId category, string itemKey)
        {
            ItemsSaveData.CategoryData data = GetData(category);
            if (data == null || data.Unlocked.Contains(itemKey))
            {
                return;
            }

            data.Unlocked.Add(itemKey);
            Unlocked?.Invoke(category, itemKey);
            Save();
        }

        public bool IsUnlocked(ItemCategoryId category, string itemKey)
        {
            ItemsSaveData.CategoryData data = GetData(category);
            return data != null && data.Unlocked.Contains(itemKey);
        }

        public bool SetCurrent(ItemCategoryId category, string itemKey)
        {
            ItemsSaveData.CategoryData data = GetData(category);
            bool clearing = string.IsNullOrWhiteSpace(itemKey);
            if (data == null || data.Current == itemKey || (!clearing && !data.Unlocked.Contains(itemKey)))
            {
                return false;
            }

            data.Current = clearing ? null : itemKey;
            CurrentChanged?.Invoke(category);
            Save();
            return true;
        }

        public bool TryGetCurrent(ItemCategoryId category, out ItemDefinition item)
        {
            ItemCatalog catalog = _config.Find(category);
            ItemsSaveData.CategoryData data = GetData(category);
            item = catalog != null && data != null ? catalog.Find(data.Current) : null;
            return item != null;
        }

        public IReadOnlyList<ItemDefinition> GetAll(ItemCategoryId category)
        {
            ItemCatalog catalog = _config.Find(category);
            return catalog != null ? catalog.Items : s_empty;
        }

        public List<ItemDefinition> GetLocked(ItemCategoryId category)
        {
            List<ItemDefinition> result = new List<ItemDefinition>();
            foreach (ItemDefinition item in GetAll(category))
            {
                if (!IsUnlocked(category, item.Key))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public List<ItemDefinition> GetUnlocked(ItemCategoryId category)
        {
            List<ItemDefinition> result = new List<ItemDefinition>();
            foreach (ItemDefinition item in GetAll(category))
            {
                if (IsUnlocked(category, item.Key))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private ItemsSaveData.CategoryData GetData(ItemCategoryId category)
        {
            ItemCatalog catalog = _config.Find(category);
            if (catalog == null)
            {
                return null;
            }

            ItemsSaveData save = _saves.Get<ItemsSaveData>();
            if (!save.Categories.TryGetValue(category.Key, out ItemsSaveData.CategoryData data))
            {
                data = new ItemsSaveData.CategoryData();
                if (catalog.FirstItemIsDefault && catalog.Items.Count > 0)
                {
                    data.Current = catalog.Items[0].Key;
                    data.Unlocked.Add(data.Current);
                }

                save.Categories[category.Key] = data;
            }

            return data;
        }

        private void Save()
        {
            if (_config.SaveOnChange)
            {
                _saves.SaveGame();
            }
        }
    }
}
