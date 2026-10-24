using System;
using Larje.Core.Saves;

namespace Larje.Core.Locations
{
    [Serializable]
    [SaveSection("location")]
    public sealed class LocationSaveData
    {
        public bool Initialized;
        public string Location;
        public int Entry;
    }
}
