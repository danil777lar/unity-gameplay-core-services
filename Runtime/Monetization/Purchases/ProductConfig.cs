using UnityEngine;

namespace Larje.Core.Monetization
{
    public abstract class ProductConfig : ScriptableObject
    {
        [Header("Store")]
        [SerializeField] private string productKey;
        [SerializeField] private ProductType productType;
        [SerializeField] private ProductGroupId group;

        [Header("Discount")]
        [SerializeField] private string discountProductKey;
        [SerializeField] private int discountPercent;

        public string Id => name;
        public string ProductKey => productKey;
        public ProductType ProductType => productType;
        public ProductGroupId Group => group;
        public string DiscountProductKey => discountProductKey;
        public int DiscountPercent => discountPercent;
        public bool HasDiscountProduct => !string.IsNullOrEmpty(discountProductKey);

        public abstract void OnPurchased(PurchaseContext context);

        public virtual void OnPurchaseFailed(PurchaseContext context)
        {
        }
    }
}
