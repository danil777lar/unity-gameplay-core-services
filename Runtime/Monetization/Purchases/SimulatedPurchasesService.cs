using System;
using Larje.Core.Saves;

namespace Larje.Core.Monetization
{
    public sealed class SimulatedPurchasesService : PurchasesServiceBase
    {
        public override bool Initialized => true;

        public SimulatedPurchasesService(PurchasesConfig config, ISaveService saves, IServiceProvider services = null) : base(config, saves, services)
        {
        }

        public override bool IsPurchased(ProductConfig product)
        {
            return Data.SimulatedPurchases.Contains(product.Id);
        }

        public override ProductInfo GetInfo(ProductConfig product)
        {
            return new ProductInfo(true, "$0.99", product.name, string.Empty, "USD", 0.99m);
        }

        public override void Buy(ProductConfig product)
        {
            if (product.ProductType != ProductType.Consumable && !Data.SimulatedPurchases.Contains(product.Id))
            {
                Data.SimulatedPurchases.Add(product.Id);
            }

            CompletePurchase(product);
        }

        public override void Restore()
        {
            foreach (ProductConfig product in _config.Products)
            {
                if (product != null && IsPurchased(product))
                {
                    product.OnPurchased(_context);
                }
            }
        }
    }
}
