using System;
using System.Collections.Generic;
using Larje.Core.Saves;
using UnityEngine;

namespace Larje.Core.Economy
{
    public sealed class CurrencyService : ICurrencyService, IInitializableService, IDisposable
    {
        private readonly CurrencyConfig _config;
        private readonly ISaveService _saves;

        public CurrencyConfig Config => _config;

        public event Action Changed;
        public event Action<CurrencyOperation> Added;
        public event Action<CurrencyOperation> Spent;

        private CurrencySaveData Data => _saves.Get<CurrencySaveData>();

        public CurrencyService(CurrencyConfig config, ISaveService saves)
        {
            _config = config;
            _saves = saves;
        }

        public void Initialize()
        {
            _saves.Loaded += OnLoaded;
            ApplyDefaults();
        }

        public void Dispose()
        {
            _saves.Loaded -= OnLoaded;
        }

        public int Get(CurrencyId currency, CurrencyPlacementId placement = null)
        {
            if (currency == null || !Data.Amounts.TryGetValue(currency.Key, out Dictionary<string, int> placements))
            {
                return 0;
            }

            return placements.TryGetValue(ResolvePlacement(placement), out int amount) ? amount : 0;
        }

        public bool Has(CurrencyId currency, int amount, CurrencyPlacementId placement = null)
        {
            return Get(currency, placement) >= amount;
        }

        public void Add(CurrencyOperation operation)
        {
            Set(operation.Currency, Get(operation.Currency, operation.Placement) + operation.Amount, operation.Placement);
            Added?.Invoke(operation);
        }

        public bool TrySpend(CurrencyOperation operation)
        {
            int current = Get(operation.Currency, operation.Placement);
            if (current < operation.Amount)
            {
                return false;
            }

            Set(operation.Currency, current - operation.Amount, operation.Placement);
            Spent?.Invoke(operation);
            return true;
        }

        public void Set(CurrencyId currency, int amount, CurrencyPlacementId placement = null)
        {
            if (currency == null)
            {
                return;
            }

            CurrencyConfig.CurrencyOptions options = _config.Find(currency);
            int clamped = Mathf.Max(0, amount);
            if (options != null && options.MaxAmount > 0)
            {
                clamped = Mathf.Min(clamped, options.MaxAmount);
            }

            if (!Data.Amounts.TryGetValue(currency.Key, out Dictionary<string, int> placements))
            {
                placements = new Dictionary<string, int>();
                Data.Amounts[currency.Key] = placements;
            }

            placements[ResolvePlacement(placement)] = clamped;
            Changed?.Invoke();
            if (_config.SaveOnChange)
            {
                _saves.SaveGame();
            }
        }

        public void Move(CurrencyId currency, CurrencyPlacementId from, CurrencyPlacementId to)
        {
            int amount = Get(currency, from);
            Set(currency, 0, from);
            Add(new CurrencyOperation(currency, amount, to));
        }

        private string ResolvePlacement(CurrencyPlacementId placement)
        {
            CurrencyPlacementId resolved = placement != null ? placement : _config.DefaultPlacement;
            return resolved != null ? resolved.Key : "global";
        }

        private void ApplyDefaults()
        {
            bool applied = false;
            foreach (CurrencyConfig.CurrencyOptions options in _config.Currencies)
            {
                if (options.Currency != null && !Data.DefaultsApplied.Contains(options.Currency.Key))
                {
                    Data.DefaultsApplied.Add(options.Currency.Key);
                    if (options.DefaultAmount > 0)
                    {
                        Data.Amounts[options.Currency.Key] = new Dictionary<string, int>
                        {
                            { ResolvePlacement(null), options.DefaultAmount }
                        };
                    }

                    applied = true;
                }
            }

            if (applied)
            {
                Changed?.Invoke();
            }
        }

        private void OnLoaded()
        {
            ApplyDefaults();
            Changed?.Invoke();
        }
    }
}
