using UnityEngine;

namespace Larje.Core.Locations
{
    public abstract class LocationAction : ScriptableObject
    {
        public abstract void Apply(LocationDefinition location);
    }
}
