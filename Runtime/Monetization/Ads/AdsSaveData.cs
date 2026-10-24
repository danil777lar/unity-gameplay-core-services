using System;
using Larje.Core.Saves;

namespace Larje.Core.Monetization
{
    [Serializable]
    [SaveSection("ads")]
    public sealed class AdsSaveData
    {
        public bool NoAds;
    }
}
