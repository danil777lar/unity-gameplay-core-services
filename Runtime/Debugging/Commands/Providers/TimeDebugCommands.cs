using Larje.Core.TimeScale;
using UnityEngine;

namespace Larje.Core.Debugging
{
    public sealed class TimeDebugCommands : IDebugCommandProvider
    {
        private readonly ITimeScaleService _timeScale;

        public TimeDebugCommands(ITimeScaleService timeScale)
        {
            _timeScale = timeScale;
        }

        [DebugCommand("Time", "Set time scale layer")]
        public string SetLayer(TimeScaleLayerId layer, float scale)
        {
            _timeScale.Set(layer, scale);
            return "Total " + _timeScale.Total;
        }

        [DebugCommand("Time", "Reset all time scale layers")]
        public string ResetAll()
        {
            _timeScale.ResetAll();
            return "Total " + Time.timeScale;
        }

        [DebugCommand("Time", "Play time scale animation")]
        public string PlayAnimation(TimeScaleAnimationId animation)
        {
            _timeScale.Play(animation);
            return "Playing " + animation.Key;
        }
    }
}
