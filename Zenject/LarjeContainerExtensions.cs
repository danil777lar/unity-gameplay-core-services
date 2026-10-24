using Larje.Core.Audio;
using Larje.Core.Debugging;
using Larje.Core.Economy;
using Larje.Core.Events;
using Larje.Core.GameFlow;
using Larje.Core.Haptics;
using Larje.Core.Input;
using Larje.Core.LevelObjects;
using Larje.Core.Levels;
using Larje.Core.Localization;
using Larje.Core.Locations;
using Larje.Core.Monetization;
using Larje.Core.Saves;
using Larje.Core.Tasks;
using Larje.Core.TimeScale;
using Larje.Core.UI;
using Zenject;

namespace Larje.Core.Zenject
{
    public static class LarjeContainerExtensions
    {
        public static void BindLarjeInfrastructure(this DiContainer container)
        {
            container.BindInterfacesAndSelfTo<LarjeInitializer>().AsSingle().NonLazy();
            container.Bind<IEventBus>().To<EventBus>().AsSingle();
            container.Bind<IObjectInjector>().To<ZenjectObjectInjector>().AsSingle();
            container.Bind<System.IServiceProvider>().To<ZenjectServiceProvider>().AsSingle();
            container.Bind<IDebugCommandRegistry>().To<DebugCommandRegistry>().AsSingle();
            container.BindInterfacesAndSelfTo<DebugLogBuffer>().AsSingle();
            container.BindInterfacesAndSelfTo<DebugCommandsRegistrar>().AsSingle().NonLazy();
            container.BindInterfacesTo<EventsDebugCommands>().AsSingle();
        }

        public static void BindLarjeSaves(this DiContainer container, SaveConfig config, ISaveStorage storage = null)
        {
            container.BindInstance(config);
            container.Bind<ISaveStorage>().FromInstance(storage ?? SaveStorageFactory.CreateDefault()).AsSingle();
            container.BindInterfacesAndSelfTo<SaveService>().AsSingle();
            container.BindInterfacesTo<SaveDebugCommands>().AsSingle();
        }

        public static void BindLarjeGameFlow(this DiContainer container, GameStatesConfig states, SceneLoaderConfig sceneLoader, AppFlowConfig appFlow)
        {
            container.BindInstance(states);
            container.BindInstance(sceneLoader);
            container.BindInterfacesAndSelfTo<GameStateService>().AsSingle();
            container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle();
            if (appFlow != null)
            {
                container.BindInstance(appFlow);
                container.BindInterfacesAndSelfTo<AppFlowService>().AsSingle();
            }

            container.BindInterfacesTo<GameFlowDebugCommands>().AsSingle();
        }

        public static void BindLarjeTimeScale(this DiContainer container, TimeScaleConfig config)
        {
            container.BindInstance(config);
            container.BindInterfacesAndSelfTo<TimeScaleService>().AsSingle();
            container.BindInterfacesTo<TimeDebugCommands>().AsSingle();
        }

        public static void BindLarjeInput(this DiContainer container, InputConfig config, PointerControlConfig pointerControl = null)
        {
            container.BindInstance(config);
            container.BindInterfacesAndSelfTo<InputService>().AsSingle();
            if (pointerControl != null)
            {
                container.BindInstance(pointerControl);
                container.BindInterfacesAndSelfTo<PointerControlService>().AsSingle();
            }
        }

        public static void BindLarjeAudio(this DiContainer container, SoundConfig config)
        {
            container.BindInstance(config);
            container.BindInterfacesAndSelfTo<SoundService>().AsSingle();
            container.BindInterfacesAndSelfTo<SystemVolumeService>().AsSingle();
            container.BindInterfacesAndSelfTo<HapticService>().AsSingle();
            container.BindInterfacesTo<AudioDebugCommands>().AsSingle();
        }

        public static void BindLarjeLocalization(this DiContainer container, GTableLocalizationConfig config)
        {
            container.BindInstance(config);
            container.BindInterfacesAndSelfTo<GTableLocalizationService>().AsSingle();
        }

        public static void BindLarjeEconomy(this DiContainer container, CurrencyConfig currency, ItemsConfig items)
        {
            if (currency != null)
            {
                container.BindInstance(currency);
                container.BindInterfacesAndSelfTo<CurrencyService>().AsSingle();
                container.BindInterfacesTo<EconomyDebugCommands>().AsSingle();
            }

            if (items != null)
            {
                container.BindInstance(items);
                container.BindInterfacesAndSelfTo<ItemsService>().AsSingle();
            }
        }

