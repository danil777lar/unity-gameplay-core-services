using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Larje.Core.Levels
{
    public interface ILevelService
    {
        LevelProcessor Current { get; }
        bool IsPlaying { get; }
        bool IsSpawning { get; }
        int LevelNumber { get; }
        int LevelCount { get; }

        event Action<LevelProcessor> Spawned;
        event Action<LevelStartData> Started;
        event Action<LevelStopData> Stopped;

        int GetCurrentIndex();
        void SetLevelNumber(int number);
        void Advance();
        UniTask<LevelProcessor> SpawnCurrentAsync(CancellationToken cancellationToken = default);
        void TryStart(LevelStartData data);
        void TryStop(LevelStopData data);
        T GetLevelData<T>() where T : class;
    }
}
