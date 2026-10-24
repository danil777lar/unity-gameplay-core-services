using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Larje.Core.Monetization;
using Larje.Core.Saves;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace Larje.Core.Zenject.Tests
{
    public sealed class MonetizationProviderTests
    {
        private string _root;
        private DiContainer _container;

        [SetUp]
        public void SetUp()
        {
            _root = Path.Combine(Path.GetTempPath(), "larje_monetization_tests_" + System.Guid.NewGuid().ToString("N"));
            _container = new DiContainer();
            _container.BindLarjeInfrastructure();
            _container.BindLarjeSaves(ScriptableObject.CreateInstance<SaveConfig>(), new FileSaveStorage(_root));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_root))
            {
                Directory.Delete(_root, true);
            }
        }

        [Test]
        public void ProviderReplacesSimulatedServicesBoundBefore()
        {
            _container.BindLarjeSimulatedMonetization(null);
            _container.BindLarjeAdsProvider<FakeAdsService>();
            _container.BindLarjeAnalyticsProvider<FakeAnalyticsService>();

            Assert.IsInstanceOf<FakeAdsService>(_container.Resolve<IAdsService>());
            Assert.IsInstanceOf<FakeAnalyticsService>(_container.Resolve<IAnalyticsService>());
        }

        [Test]
        public void SimulatedServicesSkipContractsBoundByProvider()
        {
            _container.BindLarjeAdsProvider<FakeAdsService>();
            _container.BindLarjeAnalyticsProvider<FakeAnalyticsService>();
            _container.BindLarjeSimulatedMonetization(null);

            Assert.IsInstanceOf<FakeAdsService>(_container.Resolve<IAdsService>());
            Assert.IsInstanceOf<FakeAnalyticsService>(_container.Resolve<IAnalyticsService>());
        }

        [Test]
        public void SimulatedServicesAreUsedWithoutProvider()
        {
            _container.BindLarjeSimulatedMonetization(null);

            Assert.IsInstanceOf<SimulatedAdsService>(_container.Resolve<IAdsService>());
            Assert.IsInstanceOf<LogAnalyticsService>(_container.Resolve<IAnalyticsService>());
        }

        private sealed class FakeAdsService : AdsServiceBase
        {
            public override bool Initialized => true;
            public override bool InterstitialAvailable => false;
            public override bool RewardedAvailable => false;
            public override bool BannerShowing => false;
            public override float BannerHeight => 0f;

            public FakeAdsService(ISaveService saves) : base(saves)
            {
            }

            protected override void ShowInterstitialInternal(string placement)
            {
            }

            protected override UniTask<bool> ShowRewardedInternal(string placement, CancellationToken cancellationToken)
            {
                return UniTask.FromResult(false);
            }

            protected override bool ShowAppOpenInternal()
            {
                return false;
            }

            protected override void HideBanner()
            {
            }
        }

        private sealed class FakeAnalyticsService : IAnalyticsService
        {
            public void Send(string eventName)
            {
            }

            public void Send(string eventName, IReadOnlyDictionary<string, object> parameters)
            {
            }
        }
    }
}
