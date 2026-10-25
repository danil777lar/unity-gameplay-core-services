using System.IO;
using Larje.Core.Debugging;
using Larje.Core.Economy;
using Larje.Core.Events;
using Larje.Core.GameFlow;
using Larje.Core.Saves;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace Larje.Core.Zenject.Tests
{
    public sealed class ContainerBindingTests
    {
        private string _root;
        private DiContainer _container;
        private CurrencyId _coins;

        [SetUp]
        public void SetUp()
        {
            _root = Path.Combine(Path.GetTempPath(), "larje_zenject_tests_" + System.Guid.NewGuid().ToString("N"));
            _coins = ScriptableObject.CreateInstance<CurrencyId>();
            _coins.name = "Coins";

            _container = new DiContainer();
            _container.BindLarjeInfrastructure();
            _container.BindLarjeSaves(ScriptableObject.CreateInstance<SaveConfig>(), new FileSaveStorage(_root));
            _container.BindLarjeGameFlow(ScriptableObject.CreateInstance<GameStatesConfig>(), ScriptableObject.CreateInstance<SceneLoaderConfig>(), null);
            _container.BindLarjeEconomy(ScriptableObject.CreateInstance<CurrencyConfig>(), null);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_root))
            {
                Directory.Delete(_root, true);
            }

            Object.DestroyImmediate(_coins);
        }

        [Test]
        public void ResolvesBoundServicesAsSingletons()
        {
            Assert.IsInstanceOf<SaveService>(_container.Resolve<ISaveService>());
            Assert.AreSame(_container.Resolve<ISaveService>(), _container.Resolve<SaveService>());
            Assert.IsInstanceOf<CurrencyService>(_container.Resolve<ICurrencyService>());
            Assert.IsInstanceOf<EventBus>(_container.Resolve<IEventBus>());
        }

        [Test]
        public void InitializerRunsServicesAndCurrencyPersists()
        {
            _container.Resolve<LarjeInitializer>().Initialize();
            ICurrencyService currency = _container.Resolve<ICurrencyService>();
            currency.Add(new CurrencyOperation(_coins, 25));

            Assert.AreEqual(25, currency.Get(_coins));
            Assert.IsTrue(File.Exists(Path.Combine(_root, "slot_default.json")));
        }

        [Test]
        public void DebugProvidersAreRegistered()
        {
            _container.Resolve<DebugCommandsRegistrar>().Initialize();
            IDebugCommandRegistry registry = _container.Resolve<IDebugCommandRegistry>();

            Assert.IsNotNull(registry.Find("AddCurrency"));
            Assert.IsNotNull(registry.Find("Save"));
            Assert.IsNotNull(registry.Find("SetState"));
        }
    }
}
