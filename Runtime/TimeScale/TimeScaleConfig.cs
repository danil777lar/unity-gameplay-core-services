using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.TimeScale
{
    [CreateAssetMenu(fileName = "TimeScaleConfig", menuName = "Larje/Core/Time Scale/Time Scale Config")]
    public sealed class TimeScaleConfig : ScriptableObject
    {
        [SerializeField] private float minTimeScale = 0f;
        [SerializeField] private float defaultFixedDeltaTime = 0.02f;
        [SerializeField] private List<TimeScaleAnimation> animations = new List<TimeScaleAnimation>();

        public float MinTimeScale => minTimeScale;
        public float DefaultFixedDeltaTime => defaultFixedDeltaTime;
        public IReadOnlyList<TimeScaleAnimation> Animations => animations;

        public TimeScaleAnimation Find(TimeScaleAnimationId id)
        {
            return animations.Find(x => x.Id == id);
        }

        private void OnValidate()
        {
            animations.ForEach(x => x.Validate());
        }
    }
}
