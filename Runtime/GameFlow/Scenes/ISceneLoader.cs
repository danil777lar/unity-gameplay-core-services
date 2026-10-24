using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Larje.Core.GameFlow
{
    public interface ISceneLoader
    {
        bool IsLoading { get; }
        float Progress { get; }

        event Action<string> LoadStarted;
        event Action<string> Loaded;

        UniTask LoadAsync(string sceneName, CancellationToken cancellationToken = default);
        void Load(string sceneName);
    }
}
