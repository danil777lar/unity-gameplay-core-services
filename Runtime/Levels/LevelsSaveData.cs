using System;
using System.Collections.Generic;
using Larje.Core.Saves;

namespace Larje.Core.Levels
{
    [Serializable]
    [SaveSection("levels")]
    public sealed class LevelsSaveData
    {
        public int Completed;
        public int LastIndex = -1;
        public List<int> RandomQueue = new List<int>();
    }
}
