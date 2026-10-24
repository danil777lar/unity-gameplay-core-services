using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Larje.Core.Levels
{
    public sealed class PrefabLevelSource : ILevelSource
    {
        private readonly PrefabLevelsConfig _config;
        private readonly IObjectInjector _injector;

        public int Count => _config.Levels.Count;

        public PrefabLevelSource(PrefabLevelsConfig config, IObjectInjector injector)
        {
            _config = config;
            _injector = injector;
        }

        public bool IsRandomizable(int index)
        {
            return _config.Levels[index].AddToRandom;
        }

        public UniTask<GameObject> InstantiateAsync(int index, Transform parent, CancellationToken cancellationToken)
        {
            GameObject prefab = _config.Levels[index].Prefab;
            return UniTask.FromResult(prefab != null ? _injector.Instantiate(prefab, parent) : null);
        }

        public void Release(GameObject instance)
        {
            Object.Destroy(instance);
        }
    }
}
