using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Larje.Core.Levels
{
    public sealed class AddressableLevelSource : ILevelSource
    {
        private readonly AddressableLevelsConfig _config;
        private readonly IObjectInjector _injector;

        public int Count => _config.Levels.Count;

        public AddressableLevelSource(AddressableLevelsConfig config, IObjectInjector injector)
        {
            _config = config;
            _injector = injector;
        }

        public bool IsRandomizable(int index)
        {
            return _config.Levels[index].AddToRandom;
        }

        public async UniTask<GameObject> InstantiateAsync(int index, Transform parent, CancellationToken cancellationToken)
        {
            GameObject instance = await Addressables.InstantiateAsync(_config.Levels[index].Prefab, parent).ToUniTask(cancellationToken: cancellationToken);
            if (instance != null)
            {
                _injector.Inject(instance);
            }

            return instance;
        }

        public void Release(GameObject instance)
        {
            if (!Addressables.ReleaseInstance(instance))
            {
                Object.Destroy(instance);
            }
        }
    }
}
