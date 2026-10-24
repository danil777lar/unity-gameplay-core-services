namespace Larje.Core.Input
{
    public readonly struct InputMapState
    {
        public readonly string Name;
        public readonly bool Enabled;
        public readonly int Conditions;

        public InputMapState(string name, bool enabled, int conditions)
        {
            Name = name;
            Enabled = enabled;
            Conditions = conditions;
        }
    }
}
