using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Larje.Core.Tweening
{
    public static class Tween
    {
        public static async UniTask Progress(TweenSettings settings, Action<float> onUpdate, CancellationToken cancellationToken = default)
        {
            if (settings.Delay > 0f)
            {
                await Delay(settings.Delay, settings.UnscaledTime, cancellationToken);
            }

            if (settings.Duration <= 0f)
            {
                onUpdate(settings.Evaluate(1f));
                return;
            }

            float elapsed = 0f;
            onUpdate(settings.Evaluate(0f));
            while (elapsed < settings.Duration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                elapsed += settings.UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                onUpdate(settings.Evaluate(Mathf.Clamp01(elapsed / settings.Duration)));
            }
        }

        public static UniTask Float(float from, float to, TweenSettings settings, Action<float> onUpdate, CancellationToken cancellationToken = default)
        {
            return Progress(settings, t => onUpdate(Mathf.LerpUnclamped(from, to, t)), cancellationToken);
        }

        public static UniTask Vector2(Vector2 from, Vector2 to, TweenSettings settings, Action<Vector2> onUpdate, CancellationToken cancellationToken = default)
        {
            return Progress(settings, t => onUpdate(UnityEngine.Vector2.LerpUnclamped(from, to, t)), cancellationToken);
        }

        public static UniTask Vector3(Vector3 from, Vector3 to, TweenSettings settings, Action<Vector3> onUpdate, CancellationToken cancellationToken = default)
        {
            return Progress(settings, t => onUpdate(UnityEngine.Vector3.LerpUnclamped(from, to, t)), cancellationToken);
        }

        public static UniTask Quaternion(Quaternion from, Quaternion to, TweenSettings settings, Action<Quaternion> onUpdate, CancellationToken cancellationToken = default)
        {
            return Progress(settings, t => onUpdate(UnityEngine.Quaternion.SlerpUnclamped(from, to, t)), cancellationToken);
        }

        public static UniTask Color(Color from, Color to, TweenSettings settings, Action<Color> onUpdate, CancellationToken cancellationToken = default)
        {
            return Progress(settings, t => onUpdate(UnityEngine.Color.LerpUnclamped(from, to, t)), cancellationToken);
        }

        public static UniTask Delay(float seconds, bool unscaledTime = false, CancellationToken cancellationToken = default)
        {
            DelayType delayType = unscaledTime ? DelayType.UnscaledDeltaTime : DelayType.DeltaTime;
            return UniTask.Delay(TimeSpan.FromSeconds(seconds), delayType, PlayerLoopTiming.Update, cancellationToken);
        }
    }
}
