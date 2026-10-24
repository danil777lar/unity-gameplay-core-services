using System.Collections.Generic;

namespace Larje.Core.Locations
{
    public sealed class LocationFilter
    {
        public LocationId Location;
        public int Entry = -1;
        public List<LocationArgId> Args = new List<LocationArgId>();

        public bool Matches(LocationDefinition location, int entry)
        {
            if (location == null)
            {
                return false;
            }

            if (Location != null && location.Id != Location)
            {
                return false;
            }

            if (Entry >= 0 && entry != Entry)
            {
                return false;
            }

            foreach (LocationArgId arg in Args)
            {
                if (!ContainsArg(location, arg))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ContainsArg(LocationDefinition location, LocationArgId arg)
        {
            foreach (LocationArgId locationArg in location.Args)
            {
                if (locationArg == arg)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
