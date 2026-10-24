using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Tasks
{
    [CreateAssetMenu(fileName = "DailyTasksConfig", menuName = "Larje/Core/Tasks/Daily Tasks Config")]
    public sealed class DailyTasksConfig : ScriptableObject
    {
        [SerializeField, Min(1)] private int tasksPerDay = 3;
        [SerializeField] private List<TaskConfig> pool = new List<TaskConfig>();

        public int TasksPerDay => tasksPerDay;
        public IReadOnlyList<TaskConfig> Pool => pool;
    }
}
