using System;
using System.Collections.Generic;
using Larje.Core.Saves;

namespace Larje.Core.Monetization
{
    public abstract class PurchasesServiceBase : IPurchasesService
    {
        protected readonly PurchasesConfig _config;
        protected readonly ISaveService _saves;
        protected readonly PurchaseContext _context;

        public abstract bool Initialized { get; }
        public PurchasesConfig Config => _config;

        public event Action<ProductConfig> Purchased;
        public event Action<ProductConfig> PurchaseFailed;

        protected PurchasesSaveData Data => _saves.Get<PurchasesSaveData>();

        protected PurchasesServiceBase(PurchasesConfig config, ISaveService saves, IServiceProvider services)
        {
            _config = config;
            _saves = saves;
            _context = new PurchaseContext(saves, services);
        }

        public abstract bool IsPurchased(ProductConfig product);
        public abstract ProductInfo GetInfo(ProductConfig product);
        public abstract void Buy(ProductConfig product);
        public abstract void Restore();

        public List<ProductConfig> GetByGroup(ProductGroupId group)
        {
            List<ProductConfig> result = new List<ProductConfig>();
            foreach (ProductConfig product in _config.Products)
            {
                if (product != null && product.Group == group)
                {
                    result.Add(product);
                }
            }

            return result;
        }

        public bool IsDiscountActive(ProductConfig product)
        {
            return product.HasDiscountProduct && Data.IsDiscountActive(product.Id);
        }

        public void ApplyDiscount(ProductConfig product, TimeSpan duration = default)
        {
            Data.ApplyDiscount(product.Id, duration);
            _saves.SaveGame();
        }

        public void RemoveDiscount(ProductConfig product)
        {
            Data.RemoveDiscount(product.Id);
            _saves.SaveGame();
        }

        protected string GetStoreKey(ProductConfig product)
        {
            return IsDiscountActive(product) ? product.DiscountProductKey : product.ProductKey;
        }

        protected void CompletePurchase(ProductConfig product)
        {
            product.OnPurchased(_context);
            Purchased?.Invoke(product);
            _saves.SaveGame();
        }

        protected void FailPurchase(ProductConfig product)
        {
            product.OnPurchaseFailed(_context);
            PurchaseFailed?.Invoke(product);
        }
    }
}
