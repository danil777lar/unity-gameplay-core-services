using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Larje.Core.Monetization
{
    public interface IAdsService
    {
        bool Initialized { get; }
        bool InterstitialAvailable { get; }
        bool RewardedAvailable { get; }
        bool BannerShowing { get; }
        float BannerHeight { get; }
        bool NoAds { get; }

        event Action BannerShown;
        event Action BannerHidden;
        event Action<bool> NoAdsChanged;

        void ShowInterstitial(string placement = null);
        UniTask<bool> ShowRewardedAsync(string placement = null, CancellationToken cancellationToken = default);
        bool ShowAppOpen();
        void SetNoAds(bool active);
    }
}
