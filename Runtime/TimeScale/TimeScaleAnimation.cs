using System;
using UnityEngine;

namespace Larje.Core.TimeScale
{
    [Serializable]
    public sealed class TimeScaleAnimation
    {
        [SerializeField, HideInInspector] private string inspectorName;
        [SerializeField] private TimeScaleAnimationId id;
        [SerializeField] private TimeScaleLayerId layer;
        [SerializeField] private float duration = 1f;
        [SerializeField] private Vector2 remap = new Vector2(0f, 1f);
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public TimeScaleAnimationId Id => id;
        public TimeScaleLayerId Layer => layer;
        public float Duration => duration;

        public float Evaluate(float t)
        {
            return Mathf.Lerp(remap.x, remap.y, curve.Evaluate(t));
        }

        public void Validate()
        {
            inspectorName = id != null ? id.name : "None";
        }
    }
}
