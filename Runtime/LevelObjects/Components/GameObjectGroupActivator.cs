using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.LevelObjects
{
    public sealed class GameObjectGroupActivator : MonoBehaviour, ILevelObjectDataUser
    {
        [SerializeField] private List<GameObject> targets = new List<GameObject>();

        public GameObject GameObject => gameObject;
        public Type DataType => typeof(Data);

        public void InjectData(object data)
        {
            bool[] active = ((Data)data).Active;
            for (int i = 0; i < targets.Count && i < active.Length; i++)
            {
                targets[i].SetActive(active[i]);
            }
        }

        public object ReadData()
        {
            bool[] active = new bool[targets.Count];
            for (int i = 0; i < targets.Count; i++)
            {
                active[i] = targets[i].activeSelf;
            }

            return new Data { Active = active };
        }

        [Serializable]
        public sealed class Data
        {
            public bool[] Active;
        }
    }
}
