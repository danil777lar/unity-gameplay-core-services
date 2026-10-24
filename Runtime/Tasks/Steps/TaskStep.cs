using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tasks
{
    public abstract class TaskStep
    {
        private readonly int _rawIndex;
        private readonly string _id;

        public abstract int CurrentIndex { get; }

        protected abstract StepData Content { get; set; }
        protected abstract Type ContentType { get; }
        protected abstract TaskStep Next { get; }
        protected abstract IReadOnlyList<TaskStep> Children { get; }

        protected int RawIndex => _rawIndex;
        protected string Id => _id;
        protected bool IsStarted => Content.Started;
        protected bool IsCompleted => Content.Completed;

        public event Action TreeCompleted;
        public event Action StepCompleted;

        protected TaskStep(int index, string id)
        {
            _rawIndex = index;
            _id = id;
        }

        public List<TaskStepData> ReadData()
        {
            List<TaskStepData> result = new List<TaskStepData>
            {
                new TaskStepData
                {
                    Id = _id,
                    Type = ContentType.AssemblyQualifiedName,
                    Json = JsonUtility.ToJson(Content)
                }
            };

            foreach (TaskStep child in Children)
            {
                result.AddRange(child.ReadData());
            }

            return result;
        }

        public void InjectData(List<TaskStepData> steps)
        {
            TaskStepData saved = steps.Find(x => x.Id == _id);
            StepData content = null;
            if (saved != null)
            {
                Type type = Type.GetType(saved.Type) ?? ContentType;
                content = JsonUtility.FromJson(saved.Json, type) as StepData;
            }

            Content = content ?? (StepData)Activator.CreateInstance(ContentType);
            foreach (TaskStep child in Children)
            {
                child.InjectData(steps);
            }
        }

        public void Initialize()
        {
            foreach (TaskStep child in Children)
            {
                child.Initialize();
                child.TreeCompleted += OnChildTreeCompleted;
                child.StepCompleted += OnChildStepCompleted;
            }

            if (Content.Started && !Content.Completed)
            {
                Start();
            }
        }

        public void Start()
        {
            Content.Started = true;
            OnStart();
        }

        public void Destroy()
        {
            OnDestroy();
            foreach (TaskStep child in Children)
            {
                child.Destroy();
            }
        }

        public List<Func<Vector3>> GetTaskPositions()
        {
            List<Func<Vector3>> positions = new List<Func<Vector3>>();
            if (!IsCompleted)
            {
                positions.AddRange(GetStepPositions());
            }

            foreach (TaskStep child in Children)
            {
                positions.AddRange(child.GetTaskPositions());
            }

            return positions;
        }

        protected void Complete()
        {
            if (Content.Completed)
            {
                return;
            }

            Content.Completed = true;
            if (Next != null)
            {
                Next.Start();
            }
            else
            {
                TreeCompleted?.Invoke();
            }

            StepCompleted?.Invoke();
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual IEnumerable<Func<Vector3>> GetStepPositions()
        {
            return Array.Empty<Func<Vector3>>();
        }

        private void OnChildTreeCompleted()
        {
            TreeCompleted?.Invoke();
        }

        private void OnChildStepCompleted()
        {
            StepCompleted?.Invoke();
        }

        [Serializable]
        protected class StepData
        {
            public bool Started;
            public bool Completed;
        }
    }
}
