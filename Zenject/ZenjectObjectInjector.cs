using UnityEngine;
using Zenject;

namespace Larje.Core.Zenject
{
    public sealed class ZenjectObjectInjector : IObjectInjector
    {
        private readonly DiContainer _container;

        public ZenjectObjectInjector(DiContainer container)
        {
            _container = container;
        }

        public void Inject(GameObject target)
        {
            _container.InjectGameObject(target);
        }

        public GameObject Instantiate(GameObject prefab, Transform parent)
        {
            return _container.InstantiatePrefab(prefab, parent);
        }
    }
}
