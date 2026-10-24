using System;
using System.Collections.Generic;
using Larje.Core.Saves;

namespace Larje.Core.Economy
{
    [Serializable]
    [SaveSection("items")]
    public sealed class ItemsSaveData
    {
        public Dictionary<string, CategoryData> Categories = new Dictionary<string, CategoryData>();

        [Serializable]
        public sealed class CategoryData
        {
            public string Current;
            public List<string> Unlocked = new List<string>();
        }
    }
}
