using System;
using UnityEngine;

namespace Larje.Core.Tweening
{
    [Serializable]
    public struct TweenSettings
    {
        public float Duration;
        public float Delay;
        public Ease Ease;
        public bool UseCurve;
        public AnimationCurve Curve;
        public bool UnscaledTime;

        public TweenSettings(float duration, Ease ease = Ease.OutQuad, float delay = 0f, bool unscaledTime = false)
        {
            Duration = duration;
            Delay = delay;
            Ease = ease;
            UseCurve = false;
            Curve = null;
            UnscaledTime = unscaledTime;
        }

        public float Evaluate(float t)
        {
            if (UseCurve && Curve != null)
            {
                return Curve.Evaluate(t);
            }

            return EaseFunctions.Evaluate(Ease, t);
        }
    }
}
