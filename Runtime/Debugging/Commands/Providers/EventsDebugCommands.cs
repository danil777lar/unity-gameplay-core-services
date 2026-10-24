using System.Text;
using Larje.Core.Events;

namespace Larje.Core.Debugging
{
    public sealed class EventsDebugCommands : IDebugCommandProvider
    {
        private readonly IEventBus _events;

        public EventsDebugCommands(IEventBus events)
        {
            _events = events;
        }

        [DebugCommand("Events", "Fire trigger with value")]
        public string FireTrigger(TriggerId trigger, float value = 1f)
        {
            _events.Publish(new TriggerEvent(trigger, value, "DebugConsole"));
            return $"{trigger.Key} = {value}";
        }

        [DebugCommand("Events", "Print recent events")]
        public string History(int count = 20)
        {
            StringBuilder builder = new StringBuilder();
            int start = System.Math.Max(0, _events.History.Count - count);
            for (int i = start; i < _events.History.Count; i++)
            {
                EventRecord record = _events.History[i];
                builder.Append($"[{record.Time:0.00}] {record.Description} ({record.Source})\n");
            }

            return builder.Length > 0 ? builder.ToString() : "No events";
        }
    }
}