        public static void BindLarjeDailyTasks(this DiContainer container, DailyTasksConfig config)
        {
            container.BindInstance(config);
            container.Bind<TaskContext>().AsSingle().IfNotBound();
            container.BindInterfacesAndSelfTo<DailyTasksService>().AsSingle();
        }

        public static void BindLarjeSimulatedMonetization(this DiContainer container, PurchasesConfig purchases)
        {
            if (!container.HasBinding<IAdsService>())
            {
                container.BindInterfacesAndSelfTo<SimulatedAdsService>().AsSingle();
            }

            if (!container.HasBinding<IAnalyticsService>())
            {
                container.BindInterfacesAndSelfTo<LogAnalyticsService>().AsSingle();
            }

            if (purchases != null)
            {
                if (!container.HasBinding<PurchasesConfig>())
                {
                    container.BindInstance(purchases);
                }

                if (!container.HasBinding<IPurchasesService>())
                {
                    container.BindInterfacesAndSelfTo<SimulatedPurchasesService>().AsSingle();
                }
            }

            if (!container.HasBinding<MonetizationDebugCommands>())
            {
                container.BindInterfacesAndSelfTo<MonetizationDebugCommands>().AsSingle();
            }
        }

        public static void BindLarjeAdsProvider<TService>(this DiContainer container) where TService : IAdsService
        {
            container.Unbind<IAdsService>();
            container.BindInterfacesAndSelfTo<TService>().AsSingle();
            if (!container.HasBinding<MonetizationDebugCommands>())
            {
                container.BindInterfacesAndSelfTo<MonetizationDebugCommands>().AsSingle();
            }
        }

        public static void BindLarjeAnalyticsProvider<TService>(this DiContainer container) where TService : IAnalyticsService
        {
            container.Unbind<IAnalyticsService>();
            container.BindInterfacesAndSelfTo<TService>().AsSingle();
        }

        public static void BindLarjePurchasesProvider<TService>(this DiContainer container, PurchasesConfig purchases) where TService : IPurchasesService
        {
            container.Unbind<IPurchasesService>();
            if (purchases != null && !container.HasBinding<PurchasesConfig>())
            {
                container.BindInstance(purchases);
            }

            container.BindInterfacesAndSelfTo<TService>().AsSingle();
        }

        public static void BindLarjeDiscountQueue(this DiContainer container, DiscountQueueConfig config)
        {
            container.BindInstance(config);
            container.BindInterfacesAndSelfTo<DiscountQueue>().AsSingle();
        }

        public static void BindLarjeLevels(this DiContainer container, LevelsConfig config, PrefabLevelsConfig levels)
        {
            container.BindInstance(levels);
            container.Bind<ILevelSource>().To<PrefabLevelSource>().AsSingle();
            BindLevelService(container, config);
        }

#if LARJE_ADDRESSABLES
        public static void BindLarjeAddressableLevels(this DiContainer container, LevelsConfig config, AddressableLevelsConfig levels)
        {
            container.BindInstance(levels);
            container.Bind<ILevelSource>().To<AddressableLevelSource>().AsSingle();
            BindLevelService(container, config);
        }
#endif

        public static void BindLarjeLocations(this DiContainer container, LocationsConfig config)
        {
            container.BindInstance(config);
            container.BindInterfacesAndSelfTo<LocationService>().AsSingle();
        }

        public static void BindLarjeLevelObjectKeys(this DiContainer container)
        {
            container.Bind<ILevelObjectKeyProvider>().To<DefaultLevelObjectKeyProvider>().AsSingle().IfNotBound();
        }

        public static void BindLarjeUI(this DiContainer container, UIConfig config)
        {
            container.BindInstance(config);
            container.BindInterfacesAndSelfTo<UIService>().AsSingle();
            container.BindInterfacesTo<UIDebugCommands>().AsSingle();
        }

        public static void BindLarjeDebugConsole(this DiContainer container, DebugConsoleConfig config)
        {
            container.BindInstance(config);
            container.BindInterfacesAndSelfTo<DebugConsoleService>().AsSingle();
        }

        private static void BindLevelService(DiContainer container, LevelsConfig config)
        {
            container.BindInstance(config);
            container.BindInterfacesAndSelfTo<LevelService>().AsSingle();
            container.BindInterfacesTo<LevelsDebugCommands>().AsSingle();
        }
    }
}
