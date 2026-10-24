namespace Larje.Core.Levels
{
    public sealed class LevelStartData
    {
        public readonly LevelStartTypeId StartType;

        public LevelStartData(LevelStartTypeId startType)
        {
            StartType = startType;
        }
    }
}
