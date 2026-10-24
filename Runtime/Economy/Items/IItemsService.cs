using System;
using System.Collections.Generic;

namespace Larje.Core.Economy
{
    public interface IItemsService
    {
        ItemsConfig Config { get; }

        event Action<ItemCategoryId> CurrentChanged;
        event Action<ItemCategoryId, string> Unlocked;

        void Unlock(ItemCategoryId category, string itemKey);
        bool IsUnlocked(ItemCategoryId category, string itemKey);
        bool SetCurrent(ItemCategoryId category, string itemKey);
        bool TryGetCurrent(ItemCategoryId category, out ItemDefinition item);
        IReadOnlyList<ItemDefinition> GetAll(ItemCategoryId category);
        List<ItemDefinition> GetLocked(ItemCategoryId category);
        List<ItemDefinition> GetUnlocked(ItemCategoryId category);
    }
}
