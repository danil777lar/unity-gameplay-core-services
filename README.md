# Larje Core

`com.larje.core`: gameplay services and tools for Unity 6 projects. It covers saves, events, game states and scenes, levels and locations, audio, input, economy, localization, tasks, monetization abstractions and a debug console.

UI (screens, popups, toasts, UI animations and components) lives in the separate package `com.larje.ui` ([unity-ui-framework](https://github.com/danil777lar/unity-ui-framework)), which depends on this one.

Services are plain C# classes with constructor injection. Long-running work uses UniTask and `CancellationToken` instead of `Update` and coroutines. Zenject is optional. When it is present, an installer binds everything for you.

## Install

UPM cannot resolve git dependencies, so add UniTask and core to `Packages/manifest.json` yourself:

```json
"com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask#2.5.10",
"com.larje.core": "https://github.com/danil777lar/unity-gameplay-core-services.git"
```

Optional:

```json
"com.larje.zenject": "https://github.com/danil777lar/unity-zenject-upm.git"
```

Requires Unity 6000.3+, Input System, uGUI and Newtonsoft Json. These are resolved from the Unity registry.

## Assemblies

| Assembly | Folder | Enabled when |
|---|---|---|
| `Larje.Core` | `Runtime` | always |
| `Larje.Core.Addressables` | `Runtime/Addressables` | `com.unity.addressables` is installed (`LARJE_ADDRESSABLES`) |
| `Larje.Core.Zenject` | `Zenject` | `com.larje.zenject` or an Asset Store Zenject is present (`LARJE_ZENJECT`) |
| `Larje.Core.Editor` | `Editor` | Editor |
| `Larje.Core.AI.Editor` | `Editor/AI` | `com.unity.ai.assistant` is installed (`LARJE_UNITY_AI`) |
| `Larje.Core.Defines.Editor` | `Editor/Defines` | Editor; adds or removes `LARJE_ZENJECT` when Zenject is imported into `Assets`, and the SDK defines that provider packages register with `OptionalDependencyDefines.Register(assemblyName, define)` (asmdef or precompiled DLL names) |

## Services

| Area | Interface | Implementation | Config |
|---|---|---|---|
| Saves | `ISaveService` | `SaveService` (sections, pluggable `ISaveStorage`) | `SaveConfig` |
| Events | `IEventBus` | `EventBus` | — |
| Game flow | `IGameStateService`, `IAppFlowService` | `GameStateService`, `AppFlowService`, scene loader | `GameStatesConfig`, `SceneLoaderConfig`, `AppFlowConfig` |
| Time scale | `ITimeScaleService` | `TimeScaleService` | `TimeScaleConfig` |
| Input | `IInputService`, `IPointerControlService` | `InputService`, `PointerControlService` | `InputConfig`, `PointerControlConfig` |
| Audio | `ISoundService`, `ISystemVolumeService` | `SoundService`, `SystemVolumeService` | `SoundConfig` |
| Localization | `ILocalizationService` | `GTableLocalizationService` (Google Sheets export in `Documentation~/gtable-export.gs`) | `GTableLocalizationConfig` |
| Economy | `ICurrencyService`, `IItemsService` | `CurrencyService`, `ItemsService` | `CurrencyConfig`, `ItemsConfig` |
| Tasks | `ITasksService` | `DailyTasksService` | `DailyTasksConfig` |
| Levels | `ILevelService` | `LevelService` (prefab or Addressables levels) | `LevelsConfig`, `PrefabLevelsConfig`, `AddressableLevelsConfig` |
| Locations | `ILocationService` | `LocationService` | `LocationsConfig` |
| Haptics | `IHapticService` | `HapticService` | — |
| Monetization | `IAdsService`, `IPurchasesService`, `IAnalyticsService` | `SimulatedAdsService`, `SimulatedPurchasesService`, `LogAnalyticsService` | `PurchasesConfig`, `DiscountQueueConfig` |
| Debug | `IDebugConsoleService`, `IDebugCommandRegistry` | `DebugConsoleService` | `DebugConsoleConfig` |

Ids such as `CurrencyId`, `SoundId`, `GameStateId`, `LocationId` and `TriggerId` are ScriptableObject assets, not generated enums. Save data is split into sections, so each feature owns its own data type.

Real SDK providers live in separate packages: `com.larje.ads-admob`, `com.larje.ads-unity`, `com.larje.ads-telegram` (Adsgram), `com.larje.analytics-firebase` and `com.larje.iap-unity`. Each ships a Zenject installer that calls `BindLarjeAdsProvider<T>()`, `BindLarjeAnalyticsProvider<T>()` or `BindLarjePurchasesProvider<T>(config)`. These replace the simulated service whether they run before or after `LarjeCoreInstaller`, because `BindLarjeSimulatedMonetization` skips contracts that are already bound. For a custom provider implement the interface or derive from `AdsServiceBase` / `PurchasesServiceBase` and bind it the same way.

## Lifecycle

- `IInitializableService.Initialize()` is called once after construction.
- `IAsyncInitializableService.InitializeAsync(CancellationToken)` is awaited after all synchronous initializers.
- Services that own loops or subscriptions implement `IDisposable`.

## With Zenject

1. Create `Larje/Core/Zenject/Core Installer` (`LarjeCoreInstaller`), assign the configs you use and add it to the `ProjectContext` scriptable object installers. Empty slots are skipped.
2. Or call the container extensions from your own installer: `BindLarjeInfrastructure`, `BindLarjeSaves`, `BindLarjeGameFlow`, `BindLarjeTimeScale`, `BindLarjeInput`, `BindLarjeAudio`, `BindLarjeLocalization`, `BindLarjeEconomy`, `BindLarjeDailyTasks`, `BindLarjeSimulatedMonetization`, `BindLarjeAdsProvider` / `BindLarjeAnalyticsProvider` / `BindLarjePurchasesProvider`, `BindLarjeDiscountQueue`, `BindLarjeLevels` / `BindLarjeAddressableLevels`, `BindLarjeLocations`, `BindLarjeLevelObjectKeys`, `BindLarjeDebugConsole`.
3. `BindLarjeInfrastructure` registers `LarjeInitializer`. Inject `ILarjeReady` and `await WaitAsync()` before using services that initialize asynchronously.

Zenject injects scene objects before their `Awake`. Component `Construct` methods must not rely on fields set in `Awake`.

## Without Zenject

Construct the services yourself, call `Initialize` / `InitializeAsync` in that order, and use `DefaultObjectInjector` as `IObjectInjector`. Nothing in `Larje.Core` depends on a container.

## Unity AI (MCP) tools

These are available when `com.unity.ai.assistant` is installed:

| Tool | Purpose |
|---|---|
| `Larje.ListConfigs` | List Larje config assets in the project |
| `Larje.ValidateProject` | Validate configs and ids: missing references, duplicate keys, broken levels, sounds without clips |
| `Larje.ListIds` / `Larje.CreateId` | List or create id assets |
| `Larje.ReadSave` / `Larje.WriteSaveSection` | Read or replace save sections on disk outside Play Mode |
| `Larje.ListDebugCommands` / `Larje.RunDebugCommand` | Inspect and run debug commands in Play Mode |
| `Larje.PreviewLocationArgs` | Preview location argument activators in the open scene |
| `Larje.RenderPrefabIcons` | Render PNG icons for prefabs |

## Tests

Add `"testables": ["com.larje.core"]` to the project manifest and run the EditMode tests (`Larje.Core.Tests.Editor`, plus `Larje.Core.Zenject.Tests` when Zenject is present).
