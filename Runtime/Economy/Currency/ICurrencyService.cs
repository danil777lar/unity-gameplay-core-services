using System;

namespace Larje.Core.Economy
{
    public interface ICurrencyService
    {
        CurrencyConfig Config { get; }

        event Action Changed;
        event Action<CurrencyOperation> Added;
        event Action<CurrencyOperation> Spent;

        int Get(CurrencyId currency, CurrencyPlacementId placement = null);
        bool Has(CurrencyId currency, int amount, CurrencyPlacementId placement = null);
        void Add(CurrencyOperation operation);
        bool TrySpend(CurrencyOperation operation);
        void Set(CurrencyId currency, int amount, CurrencyPlacementId placement = null);
        void Move(CurrencyId currency, CurrencyPlacementId from, CurrencyPlacementId to);
    }
}
