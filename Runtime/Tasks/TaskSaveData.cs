using System;
using System.Collections.Generic;
using Larje.Core.Saves;

namespace Larje.Core.Tasks
{
    [Serializable]
    [SaveSection("tasks")]
    public sealed class TasksSaveData
    {
        public Dictionary<string, TaskData> Tasks = new Dictionary<string, TaskData>();

        public TaskData Get(string taskId)
        {
            if (!Tasks.TryGetValue(taskId, out TaskData data))
            {
                data = new TaskData();
                Tasks[taskId] = data;
            }

            return data;
        }
    }

    [Serializable]
    public sealed class TaskData
    {
        public bool Initialized;
        public bool Tracking;
        public bool RewardClaimed;
        public TaskStatus Status;
        public List<TaskStepData> Steps = new List<TaskStepData>();
    }

    [Serializable]
    public sealed class TaskStepData
    {
        public string Id;
        public string Type;
        public string Json;
    }
}
