using System;
using System.Collections.Generic;

namespace Larje.Core.Saves
{
    [Serializable]
    public sealed class SaveMeta
    {
        public string Slot;
        public string Date;
        public int Version;
        public Dictionary<string, string> Values = new Dictionary<string, string>();
    }
}
