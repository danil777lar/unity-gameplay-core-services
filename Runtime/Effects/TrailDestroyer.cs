using System.Threading;
using Cysharp.Threading.Tasks;
using Larje.Core.Tweening;
using UnityEngine;

namespace Larje.Core.Effects
{
    [RequireComponent(typeof(TrailRenderer))]
    public sealed class TrailDestroyer : MonoBehaviour, IEffectDestroyer
    {
        [SerializeField] private bool autoDestroy = true;
        [SerializeField] private float delay;
        [SerializeField] private float fadeDuration = 0.25f;

        private bool _destroying;

        public void DestroyEffect()
        {
            if (!_destroying)
            {
                _destroying = true;
                DestroyAsync(this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow().Forget();
            }
        }

        private void Start()
        {
            if (autoDestroy)
            {
                DestroyEffect();
            }
        }

        private async UniTask DestroyAsync(CancellationToken cancellationToken)
        {
            await Tween.Delay(delay, false, cancellationToken);
            TrailRenderer trail = GetComponent<TrailRenderer>();
            await Tween.Float(trail.time, 0f, new TweenSettings(fadeDuration, Ease.Linear), x => trail.time = x, cancellationToken);
            Destroy(gameObject);
        }
    }
}
