using UnityEngine;

namespace Larje.Core.Economy
{
    public struct CurrencyOperation
    {
        public CurrencyId Currency;
        public CurrencyPlacementId Placement;
        public int Amount;
        public bool UsePosition;
        public Vector3 WorldPosition;

        public CurrencyOperation(CurrencyId currency, int amount, CurrencyPlacementId placement = null)
        {
            Currency = currency;
            Placement = placement;
            Amount = amount;
            UsePosition = false;
            WorldPosition = Vector3.zero;
        }

        public CurrencyOperation At(Vector3 worldPosition)
        {
            UsePosition = true;
            WorldPosition = worldPosition;
            return this;
        }
    }
}
