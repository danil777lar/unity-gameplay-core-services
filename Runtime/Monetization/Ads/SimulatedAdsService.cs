using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Larje.Core.Saves;
using UnityEngine;

namespace Larje.Core.Monetization
{
    public sealed class SimulatedAdsService : AdsServiceBase
    {
        private readonly bool _rewardedSucceeds;
        private readonly float _rewardedDuration;

        public override bool Initialized => true;
        public override bool InterstitialAvailable => true;
        public override bool RewardedAvailable => true;
        public override bool BannerShowing => false;
        public override float BannerHeight => 0f;

        public SimulatedAdsService(ISaveService saves, bool rewardedSucceeds = true, float rewardedDuration = 0.5f) : base(saves)
        {
            _rewardedSucceeds = rewardedSucceeds;
            _rewardedDuration = rewardedDuration;
        }

        protected override void ShowInterstitialInternal(string placement)
        {
            Debug.Log($"SimulatedAdsService: interstitial '{placement}'");
        }

        protected override async UniTask<bool> ShowRewardedInternal(string placement, CancellationToken cancellationToken)
        {
            Debug.Log($"SimulatedAdsService: rewarded '{placement}'");
            await UniTask.Delay(TimeSpan.FromSeconds(_rewardedDuration), DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update, cancellationToken);
            return _rewardedSucceeds;
        }

        protected override bool ShowAppOpenInternal()
        {
            return false;
        }

        protected override void HideBanner()
        {
        }
    }
}
