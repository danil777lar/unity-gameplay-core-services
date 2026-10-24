using UnityEngine;

namespace Larje.Core.Tweening
{
    public static class EaseFunctions
    {
        private const float BACK = 1.70158f;
        private const float BACK_IN_OUT = BACK * 1.525f;

        public static float Evaluate(Ease ease, float t)
        {
            switch (ease)
            {
                case Ease.InSine:
                    return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                case Ease.OutSine:
                    return Mathf.Sin(t * Mathf.PI * 0.5f);
                case Ease.InOutSine:
                    return -(Mathf.Cos(Mathf.PI * t) - 1f) * 0.5f;
                case Ease.InQuad:
                    return t * t;
                case Ease.OutQuad:
                    return 1f - (1f - t) * (1f - t);
                case Ease.InOutQuad:
                    return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;
                case Ease.InCubic:
                    return t * t * t;
                case Ease.OutCubic:
                    return 1f - Mathf.Pow(1f - t, 3f);
                case Ease.InOutCubic:
                    return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;
                case Ease.InExpo:
                    return t <= 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f);
                case Ease.OutExpo:
                    return t >= 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
                case Ease.InOutExpo:
                    return EvaluateInOutExpo(t);
                case Ease.InBack:
                    return (BACK + 1f) * t * t * t - BACK * t * t;
                case Ease.OutBack:
                    return 1f + (BACK + 1f) * Mathf.Pow(t - 1f, 3f) + BACK * Mathf.Pow(t - 1f, 2f);
                case Ease.InOutBack:
                    return EvaluateInOutBack(t);
                case Ease.OutElastic:
                    return EvaluateOutElastic(t);
                case Ease.OutBounce:
                    return EvaluateOutBounce(t);
                default:
                    return t;
            }
        }

        private static float EvaluateInOutExpo(float t)
        {
            if (t <= 0f || t >= 1f)
            {
                return t;
            }

            return t < 0.5f
                ? Mathf.Pow(2f, 20f * t - 10f) * 0.5f
                : (2f - Mathf.Pow(2f, -20f * t + 10f)) * 0.5f;
        }

        private static float EvaluateInOutBack(float t)
        {
            return t < 0.5f
                ? Mathf.Pow(2f * t, 2f) * ((BACK_IN_OUT + 1f) * 2f * t - BACK_IN_OUT) * 0.5f
                : (Mathf.Pow(2f * t - 2f, 2f) * ((BACK_IN_OUT + 1f) * (t * 2f - 2f) + BACK_IN_OUT) + 2f) * 0.5f;
        }

        private static float EvaluateOutElastic(float t)
        {
            if (t <= 0f || t >= 1f)
            {
                return t;
            }

            return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * (2f * Mathf.PI / 3f)) + 1f;
        }

        private static float EvaluateOutBounce(float t)
        {
            const float n = 7.5625f;
            const float d = 2.75f;

            if (t < 1f / d)
            {
                return n * t * t;
            }

            if (t < 2f / d)
            {
                t -= 1.5f / d;
                return n * t * t + 0.75f;
            }

            if (t < 2.5f / d)
            {
                t -= 2.25f / d;
                return n * t * t + 0.9375f;
            }

            t -= 2.625f / d;
            return n * t * t + 0.984375f;
        }
    }
}
