using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Economy
{
    [CreateAssetMenu(fileName = "CurrencyConfig", menuName = "Larje/Core/Economy/Currency Config")]
    public sealed class CurrencyConfig : ScriptableObject, IConfigValidatable
    {
        [Header("Placements")]
        [SerializeField] private CurrencyPlacementId defaultPlacement;
        [SerializeField] private List<CurrencyPlacementId> placements = new List<CurrencyPlacementId>();

        [Header("Currencies")]
        [SerializeField] private List<CurrencyOptions> currencies = new List<CurrencyOptions>();

        [Header("Saving")]
        [SerializeField] private bool saveOnChange = true;

        public CurrencyPlacementId DefaultPlacement => defaultPlacement;
        public IReadOnlyList<CurrencyPlacementId> Placements => placements;
        public IReadOnlyList<CurrencyOptions> Currencies => currencies;
        public bool SaveOnChange => saveOnChange;

        public CurrencyOptions Find(CurrencyId currency)
        {
            return currencies.Find(x => x.Currency == currency);
        }

        private void OnValidate()
        {
            currencies.ForEach(x => x.Validate());
        }

        [Serializable]
        public sealed class CurrencyOptions
        {
            [SerializeField, HideInInspector] private string inspectorName;
            [SerializeField] private CurrencyId currency;
            [SerializeField] private int defaultAmount;
            [SerializeField] private int maxAmount;
            [SerializeField] private Sprite icon;

            public CurrencyId Currency => currency;
            public int DefaultAmount => defaultAmount;
            public int MaxAmount => maxAmount;
            public Sprite Icon => icon;

            public void Validate()
            {
                inspectorName = currency != null ? currency.name : "None";
            }
        }

        public void Validate(List<string> problems)
        {
            if (defaultPlacement == null)
            {
                problems.Add("Default placement is not set");
            }

            foreach (CurrencyOptions options in currencies)
            {
                if (options.Currency == null)
                {
                    problems.Add("Currency entry has no CurrencyId");
                }
            }
        }
    }
}
