namespace Larje.Core.Events
{
    public readonly struct EventRecord
    {
        public readonly float Time;
        public readonly string Type;
        public readonly string Source;
        public readonly string Description;

        public EventRecord(float time, string type, string source, string description)
        {
            Time = time;
            Type = type;
            Source = source;
            Description = description;
        }
    }
}
