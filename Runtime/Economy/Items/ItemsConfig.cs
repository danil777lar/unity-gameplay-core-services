using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Economy
{
    [CreateAssetMenu(fileName = "ItemsConfig", menuName = "Larje/Core/Economy/Items Config")]
    public sealed class ItemsConfig : ScriptableObject
    {
        [SerializeField] private bool saveOnChange = true;
        [SerializeField] private List<ItemCatalog> catalogs = new List<ItemCatalog>();

        public bool SaveOnChange => saveOnChange;
        public IReadOnlyList<ItemCatalog> Catalogs => catalogs;

        public ItemCatalog Find(ItemCategoryId category)
        {
            return catalogs.Find(x => x != null && x.Category == category);
        }
    }
}
