using System.Collections.Generic;

namespace Larje.Core.TimeScale
{
    public interface ITimeScaleService
    {
        float Total { get; }
        IReadOnlyDictionary<TimeScaleLayerId, float> Layers { get; }

        void Set(TimeScaleLayerId layer, float scale);
        float Get(TimeScaleLayerId layer);
        void Play(TimeScaleAnimationId animation);
        void ResetAll();
    }
}
