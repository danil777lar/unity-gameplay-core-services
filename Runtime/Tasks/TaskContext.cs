using System;
using Larje.Core.Events;
using Larje.Core.Saves;

namespace Larje.Core.Tasks
{
    public sealed class TaskContext
    {
        public readonly ISaveService Saves;
        public readonly IEventBus Events;
        public readonly IServiceProvider Services;

        public TaskContext(ISaveService saves, IEventBus events, IServiceProvider services)
        {
            Saves = saves;
            Events = events;
            Services = services;
        }

        public T Resolve<T>() where T : class
        {
            return Services?.GetService(typeof(T)) as T;
        }

        public TaskData GetData(string taskId)
        {
            return Saves.Get<TasksSaveData>().Get(taskId);
        }

        public void ClearData(string taskId)
        {
            Saves.Get<TasksSaveData>().Tasks.Remove(taskId);
        }
    }
}
