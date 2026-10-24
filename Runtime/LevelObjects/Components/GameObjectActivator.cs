using System;
using UnityEngine;

namespace Larje.Core.LevelObjects
{
    public sealed class GameObjectActivator : MonoBehaviour, ILevelObjectDataUser
    {
        [SerializeField] private GameObject target;

        public GameObject GameObject => gameObject;
        public Type DataType => typeof(Data);

        public void InjectData(object data)
        {
            target.SetActive(((Data)data).Active);
        }

        public object ReadData()
        {
            return new Data { Active = target.activeSelf };
        }

        [Serializable]
        public sealed class Data
        {
            public bool Active;
        }
    }
}
