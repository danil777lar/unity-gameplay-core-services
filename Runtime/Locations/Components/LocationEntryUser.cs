using UnityEngine;

namespace Larje.Core.Locations
{
    public sealed class LocationEntryUser : MonoBehaviour
    {
        [SerializeField] private bool permitted = true;

        private ILocationService _locations;

#if LARJE_ZENJECT
        [Zenject.Inject]
#endif
        public void Construct(ILocationService locations)
        {
            _locations = locations;
        }

        private void Start()
        {
            if (permitted && _locations != null && _locations.TryGetEntry(out ILocationEntry entry))
            {
                transform.position = entry.Position;
                transform.forward = entry.Direction;
            }
        }
    }
}
