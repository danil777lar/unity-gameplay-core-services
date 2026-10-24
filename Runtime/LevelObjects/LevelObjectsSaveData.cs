using System;
using System.Collections.Generic;
using Larje.Core.Saves;

namespace Larje.Core.LevelObjects
{
    [Serializable]
    [SaveSection("level_objects")]
    public sealed class LevelObjectsSaveData
    {
        public Dictionary<string, Dictionary<string, string>> Objects = new Dictionary<string, Dictionary<string, string>>();
    }
}
