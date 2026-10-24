using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Larje.Core.Saves;

namespace Larje.Core.Monetization
{
    public abstract class AdsServiceBase : IAdsService
    {
        protected readonly ISaveService _saves;

        public abstract bool Initialized { get; }
        public abstract bool InterstitialAvailable { get; }
        public abstract bool RewardedAvailable { get; }
        public abstract bool BannerShowing { get; }
        public abstract float BannerHeight { get; }
        public bool NoAds => _saves.Get<AdsSaveData>().NoAds;

        public event Action BannerShown;
        public event Action BannerHidden;
        public event Action<bool> NoAdsChanged;

        protected AdsServiceBase(ISaveService saves)
        {
            _saves = saves;
        }

        public void ShowInterstitial(string placement = null)
        {
            if (!NoAds)
            {
                ShowInterstitialInternal(placement);
            }
        }

        public UniTask<bool> ShowRewardedAsync(string placement = null, CancellationToken cancellationToken = default)
        {
            return ShowRewardedInternal(placement, cancellationToken);
        }

        public bool ShowAppOpen()
        {
            return !NoAds && ShowAppOpenInternal();
        }

        public void SetNoAds(bool active)
        {
            if (NoAds == active)
            {
                return;
            }

            _saves.Get<AdsSaveData>().NoAds = active;
            _saves.SaveGame();
            if (active)
            {
                HideBanner();
            }

            NoAdsChanged?.Invoke(active);
        }

        protected abstract void ShowInterstitialInternal(string placement);
        protected abstract UniTask<bool> ShowRewardedInternal(string placement, CancellationToken cancellationToken);
        protected abstract bool ShowAppOpenInternal();
        protected abstract void HideBanner();

        protected void NotifyBannerShown()
        {
            BannerShown?.Invoke();
        }

        protected void NotifyBannerHidden()
        {
            BannerHidden?.Invoke();
        }
    }
}
