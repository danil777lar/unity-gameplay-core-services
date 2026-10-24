using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using Larje.Core.Saves;

namespace Larje.Core.Monetization
{
    public sealed class DiscountQueue : IInitializableService, IDisposable
    {
        private float _sinceCheck;
        private readonly DiscountQueueConfig _config;
        private readonly IPurchasesService _purchases;
        private readonly ISaveService _saves;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();

        public DiscountQueue(DiscountQueueConfig config, IPurchasesService purchases, ISaveService saves)
        {
            _config = config;
            _purchases = purchases;
            _saves = saves;
        }

        public void Initialize()
        {
            Advance();
            ServiceLoop.Run(Tick, _lifetime.Token, PlayerLoopTiming.Update, true);
        }

        public void Dispose()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        private void Tick(float deltaTime)
        {
            _sinceCheck += deltaTime;
            if (_sinceCheck >= _config.CheckInterval)
            {
                _sinceCheck = 0f;
                Advance();
            }
        }

        private void Advance()
        {
            List<ProductConfig> pending = new List<ProductConfig>();
            foreach (ProductConfig product in _config.Queue)
            {
                if (product != null && !_purchases.IsPurchased(product))
                {
                    pending.Add(product);
                }
            }

            if (pending.Count == 0 || pending.Exists(x => _purchases.IsDiscountActive(x)))
            {
                return;
            }

            int next = (FindLatestApplied(pending) + 1) % pending.Count;
            _purchases.ApplyDiscount(pending[next], TimeSpan.FromMinutes(_config.DiscountDurationMinutes));
        }

        private int FindLatestApplied(List<ProductConfig> pending)
        {
            PurchasesSaveData data = _saves.Get<PurchasesSaveData>();
            int latest = -1;
            DateTime latestTime = DateTime.MinValue;
            for (int i = 0; i < pending.Count; i++)
            {
                PurchasesSaveData.DiscountEntry entry = data.Discounts.Find(x => x.ProductId == pending[i].Id);
                if (entry != null && DateTime.TryParse(entry.AppliedAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime applied) && applied > latestTime)
                {
                    latestTime = applied;
                    latest = i;
                }
            }

            return latest;
        }
    }
}
