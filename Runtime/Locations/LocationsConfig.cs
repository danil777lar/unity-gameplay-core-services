using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Locations
{
    [CreateAssetMenu(fileName = "LocationsConfig", menuName = "Larje/Core/Locations/Locations Config")]
    public sealed class LocationsConfig : ScriptableObject
    {
        [Header("Defaults")]
        [SerializeField] private LocationId defaultLocation;
        [SerializeField] private int defaultEntry;
        [SerializeField] private bool loadOnInitialize = true;

        [Header("Transition")]
        [SerializeField] private float transitionDuration = 0.5f;

        [Header("Locations")]
        [SerializeField] private List<LocationInfo> locations = new List<LocationInfo>();

        public LocationId DefaultLocation => defaultLocation;
        public int DefaultEntry => defaultEntry;
        public bool LoadOnInitialize => loadOnInitialize;
        public float TransitionDuration => transitionDuration;
        public IReadOnlyList<LocationInfo> Locations => locations;

        public LocationInfo Find(LocationId id)
        {
            return locations.Find(x => x.Id == id);
        }

        public LocationInfo FindByKey(string key)
        {
            return locations.Find(x => x.Id != null && x.Id.Key == key);
        }

        private void OnValidate()
        {
            locations.ForEach(x => x.Validate());
        }
    }
}
