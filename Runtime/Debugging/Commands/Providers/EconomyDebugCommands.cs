using Larje.Core.Economy;

namespace Larje.Core.Debugging
{
    public sealed class EconomyDebugCommands : IDebugCommandProvider
    {
        private readonly ICurrencyService _currency;

        public EconomyDebugCommands(ICurrencyService currency)
        {
            _currency = currency;
        }

        [DebugCommand("Economy", "Add currency")]
        public string AddCurrency(CurrencyId currency, int amount, CurrencyPlacementId placement = null)
        {
            _currency.Add(new CurrencyOperation(currency, amount, placement));
            return $"{currency.Key} = {_currency.Get(currency, placement)}";
        }

        [DebugCommand("Economy", "Spend currency")]
        public string SpendCurrency(CurrencyId currency, int amount, CurrencyPlacementId placement = null)
        {
            bool spent = _currency.TrySpend(new CurrencyOperation(currency, amount, placement));
            return spent ? $"{currency.Key} = {_currency.Get(currency, placement)}" : "Not enough";
        }

        [DebugCommand("Economy", "Set currency amount")]
        public string SetCurrency(CurrencyId currency, int amount, CurrencyPlacementId placement = null)
        {
            _currency.Set(currency, amount, placement);
            return $"{currency.Key} = {_currency.Get(currency, placement)}";
        }

        [DebugCommand("Economy", "Print currency amount")]
        public int GetCurrency(CurrencyId currency, CurrencyPlacementId placement = null)
        {
            return _currency.Get(currency, placement);
        }
    }
}
