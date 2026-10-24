using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Locations
{
    public sealed class LocationArgActivator : MonoBehaviour
    {
        [SerializeField] private bool permitted = true;
        [SerializeField] private Option[] options = new Option[0];

        private ILocationService _locations;

#if LARJE_ZENJECT
        [Zenject.Inject]
#endif
        public void Construct(ILocationService locations)
        {
            _locations = locations;
        }

        public void Apply(IReadOnlyList<LocationArgId> args)
        {
            if (!permitted)
            {
                return;
            }

            foreach (Option option in options)
            {
                if (option.Target != null)
                {
                    option.Target.SetActive(Contains(args, option.Arg));
                }
            }
        }

        private void Start()
        {
            if (_locations != null && _locations.Current != null)
            {
                Apply(_locations.Current.Args);
            }
        }

        private bool Contains(IReadOnlyList<LocationArgId> args, LocationArgId arg)
        {
            foreach (LocationArgId candidate in args)
            {
                if (candidate == arg)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnValidate()
        {
            foreach (Option option in options)
            {
                option.Validate();
            }
        }

        [Serializable]
        private sealed class Option
        {
            [SerializeField, HideInInspector] private string inspectorName;

            public LocationArgId Arg;
            public GameObject Target;

            public void Validate()
            {
                inspectorName = Arg != null ? Arg.name : "None";
            }
        }
    }
}
