using UnityEngine;

namespace Larje.Core.Locations
{
    public sealed class LocationEntry : MonoBehaviour, ILocationEntry
    {
        [SerializeField] private int id;

        private bool _registered;
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
            Register();
        }

        private void OnEnable()
        {
            Register();
        }

        private void OnDisable()
        {
            if (_registered)
            {
                _locations.RemoveEntry(this);
                _registered = false;
            }
        }

        private void Register()
        {
            if (!_registered && _locations != null && isActiveAndEnabled)
            {
                _locations.AddEntry(this);
                _registered = true;
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
