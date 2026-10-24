using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Larje.Core.GameFlow
{
    public sealed class SceneLoader : ISceneLoader
    {
        private const float UNITY_LOAD_READY_PROGRESS = 0.9f;

        private bool _isLoading;
        private float _progress;
        private readonly SceneLoaderConfig _config;
        private readonly IGameStateService _gameState;

        public bool IsLoading => _isLoading;
        public float Progress => _progress;

        public event Action<string> LoadStarted;
        public event Action<string> Loaded;

        public SceneLoader(SceneLoaderConfig config, IGameStateService gameState)
        {
            _config = config;
            _gameState = gameState;
        }

        public void Load(string sceneName)
        {
            LoadAsync(sceneName).Forget();
        }

        public async UniTask LoadAsync(string sceneName, CancellationToken cancellationToken = default)
        {
            if (_isLoading)
            {
                Debug.LogWarning($"SceneLoader: already loading, '{sceneName}' ignored");
                return;
            }

            _isLoading = true;
            _progress = 0f;
            _gameState.Set(_gameState.States.Loading);
            LoadStarted?.Invoke(sceneName);

            try
            {
                if (_config.MinimumDelay > 0f)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_config.MinimumDelay), DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update, cancellationToken);
                }

                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                operation.allowSceneActivation = false;

                while (_progress < 1f)
                {
                    float target = Mathf.Clamp01(operation.progress / UNITY_LOAD_READY_PROGRESS);
                    _progress = Mathf.MoveTowards(_progress, target, Time.unscaledDeltaTime * _config.ProgressSmoothSpeed);
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }

                operation.allowSceneActivation = true;
                await operation.ToUniTask(cancellationToken: cancellationToken);
            }
            finally
            {
                _isLoading = false;
            }

            Loaded?.Invoke(sceneName);
        }
    }
}
