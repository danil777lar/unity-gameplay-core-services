using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tasks
{
    public abstract class TaskProcessor
    {
        private bool _destroyed;
        private readonly TaskConfig _config;
        private readonly TaskContext _context;

        public TaskConfig Config => _config;
        public TaskStatus Status => Data.Status;
        public int CurrentStep => RootStep != null ? RootStep.CurrentIndex : 0;

        protected TaskContext Context => _context;
        protected TaskData Data => _context.GetData(_config.TaskId);
        protected abstract TaskStep RootStep { get; }

        public event Action<TaskProcessor, TaskStatus> StatusChanged;

        protected TaskProcessor(TaskConfig config, TaskContext context)
        {
            _config = config;
            _context = context;
        }

        public void Initialize()
        {
            CreateTree();
            if (!Data.Initialized)
            {
                Data.Initialized = true;
                Data.Steps = RootStep.ReadData();
            }

            RootStep.InjectData(Data.Steps);
            RootStep.Initialize();
            RootStep.TreeCompleted += OnTreeCompleted;
            _context.Saves.Saving += OnSaving;
        }

        public void Start()
        {
            if (Data.Status == TaskStatus.NotStarted)
            {
                ChangeStatus(TaskStatus.Started);
                RootStep.Start();
            }
        }

        public void Fail()
        {
            if (Data.Status == TaskStatus.Started)
            {
                ChangeStatus(TaskStatus.Failed);
            }
        }

        public void Destroy()
        {
            if (_destroyed)
            {
                return;
            }

            _destroyed = true;
            _context.Saves.Saving -= OnSaving;
            if (RootStep != null)
            {
                RootStep.TreeCompleted -= OnTreeCompleted;
                RootStep.Destroy();
            }

            OnDestroyed();
        }

        public List<Func<Vector3>> GetTaskPositions()
        {
            return RootStep != null ? RootStep.GetTaskPositions() : new List<Func<Vector3>>();
        }

        protected abstract void CreateTree();

        protected virtual void OnCompleted()
        {
        }

        protected virtual void OnDestroyed()
        {
        }

        private void OnTreeCompleted()
        {
            Data.Tracking = false;
            ChangeStatus(TaskStatus.Completed);
            OnCompleted();
        }

        private void OnSaving()
        {
            Data.Steps = RootStep.ReadData();
        }

        private void ChangeStatus(TaskStatus status)
        {
            Data.Status = status;
            StatusChanged?.Invoke(this, status);
            _context.Events.Publish(new TaskStatusChangedEvent(_config, status));
        }
    }
}
