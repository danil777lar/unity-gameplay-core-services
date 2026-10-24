using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tasks
{
    public abstract class TaskConfig : ScriptableObject
    {
        [Header("Info")]
        [SerializeField] protected string nameKey;
        [SerializeField] protected string descriptionKey;
        [SerializeField] protected Sprite icon;

        [Header("Logic")]
        [SerializeField, Min(1)] protected int weight = 1;

        [Header("Rewards")]
        [SerializeField] protected List<TaskRewardConfig> rewards = new List<TaskRewardConfig>();

        public virtual string TaskId => name;
        public virtual string NameKey => nameKey;
        public virtual string DescriptionKey => descriptionKey;
        public virtual Sprite Icon => icon;
        public virtual int Weight => weight;
        public IReadOnlyList<TaskRewardConfig> Rewards => rewards;

        public virtual bool IsAvailable(TaskContext context)
        {
            return true;
        }

        public virtual void GetProgress(TaskContext context, out int current, out int target)
        {
            current = context.GetData(TaskId).Status == TaskStatus.Completed ? 1 : 0;
            target = 1;
        }

        public abstract TaskProcessor CreateProcessor(TaskContext context);

        [ContextMenu("Set Localization Keys")]
        protected virtual void SetLocalizationKeys()
        {
            string formatted = name.Replace(" ", "_").ToLowerInvariant();
            nameKey = formatted + "_name";
            descriptionKey = formatted + "_desc";
        }
    }
}
