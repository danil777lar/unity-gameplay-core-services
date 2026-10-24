using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Larje.Core.Events;
using Larje.Core.GameFlow;
using Larje.Core.LevelObjects;
using Larje.Core.Saves;
using Larje.Core.Tweening;
using UnityEngine;

namespace Larje.Core.Locations
{
    public sealed class LocationService : ILocationService, ILevelObjectKeyProvider, IInitializableService, IDisposable
    {
        private bool _isLoaded;
        private bool _isTransitioning;
        private float _transition;
        private readonly LocationsConfig _config;
        private readonly ISaveService _saves;
        private readonly IEventBus _events;
        private readonly IGameStateService _gameState;
        private readonly ISceneLoader _sceneLoader;
        private readonly IAppFlowService _appFlow;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();
        private readonly List<ILocationEntry> _entries = new List<ILocationEntry>();
        private readonly List<Callback> _callbacks = new List<Callback>();

        public LocationDefinition Current => _config.FindByKey(Data.Location);
        public int CurrentEntry => Data.Entry;
        public bool IsLoaded => _isLoaded;
        public float Transition => _transition;
        public LocationsConfig Config => _config;

        public event Action Exiting;
        public event Action LoadStarted;
        public event Action<LocationDefinition, int> Loaded;
        public event Action<LocationDefinition, int> Entered;

        private LocationSaveData Data => _saves.Get<LocationSaveData>();

        public LocationService(LocationsConfig config, ISaveService saves, IEventBus events, IGameStateService gameState, ISceneLoader sceneLoader, IAppFlowService appFlow)
        {
            _config = config;
            _saves = saves;
            _events = events;
            _gameState = gameState;
            _sceneLoader = sceneLoader;
            _appFlow = appFlow;
        }

        public void Initialize()
        {
            _saves.Loaded += OnSaveLoaded;
            if (_config.LoadOnInitialize)
            {
                OnSaveLoaded();
            }
        }

        public void Dispose()
        {
            _saves.Loaded -= OnSaveLoaded;
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        public async UniTask LoadAsync(LocationId location, int entry = 0, CancellationToken cancellationToken = default)
        {
            LocationDefinition info = _config.Find(location);
            if (info == null || _isTransitioning)
            {
                Debug.LogWarning($"LocationService: cannot load '{location}'");
                return;
            }

            _isTransitioning = true;
            _isLoaded = false;
            Data.Initialized = true;
            Data.Location = location.Key;
            Data.Entry = entry;

            try
            {
                _gameState.Set(_gameState.States.Transition);
                LoadStarted?.Invoke();
                await FadeAsync(1f, cancellationToken);
                Exiting?.Invoke();

                await _sceneLoader.LoadAsync(info.SceneName, cancellationToken);
                _isLoaded = true;
                _gameState.Set(_gameState.States.Transition);

                foreach (LocationAction action in info.Actions)
                {
                    if (action != null)
                    {
                        action.Apply(info);
                    }
                }

                foreach (TriggerId trigger in info.Triggers)
                {
                    _events.Publish(new TriggerEvent(trigger, 1f, nameof(LocationService)));
                }

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                InvokeCallbacks();
                Loaded?.Invoke(info, entry);

                await FadeAsync(0f, cancellationToken);
                _gameState.Set(_gameState.States.Playing);
                Entered?.Invoke(info, entry);
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        public async UniTask QuitToMenuAsync(CancellationToken cancellationToken = default)
        {
            _isLoaded = false;
            _gameState.Set(_gameState.States.Transition);
            await FadeAsync(1f, cancellationToken);
            await _sceneLoader.LoadAsync(_appFlow.MenuScene, cancellationToken);
            await FadeAsync(0f, cancellationToken);
            _gameState.Set(_gameState.States.Menu);
        }

        public void AddEntry(ILocationEntry entry)
        {
            _entries.RemoveAll(x => x.Id == entry.Id);
            _entries.Add(entry);
        }

        public void RemoveEntry(ILocationEntry entry)
        {
            _entries.Remove(entry);
        }

        public bool TryGetEntry(out ILocationEntry entry)
        {
            entry = _entries.Find(x => x.Id == Data.Entry);
            return entry != null;
        }

        public IDisposable OnEntered(Action callback, LocationFilter filter = null)
        {
            Callback entry = new Callback(callback, filter ?? new LocationFilter(), _callbacks);
            _callbacks.Add(entry);
            if (_isLoaded)
            {
                entry.TryInvoke(Current, CurrentEntry);
            }

            return entry;
        }

        public string GetKeyPrefix()
        {
            LocationDefinition current = Current;
            if (current == null)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder(current.Id.Key);
            foreach (LocationArgId arg in current.Args)
            {
                builder.Append('(').Append(arg.Key).Append(')');
            }

            return builder.ToString();
        }

        private UniTask FadeAsync(float target, CancellationToken cancellationToken)
        {
            TweenSettings settings = new TweenSettings(_config.TransitionDuration, Ease.Linear, 0f, true);
            return Tween.Float(_transition, target, settings, x => _transition = x, cancellationToken);
        }

        private void InvokeCallbacks()
        {
            foreach (Callback callback in _callbacks.ToArray())
            {
                callback.TryInvoke(Current, CurrentEntry);
            }
        }

        private void OnSaveLoaded()
        {
            if (!Data.Initialized)
            {
                Data.Location = _config.DefaultLocation != null ? _config.DefaultLocation.Key : null;
                Data.Entry = _config.DefaultEntry;
            }

            LocationDefinition current = Current;
            if (current != null)
            {
                LoadAsync(current.Id, Data.Entry, _lifetime.Token).Forget();
            }
        }

        private sealed class Callback : IDisposable
        {
            private readonly Action _action;
            private readonly LocationFilter _filter;
            private List<Callback> _owner;

            public Callback(Action action, LocationFilter filter, List<Callback> owner)
            {
                _action = action;
                _filter = filter;
                _owner = owner;
            }

            public void TryInvoke(LocationDefinition location, int entry)
            {
                if (_filter.Matches(location, entry))
                {
                    _action();
                }
            }

            public void Dispose()
            {
                _owner?.Remove(this);
                _owner = null;
            }
        }
    }
}
