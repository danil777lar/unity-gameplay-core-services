using System;
using Larje.Core.GameFlow;
using UnityEngine;

namespace Larje.Core.Levels
{
    public abstract class LevelProcessor : MonoBehaviour
    {
        private IGameStateService _gameState;

        public bool IsPlaying => _gameState != null && _gameState.Is(_gameState.States.Playing);
        public bool IsPaused => _gameState != null && _gameState.Is(_gameState.States.Paused);

        public event Action<LevelStartData> Started;
        public event Action<LevelStopData> Stopped;

        public void Initialize(IGameStateService gameState)
        {
            _gameState = gameState;
            OnInitialized();
        }

        public abstract void TryStart(LevelStartData data);
        public abstract void TryStop(LevelStopData data);

        public virtual object GetLevelData()
        {
            return null;
        }

        protected virtual void OnInitialized()
        {
        }

        protected void StartLevel(LevelStartData data)
        {
            if (IsPlaying)
            {
                return;
            }

            foreach (ILevelStartHandler handler in GetComponentsInChildren<ILevelStartHandler>(true))
            {
                handler.OnLevelStarted(data);
            }

            Started?.Invoke(data);
        }

        protected void StopLevel(LevelStopData data)
        {
            if (!IsPlaying && !IsPaused)
            {
                return;
            }

            foreach (ILevelStopHandler handler in GetComponentsInChildren<ILevelStopHandler>(true))
            {
                handler.OnLevelStopped(data);
            }

            Stopped?.Invoke(data);
        }
    }
}
