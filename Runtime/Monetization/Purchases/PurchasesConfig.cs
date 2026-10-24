using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Monetization
{
    [CreateAssetMenu(fileName = "PurchasesConfig", menuName = "Larje/Core/Monetization/Purchases Config")]
    public sealed class PurchasesConfig : ScriptableObject
    {
        [SerializeField] private List<ProductConfig> products = new List<ProductConfig>();

        public IReadOnlyList<ProductConfig> Products => products;

        public ProductConfig FindByKey(string productKey)
        {
            return products.Find(x => x != null && (x.ProductKey == productKey || x.DiscountProductKey == productKey));
        }
    }
}
