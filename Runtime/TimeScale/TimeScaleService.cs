using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Larje.Core.Tweening;
using UnityEngine;

namespace Larje.Core.TimeScale
{
    public sealed class TimeScaleService : ITimeScaleService, IInitializableService, IDisposable
    {
        private readonly TimeScaleConfig _config;
        private readonly Dictionary<TimeScaleLayerId, float> _layers = new Dictionary<TimeScaleLayerId, float>();
        private readonly Dictionary<TimeScaleLayerId, TweenScope> _animations = new Dictionary<TimeScaleLayerId, TweenScope>();

        public float Total => Time.timeScale;
        public IReadOnlyDictionary<TimeScaleLayerId, float> Layers => _layers;

        public TimeScaleService(TimeScaleConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            Apply();
        }

        public void Dispose()
        {
            foreach (TweenScope scope in _animations.Values)
            {
                scope.Dispose();
            }

            _animations.Clear();
            _layers.Clear();
            Apply();
        }

        public void Set(TimeScaleLayerId layer, float scale)
        {
            if (layer == null)
            {
                return;
            }

            _layers[layer] = scale;
            Apply();
        }

        public float Get(TimeScaleLayerId layer)
        {
            return layer != null && _layers.TryGetValue(layer, out float scale) ? scale : 1f;
        }

        public void Play(TimeScaleAnimationId animationId)
        {
            TimeScaleAnimation animation = _config.Find(animationId);
            if (animation == null || animation.Layer == null)
            {
                Debug.LogWarning($"TimeScaleService: animation '{animationId}' not configured");
                return;
            }

            if (!_animations.TryGetValue(animation.Layer, out TweenScope scope))
            {
                scope = new TweenScope();
                _animations[animation.Layer] = scope;
            }

            CancellationToken token = scope.Restart();
            TweenSettings settings = new TweenSettings(animation.Duration, Ease.Linear, 0f, true);
            Tween.Progress(settings, t => Set(animation.Layer, animation.Evaluate(t)), token).SuppressCancellationThrow().Forget();
        }

        public void ResetAll()
        {
            foreach (TweenScope scope in _animations.Values)
            {
                scope.Kill();
            }

            _layers.Clear();
            Apply();
        }

        private void Apply()
        {
            float scale = 1f;
            foreach (float layer in _layers.Values)
            {
                scale *= layer;
            }

            scale = Mathf.Max(scale, _config.MinTimeScale);
            Time.timeScale = scale;
            Time.fixedDeltaTime = _config.DefaultFixedDeltaTime * scale;
        }
    }
}
