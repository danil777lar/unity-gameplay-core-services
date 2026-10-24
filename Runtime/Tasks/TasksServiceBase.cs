using System;
using System.Collections.Generic;

namespace Larje.Core.Tasks
{
    public abstract class TasksServiceBase : ITasksService, IDisposable
    {
        protected readonly TaskContext _context;
        protected readonly List<TaskProcessor> _processors = new List<TaskProcessor>();

        public abstract IReadOnlyList<TaskConfig> Tasks { get; }

        public event Action<TaskConfig, TaskStatus> StatusChanged;
        public event Action<TaskConfig> RewardClaimed;

        protected TasksServiceBase(TaskContext context)
        {
            _context = context;
        }

        public virtual void Dispose()
        {
            DestroyProcessors();
        }

        public TaskStatus GetStatus(TaskConfig task)
        {
            return _context.GetData(task.TaskId).Status;
        }

        public bool CanClaim(TaskConfig task)
        {
            TaskData data = _context.GetData(task.TaskId);
            return data.Status == TaskStatus.Completed && !data.RewardClaimed;
        }

        public bool Claim(TaskConfig task, int multiplier = 1)
        {
            if (!CanClaim(task))
            {
                return false;
            }

            _context.GetData(task.TaskId).RewardClaimed = true;
            foreach (TaskRewardConfig reward in task.Rewards)
            {
                if (reward != null)
                {
                    reward.Give(_context, multiplier);
                }
            }

            RewardClaimed?.Invoke(task);
            _context.Saves.SaveGame();
            return true;
        }

        public void SetTracking(TaskConfig task, bool tracking)
        {
            _context.GetData(task.TaskId).Tracking = tracking;
        }

        protected TaskProcessor StartProcessor(TaskConfig config)
        {
            TaskProcessor processor = config.CreateProcessor(_context);
            processor.StatusChanged += OnProcessorStatusChanged;
            processor.Initialize();
            processor.Start();
            _processors.Add(processor);
            return processor;
        }

        protected void DestroyProcessors()
        {
            foreach (TaskProcessor processor in _processors)
            {
                processor.StatusChanged -= OnProcessorStatusChanged;
                processor.Destroy();
            }

            _processors.Clear();
        }

        private void OnProcessorStatusChanged(TaskProcessor processor, TaskStatus status)
        {
            StatusChanged?.Invoke(processor.Config, status);
        }
    }
}
