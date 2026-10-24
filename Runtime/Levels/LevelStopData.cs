namespace Larje.Core.Levels
{
    public sealed class LevelStopData
    {
        public readonly bool IsWin;
        public readonly LevelStopTypeId StopType;

        public LevelStopData(bool isWin, LevelStopTypeId stopType)
        {
            IsWin = isWin;
            StopType = stopType;
        }
    }
}
