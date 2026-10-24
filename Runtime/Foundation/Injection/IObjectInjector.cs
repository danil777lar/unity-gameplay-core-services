using UnityEngine;

namespace Larje.Core
{
    public interface IObjectInjector
    {
        void Inject(GameObject target);
        GameObject Instantiate(GameObject prefab, Transform parent);
    }
}
