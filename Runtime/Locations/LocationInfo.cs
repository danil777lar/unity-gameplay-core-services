using System;
using System.Collections.Generic;
using Larje.Core.Events;
using UnityEngine;

namespace Larje.Core.Locations
{
    [Serializable]
    public sealed class LocationInfo
    {
        [SerializeField, HideInInspector] private string inspectorName;

        [Header("Location")]
        [SerializeField] private LocationId id;
        [SerializeField] private string sceneName;
        [SerializeField] private List<LocationArgId> args = new List<LocationArgId>();

        [Header("On Loaded")]
        [SerializeField] private List<LocationAction> actions = new List<LocationAction>();
        [SerializeField] private List<TriggerId> triggers = new List<TriggerId>();

        public LocationId Id => id;
        public string SceneName => sceneName;
        public IReadOnlyList<LocationArgId> Args => args;
        public IReadOnlyList<LocationAction> Actions => actions;
        public IReadOnlyList<TriggerId> Triggers => triggers;

        public void Validate()
        {
            inspectorName = id != null ? id.name : "None";
        }
    }
}
