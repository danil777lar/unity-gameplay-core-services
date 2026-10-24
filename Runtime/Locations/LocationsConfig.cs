using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Locations
{
    [CreateAssetMenu(fileName = "LocationsConfig", menuName = "Larje/Core/Locations/Locations Config")]
    public sealed class LocationsConfig : ScriptableObject, IConfigValidatable
    {
        [Header("Defaults")]
        [SerializeField] private LocationId defaultLocation;
        [SerializeField] private int defaultEntry;
        [SerializeField] private bool loadOnInitialize = true;

        [Header("Transition")]
        [SerializeField] private float transitionDuration = 0.5f;

        [Header("Locations")]
        [SerializeField] private List<LocationDefinition> locations = new List<LocationDefinition>();

        public LocationId DefaultLocation => defaultLocation;
        public int DefaultEntry => defaultEntry;
        public bool LoadOnInitialize => loadOnInitialize;
        public float TransitionDuration => transitionDuration;
        public IReadOnlyList<LocationDefinition> Locations => locations;

        public LocationDefinition Find(LocationId id)
        {
            return locations.Find(x => x.Id == id);
        }

        public LocationDefinition FindByKey(string key)
        {
            return locations.Find(x => x.Id != null && x.Id.Key == key);
        }

        private void OnValidate()
        {
            locations.ForEach(x => x.Validate());
        }

        public void Validate(List<string> problems)
        {
            if (defaultLocation == null || Find(defaultLocation) == null)
            {
                problems.Add("Default location is missing or not configured");
            }

            foreach (LocationDefinition location in locations)
            {
                if (location.Id == null)
                {
                    problems.Add("Location entry has no LocationId");
                }

                if (string.IsNullOrEmpty(location.SceneName))
                {
                    problems.Add($"Location '{(location.Id != null ? location.Id.name : "None")}' has no scene");
                }
            }
        }
    }
}
