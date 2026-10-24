using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Larje.Core.Extensions;
using Larje.Core.GameFlow;
using Larje.Core.Saves;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Larje.Core.Levels
{
    public sealed class LevelService : ILevelService, IInitializableService, IDisposable
    {
        private const string ROOT_NAME = "[Level]";

        private bool _isSpawning;
        private bool _firstSpawn = true;
        private LevelProcessor _current;
        private readonly LevelsConfig _config;
        private readonly ILevelSource _source;
        private readonly ISaveService _saves;
        private readonly IGameStateService _gameState;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();

        public LevelProcessor Current => _current;
        public bool IsPlaying => _current != null && _current.IsPlaying;
        public bool IsSpawning => _isSpawning;
        public int LevelNumber => Data.Completed;
        public int LevelCount => _source.Count;

        public event Action<LevelProcessor> Spawned;
        public event Action<LevelStartData> Started;
        public event Action<LevelStopData> Stopped;

        private LevelsSaveData Data => _saves.Get<LevelsSaveData>();

        public LevelService(LevelsConfig config, ILevelSource source, ISaveService saves, IGameStateService gameState)
        {
            _config = config;
            _source = source;
            _saves = saves;
            _gameState = gameState;
        }

        public void Initialize()
        {
            if (_config.SpawnOnInitialize)
            {
                SpawnCurrentAsync(_lifetime.Token).Forget();
            }
        }

        public void Dispose()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
            Detach();
        }

        public int GetCurrentIndex()
        {
            if (_source.Count == 0)
            {
                return -1;
            }

            if (Data.Completed < _source.Count)
            {
                return Data.Completed;
            }

            if (Data.RandomQueue.Count == 0)
            {
                RefillRandomQueue();
            }

            return Data.RandomQueue.Count > 0 ? Data.RandomQueue[0] : _source.Count - 1;
        }

        public void SetLevelNumber(int number)
        {
            Data.Completed = Mathf.Max(0, number);
            Data.RandomQueue.Clear();
            _saves.SaveGame();
        }

        public void Advance()
        {
            Data.Completed++;
            if (Data.RandomQueue.Count > 0)
            {
                Data.RandomQueue.RemoveAt(0);
            }

            _saves.SaveGame();
        }

        public async UniTask<LevelProcessor> SpawnCurrentAsync(CancellationToken cancellationToken = default)
        {
            if (_isSpawning)
            {
                return _current;
            }

            if (_firstSpawn && TryAttachSceneLevel())
            {
                return _current;
            }

            _isSpawning = true;
            try
            {
                Transform root = GetRoot();
                Detach();
                for (int i = root.childCount - 1; i >= 0; i--)
                {
                    _source.Release(root.GetChild(i).gameObject);
                }

                int index = GetCurrentIndex();
                Data.LastIndex = index;
                _saves.SaveGame();

                GameObject instance = await _source.InstantiateAsync(index, root, cancellationToken);
                if (instance == null || !instance.TryGetComponent(out LevelProcessor level))
                {
                    Debug.LogError($"LevelService: level {index} has no LevelProcessor");
                    return null;
                }

                Attach(level);
                Spawned?.Invoke(level);
                return level;
            }
            finally
            {
                _isSpawning = false;
                _firstSpawn = false;
            }
        }

        public void TryStart(LevelStartData data)
        {
            if (_current != null)
            {
                _current.TryStart(data);
            }
        }

        public void TryStop(LevelStopData data)
        {
            if (_current != null)
            {
                _current.TryStop(data);
            }
        }

        public T GetLevelData<T>() where T : class
        {
            return _current != null ? _current.GetLevelData() as T : null;
        }

        private bool TryAttachSceneLevel()
        {
            if (!Application.isEditor || !_config.UseSceneLevelInEditor)
            {
                return false;
            }

            LevelProcessor existing = Object.FindAnyObjectByType<LevelProcessor>();
            if (existing == null)
            {
                return false;
            }

            _firstSpawn = false;
            Attach(existing);
            Spawned?.Invoke(existing);
            return true;
        }

        private void Attach(LevelProcessor level)
        {
            _current = level;
            _current.Initialize(_gameState);
            _current.Started += OnStarted;
            _current.Stopped += OnStopped;
        }

        private void Detach()
        {
            if (_current != null)
            {
                _current.Started -= OnStarted;
                _current.Stopped -= OnStopped;
                _current = null;
            }
        }

        private void RefillRandomQueue()
        {
            List<int> queue = Data.RandomQueue;
            for (int i = 0; i < _source.Count; i++)
            {
                if (_source.IsRandomizable(i))
                {
                    queue.Add(i);
                }
            }

            queue.Shuffle();
            if (queue.Count > 1 && queue[0] == Data.LastIndex)
            {
                queue.Swap(0, UnityEngine.Random.Range(1, queue.Count));
            }
        }

        private Transform GetRoot()
        {
            LevelRoot root = Object.FindAnyObjectByType<LevelRoot>();
            if (root == null)
            {
                root = new GameObject(ROOT_NAME).AddComponent<LevelRoot>();
            }

            return root.transform;
        }

        private void OnStarted(LevelStartData data)
        {
            _gameState.Set(_gameState.States.Playing);
            Started?.Invoke(data);
        }

        private void OnStopped(LevelStopData data)
        {
            _gameState.Set(data.IsWin ? _gameState.States.Win : _gameState.States.Fail);
            Stopped?.Invoke(data);
        }
    }
}
