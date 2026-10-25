# AGENTS.md: Larje Core

Instructions for coding agents working on this package.

## Package layout

- `Runtime/`: `Larje.Core`, with one folder per feature (Saves, Events, GameFlow, Audio, Input, Economy, Levels, Locations, Localization, Tasks, Monetization, Debugging, Foundation).
- `Runtime/Addressables/`: `Larje.Core.Addressables` (`LARJE_ADDRESSABLES`).
- UI is not here: it lives in `com.larje.ui` (`~/UnityPackages/unity-ui-framework`), which depends on core. Core must not reference it.
- `Zenject/`: `Larje.Core.Zenject`, which contains `LarjeCoreInstaller`, `LarjeContainerExtensions` (`BindLarje*`) and `LarjeInitializer`.
- `Editor/`: inspectors and tools. `Editor/Cli/` holds the Unity CLI commands (`larje.*` groups). `Editor/Defines/` holds the optional-dependency define sync; provider packages register their SDK assemblies through `OptionalDependencyDefines.Register`, keep it dependency-free.
- `Tests/Editor`, `Tests/Zenject`: EditMode tests.

## When adding a service

1. Add an interface and a sealed implementation in the feature folder. Add a config ScriptableObject if the service needs settings.
2. Implement `IInitializableService`, `IAsyncInitializableService` or `IDisposable` as needed.
3. Add a `BindLarje<Feature>` extension and an optional slot in `LarjeCoreInstaller`.
4. Register debug commands through a `DebugCommands` provider if the feature is useful to poke at runtime.
5. If agents need to inspect or edit the feature's data, add or extend a Unity CLI command in `Editor/Cli`.

## Code style (C#)

- Do not use `var`. Write explicit types.
- Do not write comments in code.
- Do not use `continue`. Restructure the loop or extract a method.
- Seal classes when they are not designed for inheritance.
- Use block bodies `{ }` for methods and `if` statements. Expression-bodied members are fine for simple properties when the surrounding code already uses them.
- Do not name variables `value` except in property setters.
- Make private methods `static` only when a static context requires it.
- Order members as: constants, static fields, serialized fields, fields, properties, events, methods, nested enums, nested classes. Within each group order by access: public, protected, internal, private.
- Private non-serialized fields use `_camelCase`, constants use `UPPER_SNAKE_CASE`. `[SerializeField]` fields follow the style already used in the package (mostly `camelCase`); never rename an existing serialized field without `[FormerlySerializedAs]`, because assets and `com.larje.projectsetup` reference fields by name.
- Serialized fields: never use `[Tooltip]`. Group related fields with `[Header]` and avoid single-field headers.

## Architecture rules

- Services are plain C# classes with constructor injection. Do not add a service locator, singletons or a custom DI container.
- Use UniTask with a `CancellationToken` for loops, delays and async work. Do not use `Update` loops or coroutines in services.
- Ids are ScriptableObject assets. Persistent data lives in save sections owned by the feature.
- Zenject code goes only into the `*.Zenject` assembly. It compiles under `LARJE_ZENJECT`, which comes from `versionDefines` on `com.larje.zenject`.
- Unity CLI commands (`[CliGroupCommand]` from `com.unity.pipeline`, called with `unity command <group> <name>`) go only into the `*.Cli.Editor` assembly. It references `Unity.Pipeline.Attributes` and compiles under `LARJE_PIPELINE` (a `versionDefines` entry on `com.unity.pipeline`). Groups use the `larje.<area>` prefix (`larje.ids`, `larje.save`), command and parameter names are snake_case, failures are thrown exceptions, and commands that take large JSON also accept `--file`. Put logic that tests need into the regular editor assembly so the tests do not depend on the Pipeline package.
- Runtime assemblies must not reference editor-only APIs outside `#if UNITY_EDITOR`.

## Working in a Unity project

- Do not edit `.prefab`, `.unity`, `.asset` or `.mat` files as text. Change them through the Unity Editor: editor scripts, or `unity command` against a running editor.
- Commit the `.meta` file for every new file and folder. Git packages are immutable, so missing metas break consumers.
- After every C# change, refresh the editor, wait until compilation is idle, and check that the console has no errors or warnings before moving on.
- Run EditMode tests through the Test Runner. List the package in the project manifest `testables` first.

## Commits

- Write short technical commit messages only. Do not push unless explicitly asked.
