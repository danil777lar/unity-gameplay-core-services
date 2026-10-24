using System;
using UnityEngine;

namespace Larje.Core.Economy
{
    [Serializable]
    public class ItemDefinition
    {
        [SerializeField] private string key;
        [SerializeField] private Sprite icon;

        public string Key => key;
        public Sprite Icon => icon;
    }
}
