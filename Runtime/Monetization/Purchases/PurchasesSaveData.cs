using System;
using System.Collections.Generic;
using System.Globalization;
using Larje.Core.Saves;

namespace Larje.Core.Monetization
{
    [Serializable]
    [SaveSection("purchases")]
    public sealed class PurchasesSaveData
    {
        public List<string> SimulatedPurchases = new List<string>();
        public List<DiscountEntry> Discounts = new List<DiscountEntry>();

        public bool IsDiscountActive(string productId)
        {
            DiscountEntry entry = Discounts.Find(x => x.ProductId == productId);
            return entry != null && !entry.IsExpired;
        }

        public bool HasDiscountEntry(string productId)
        {
            return Discounts.Exists(x => x.ProductId == productId);
        }

        public void ApplyDiscount(string productId, TimeSpan duration)
        {
            Discounts.RemoveAll(x => x.ProductId == productId);
            Discounts.Add(new DiscountEntry(productId, DateTime.UtcNow, duration));
        }

        public void RemoveDiscount(string productId)
        {
            Discounts.RemoveAll(x => x.ProductId == productId);
        }

        [Serializable]
        public sealed class DiscountEntry
        {
            public string ProductId;
            public string AppliedAt;
            public long DurationTicks;

            public bool IsTimeless => DurationTicks <= 0;

            public DateTime ExpiresAt
            {
                get
                {
                    if (IsTimeless)
                    {
                        return DateTime.MaxValue;
                    }

                    DateTime applied = DateTime.Parse(AppliedAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                    return applied + TimeSpan.FromTicks(DurationTicks);
                }
            }

            public bool IsExpired => !IsTimeless && DateTime.UtcNow > ExpiresAt;

            public DiscountEntry()
            {
            }

            public DiscountEntry(string productId, DateTime appliedAt, TimeSpan duration)
            {
                ProductId = productId;
                AppliedAt = appliedAt.ToString("o", CultureInfo.InvariantCulture);
                DurationTicks = duration.Ticks;
            }
        }
    }
}
