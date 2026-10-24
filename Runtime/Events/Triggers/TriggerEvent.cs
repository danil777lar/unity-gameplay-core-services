namespace Larje.Core.Events
{
    public sealed class TriggerEvent : GameEvent
    {
        public readonly TriggerId Trigger;
        public readonly float Value;

        public override bool IsValid => Trigger != null;
        public bool BoolValue => Value >= 1f;

        public TriggerEvent(TriggerId trigger, float value, string source) : base(source)
        {
            Trigger = trigger;
            Value = value;
        }

        public override string ToString()
        {
            return $"Trigger {Trigger} = {Value}";
        }
    }
}
