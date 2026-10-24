using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Larje.Core
{
    public static class ServiceLoop
    {
        public static void Run(Action<float> tick, CancellationToken cancellationToken, PlayerLoopTiming timing = PlayerLoopTiming.Update, bool unscaledTime = false)
        {
            RunAsync(tick, cancellationToken, timing, unscaledTime).Forget();
        }

        private static async UniTaskVoid RunAsync(Action<float> tick, CancellationToken cancellationToken, PlayerLoopTiming timing, bool unscaledTime)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                bool cancelled = await UniTask.Yield(timing, cancellationToken).SuppressCancellationThrow();
                if (cancelled)
                {
                    return;
                }

                tick(unscaledTime ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime);
            }
        }
    }
}
