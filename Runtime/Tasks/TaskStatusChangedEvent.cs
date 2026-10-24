using Larje.Core.Events;

namespace Larje.Core.Tasks
{
    public sealed class TaskStatusChangedEvent : GameEvent
    {
        public readonly TaskConfig Task;
        public readonly TaskStatus Status;

        public override bool IsValid => Task != null;

        public TaskStatusChangedEvent(TaskConfig task, TaskStatus status) : base(task != null ? task.name : string.Empty)
        {
            Task = task;
            Status = status;
        }

        public override string ToString()
        {
            return $"Task {Source} -> {Status}";
        }
    }
}
