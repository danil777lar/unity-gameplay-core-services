using UnityEngine;

namespace Larje.Core.Tasks
{
    public abstract class TaskRewardConfig : ScriptableObject
    {
        public virtual int Amount => 0;

        public abstract void Give(TaskContext context, int multiplier);
    }
}
