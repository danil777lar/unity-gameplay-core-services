using System;
using System.Collections.Generic;
using Larje.Core.Saves;

namespace Larje.Core.Economy
{
    [Serializable]
    [SaveSection("currency")]
    public sealed class CurrencySaveData
    {
        public Dictionary<string, Dictionary<string, int>> Amounts = new Dictionary<string, Dictionary<string, int>>();
        public List<string> DefaultsApplied = new List<string>();
    }
}
