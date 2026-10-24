using System;
using System.Collections.Generic;

namespace Larje.Core.Monetization
{
    public interface IPurchasesService
    {
        bool Initialized { get; }
        PurchasesConfig Config { get; }

        event Action<ProductConfig> Purchased;
        event Action<ProductConfig> PurchaseFailed;

        bool IsPurchased(ProductConfig product);
        ProductInfo GetInfo(ProductConfig product);
        List<ProductConfig> GetByGroup(ProductGroupId group);
        void Buy(ProductConfig product);
        void Restore();

        bool IsDiscountActive(ProductConfig product);
        void ApplyDiscount(ProductConfig product, TimeSpan duration = default);
        void RemoveDiscount(ProductConfig product);
    }
}
