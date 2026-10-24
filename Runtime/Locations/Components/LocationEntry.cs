using UnityEngine;

namespace Larje.Core.Locations
{
    public sealed class LocationEntry : MonoBehaviour, ILocationEntry
    {
        [SerializeField] private int id;

        private ILocationService _locations;

        public int Id => id;
        public Vector3 Position => transform.position;
        public Vector3 Direction => transform.forward;

#if LARJE_ZENJECT
        [Zenject.Inject]
#endif
        public void Construct(ILocationService locations)
        {
            _locations = locations;
            if (isActiveAndEnabled)
            {
                _locations.AddEntry(this);
            }
        }

        private void OnEnable()
        {
            if (_locations != null)
            {
                _locations.AddEntry(this);
            }
        }

        private void OnDisable()
        {
            if (_locations != null)
            {
                _locations.RemoveEntry(this);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.25f);
            Gizmos.DrawRay(transform.position, transform.forward);
        }
    }
}
