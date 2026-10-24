using System;
using System.Collections.Generic;
using Larje.Core.Saves;

namespace Larje.Core.Tasks
{
    [Serializable]
    [SaveSection("daily_tasks")]
    public sealed class DailyTasksSaveData
    {
        public string LastReset;
        public List<string> Selected = new List<string>();
    }
}
