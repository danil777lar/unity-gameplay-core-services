using System.Threading;
using Cysharp.Threading.Tasks;

namespace Larje.Core.GameFlow
{
    public interface IAppFlowService
    {
        float SplashProgress { get; }
        string MenuScene { get; }

        UniTask ShowSplashAsync(CancellationToken cancellationToken = default);
        void ShowMenu();
        UniTask LoadSceneAsync(string sceneName, CancellationToken cancellationToken = default);
        void Quit();
    }
}
