using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Economy
{
    public abstract class ItemCatalog : ScriptableObject
    {
        [Header("Catalog")]
        [SerializeField] private ItemCategoryId category;
        [SerializeField] private bool firstItemIsDefault = true;

        public ItemCategoryId Category => category;
        public bool FirstItemIsDefault => firstItemIsDefault;
        public abstract IReadOnlyList<ItemDefinition> Items { get; }

        public ItemDefinition Find(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            foreach (ItemDefinition item in Items)
            {
                if (item.Key == key)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
