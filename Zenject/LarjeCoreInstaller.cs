using Larje.Core.Audio;
using Larje.Core.Debugging;
using Larje.Core.Economy;
using Larje.Core.GameFlow;
using Larje.Core.Input;
using Larje.Core.Levels;
using Larje.Core.Localization;
using Larje.Core.Locations;
using Larje.Core.Monetization;
using Larje.Core.Saves;
using Larje.Core.Tasks;
using Larje.Core.TimeScale;
using Larje.Core.UI;
using UnityEngine;
using Zenject;

namespace Larje.Core.Zenject
{
    [CreateAssetMenu(fileName = "LarjeCoreInstaller", menuName = "Larje/Core/Zenject/Core Installer")]
    public sealed class LarjeCoreInstaller : ScriptableObjectInstaller<LarjeCoreInstaller>
    {
        [Header("Saves")]
        [SerializeField] private SaveConfig saves;

        [Header("Game Flow")]
        [SerializeField] private GameStatesConfig gameStates;
        [SerializeField] private SceneLoaderConfig sceneLoader;
        [SerializeField] private AppFlowConfig appFlow;
        [SerializeField] private TimeScaleConfig timeScale;

        [Header("Input")]
        [SerializeField] private InputConfig input;
        [SerializeField] private PointerControlConfig pointerControl;

        [Header("Presentation")]
        [SerializeField] private UIConfig ui;
        [SerializeField] private SoundConfig sound;
        [SerializeField] private GTableLocalizationConfig localization;

        [Header("Economy")]
        [SerializeField] private CurrencyConfig currency;
        [SerializeField] private ItemsConfig items;
        [SerializeField] private DailyTasksConfig dailyTasks;

        [Header("Monetization")]
        [SerializeField] private bool bindSimulatedMonetization = true;
        [SerializeField] private PurchasesConfig purchases;
        [SerializeField] private DiscountQueueConfig discountQueue;

        [Header("Levels")]
        [SerializeField] private LevelsConfig levels;
        [SerializeField] private PrefabLevelsConfig prefabLevels;
        [SerializeField] private LocationsConfig locations;

        [Header("Debug")]
        [SerializeField] private DebugConsoleConfig debugConsole;

        public override void InstallBindings()
        {
            Container.BindLarjeInfrastructure();
            if (saves != null)
            {
                Container.BindLarjeSaves(saves);
            }

            if (gameStates != null && sceneLoader != null)
            {
                Container.BindLarjeGameFlow(gameStates, sceneLoader, appFlow);
            }

            if (timeScale != null)
            {
                Container.BindLarjeTimeScale(timeScale);
            }

            if (input != null)
            {
                Container.BindLarjeInput(input, pointerControl);
            }

            if (sound != null)
            {
                Container.BindLarjeAudio(sound);
            }

            if (localization != null)
            {
                Container.BindLarjeLocalization(localization);
            }

            Container.BindLarjeEconomy(currency, items);
            if (dailyTasks != null)
            {
                Container.BindLarjeDailyTasks(dailyTasks);
            }

            if (bindSimulatedMonetization)
            {
                Container.BindLarjeSimulatedMonetization(purchases);
            }

            if (discountQueue != null)
            {
                Container.BindLarjeDiscountQueue(discountQueue);
            }

            if (levels != null && prefabLevels != null)
            {
                Container.BindLarjeLevels(levels, prefabLevels);
            }

            if (locations != null)
            {
                Container.BindLarjeLocations(locations);
            }

            Container.BindLarjeLevelObjectKeys();
            if (ui != null)
            {
                Container.BindLarjeUI(ui);
            }

            if (debugConsole != null)
            {
                Container.BindLarjeDebugConsole(debugConsole);
            }
        }
    }
}
