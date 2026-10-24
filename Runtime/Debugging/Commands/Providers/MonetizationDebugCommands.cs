using Cysharp.Threading.Tasks;
using Larje.Core.Monetization;
using UnityEngine;

namespace Larje.Core.Debugging
{
    public sealed class MonetizationDebugCommands : IDebugCommandProvider
    {
        private readonly IAdsService _ads;

        public MonetizationDebugCommands(IAdsService ads)
        {
            _ads = ads;
        }

        [DebugCommand("Ads", "Print ads status")]
        public string Status()
        {
            return $"initialized={_ads.Initialized} interstitial={_ads.InterstitialAvailable} rewarded={_ads.RewardedAvailable} banner={_ads.BannerShowing} height={_ads.BannerHeight} noAds={_ads.NoAds}";
        }

        [DebugCommand("Ads", "Show interstitial")]
        public string ShowInterstitial(string placement = "")
        {
            _ads.ShowInterstitial(placement);
            return "Requested";
        }

        [DebugCommand("Ads", "Show rewarded and log result")]
        public string ShowRewarded(string placement = "")
        {
            ShowRewardedAsync(placement).Forget();
            return "Requested";
        }

        [DebugCommand("Ads", "Toggle no-ads mode")]
        public string SetNoAds(bool active)
        {
            _ads.SetNoAds(active);
            return "NoAds " + active;
        }

        private async UniTaskVoid ShowRewardedAsync(string placement)
        {
            bool rewarded = await _ads.ShowRewardedAsync(placement);
            Debug.Log("[Ads] Rewarded result: " + rewarded);
        }
    }
}
