using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Economy
{
    [CreateAssetMenu(fileName = "ItemCatalog", menuName = "Larje/Core/Economy/Simple Item Catalog")]
    public sealed class SimpleItemCatalog : ItemCatalog
    {
        [SerializeField] private List<ItemDefinition> items = new List<ItemDefinition>();

        public override IReadOnlyList<ItemDefinition> Items => items;
    }
}
