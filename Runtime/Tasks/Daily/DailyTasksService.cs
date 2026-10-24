using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Larje.Core.Tasks
{
    public sealed class DailyTasksService : TasksServiceBase, IInitializableService
    {
        private readonly DailyTasksConfig _config;
        private readonly List<TaskConfig> _tasks = new List<TaskConfig>();

        public override IReadOnlyList<TaskConfig> Tasks => _tasks;
        public DateTime NextReset => DateTime.Today.AddDays(1);

        private DailyTasksSaveData Data => _context.Saves.Get<DailyTasksSaveData>();

        public DailyTasksService(DailyTasksConfig config, TaskContext context) : base(context)
        {
            _config = config;
        }

        public void Initialize()
        {
            if (NeedsReset())
            {
                Reset();
            }
            else
            {
                Restore();
            }
        }

        public void Reset()
        {
            DestroyProcessors();
            foreach (TaskConfig task in _config.Pool)
            {
                _context.ClearData(task.TaskId);
            }

            _tasks.Clear();
            Data.Selected.Clear();
            Data.LastReset = DateTime.Now.ToString("o", CultureInfo.InvariantCulture);

            List<TaskConfig> available = new List<TaskConfig>();
            foreach (TaskConfig task in _config.Pool)
            {
                if (task != null && task.IsAvailable(_context))
                {
                    available.Add(task);
                }
            }

            while (_tasks.Count < _config.TasksPerDay && available.Count > 0)
            {
                TaskConfig picked = PickWeighted(available);
                available.Remove(picked);
                _tasks.Add(picked);
                Data.Selected.Add(picked.TaskId);
                StartProcessor(picked);
            }

            _context.Saves.SaveGame();
        }

        private void Restore()
        {
            foreach (TaskConfig task in _config.Pool)
            {
                if (task != null && Data.Selected.Contains(task.TaskId))
                {
                    _tasks.Add(task);
                    StartProcessor(task);
                }
            }
        }

        private bool NeedsReset()
        {
            if (Data.Selected.Count == 0)
            {
                return true;
            }

            bool parsed = DateTime.TryParse(Data.LastReset, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime lastReset);
            return !parsed || lastReset.Date != DateTime.Now.Date;
        }

        private TaskConfig PickWeighted(List<TaskConfig> tasks)
        {
            int total = 0;
            foreach (TaskConfig task in tasks)
            {
                total += task.Weight;
            }

            int roll = UnityEngine.Random.Range(0, total);
            foreach (TaskConfig task in tasks)
            {
                roll -= task.Weight;
                if (roll < 0)
                {
                    return task;
                }
            }

            return tasks[tasks.Count - 1];
        }
    }
}
