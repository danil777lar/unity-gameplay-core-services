using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Larje.Core.GameFlow
{
    public sealed class AppFlowService : IAppFlowService, IInitializableService, IDisposable
    {
        private float _splashProgress;
        private readonly AppFlowConfig _config;
        private readonly IGameStateService _gameState;
        private readonly ISceneLoader _sceneLoader;
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();

        public float SplashProgress => _splashProgress;
        public string MenuScene => _config.MenuScene;

        public AppFlowService(AppFlowConfig config, IGameStateService gameState, ISceneLoader sceneLoader)
        {
            _config = config;
            _gameState = gameState;
            _sceneLoader = sceneLoader;
        }

        public void Initialize()
        {
            ApplyApplicationSettings();
            if (_config.RunOnInitialize)
            {
                RunAsync(_lifetime.Token).Forget();
            }
        }

        public void Dispose()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        public async UniTask ShowSplashAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_config.SplashScene) || _gameState.Is(_gameState.States.Splash))
            {
                return;
            }

            _splashProgress = 0f;
            _gameState.Set(_gameState.States.Splash);
            await SceneManager.LoadSceneAsync(_config.SplashScene).ToUniTask(cancellationToken: cancellationToken);

            float elapsed = 0f;
            while (elapsed < _config.SplashDuration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                elapsed += Time.unscaledDeltaTime;
                _splashProgress = Mathf.Clamp01(elapsed / _config.SplashDuration);
            }

            _splashProgress = 1f;
        }

        public void ShowMenu()
        {
            if (string.IsNullOrEmpty(_config.MenuScene) || _gameState.Is(_gameState.States.Menu))
            {
                return;
            }

            _gameState.Set(_gameState.States.Menu);
            SceneManager.LoadScene(_config.MenuScene);
        }

        public UniTask LoadSceneAsync(string sceneName, CancellationToken cancellationToken = default)
        {
            return _sceneLoader.LoadAsync(sceneName, cancellationToken);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private async UniTaskVoid RunAsync(CancellationToken cancellationToken)
        {
            await ShowSplashAsync(cancellationToken);
            ShowMenu();
        }

        private void ApplyApplicationSettings()
        {
            Application.targetFrameRate = _config.TargetFrameRate;
            if (_config.NeverSleep)
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

#if !UNITY_EDITOR
            Debug.unityLogger.logEnabled = _config.LogsInBuild;
#endif
        }
    }
}
