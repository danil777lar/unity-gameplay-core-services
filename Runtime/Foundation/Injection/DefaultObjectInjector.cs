using UnityEngine;

namespace Larje.Core
{
    public sealed class DefaultObjectInjector : IObjectInjector
    {
        public void Inject(GameObject target)
        {
        }

        public GameObject Instantiate(GameObject prefab, Transform parent)
        {
            return Object.Instantiate(prefab, parent);
        }
    }
}
