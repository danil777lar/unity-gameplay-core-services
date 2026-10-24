using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Larje.Core.Levels
{
    public interface ILevelSource
    {
        int Count { get; }

        bool IsRandomizable(int index);
        UniTask<GameObject> InstantiateAsync(int index, Transform parent, CancellationToken cancellationToken);
        void Release(GameObject instance);
    }
}
