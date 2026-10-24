using System;
using System.Collections.Generic;

namespace Larje.Core.Tasks
{
    public interface ITasksService
    {
        IReadOnlyList<TaskConfig> Tasks { get; }

        event Action<TaskConfig, TaskStatus> StatusChanged;
        event Action<TaskConfig> RewardClaimed;

        TaskStatus GetStatus(TaskConfig task);
        bool CanClaim(TaskConfig task);
        bool Claim(TaskConfig task, int multiplier = 1);
        void SetTracking(TaskConfig task, bool tracking);
    }
}
