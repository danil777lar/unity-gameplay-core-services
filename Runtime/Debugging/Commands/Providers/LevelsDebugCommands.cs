using Cysharp.Threading.Tasks;
using Larje.Core.Levels;

namespace Larje.Core.Debugging
{
    public sealed class LevelsDebugCommands : IDebugCommandProvider
    {
        private readonly ILevelService _levels;

        public LevelsDebugCommands(ILevelService levels)
        {
            _levels = levels;
        }

        [DebugCommand("Levels", "Spawn current level")]
        public string Spawn()
        {
            _levels.SpawnCurrentAsync().Forget();
            return "Spawning level " + _levels.GetCurrentIndex();
        }

        [DebugCommand("Levels", "Set completed level count")]
        public string SetLevelNumber(int number)
        {
            _levels.SetLevelNumber(number);
            return "Level number " + _levels.LevelNumber;
        }

        [DebugCommand("Levels", "Start current level")]
        public string StartLevel(LevelStartTypeId startType = null)
        {
            _levels.TryStart(new LevelStartData(startType));
            return _levels.IsPlaying ? "Playing" : "Not started";
        }

        [DebugCommand("Levels", "Stop current level")]
        public string StopLevel(bool win, LevelStopTypeId stopType = null)
        {
            _levels.TryStop(new LevelStopData(win, stopType));
            return win ? "Win" : "Fail";
        }

        [DebugCommand("Levels", "Print level status")]
        public string Status()
        {
            return $"number={_levels.LevelNumber} index={_levels.GetCurrentIndex()} count={_levels.LevelCount} playing={_levels.IsPlaying}";
        }
    }
}
