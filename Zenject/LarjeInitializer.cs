using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Larje.Core.Zenject
{
    public sealed class LarjeInitializer : IInitializable, ILarjeReady, IDisposable
    {
        private bool _ready;
        private readonly List<IInitializableService> _services;
        private readonly List<IAsyncInitializableService> _asyncServices;
        private readonly UniTaskCompletionSource _completion = new UniTaskCompletionSource();
        private readonly CancellationTokenSource _lifetime = new CancellationTokenSource();

        public bool IsReady => _ready;

        public LarjeInitializer(
            [Inject(Source = InjectSources.Local, Optional = true)] List<IInitializableService> services,
            [Inject(Source = InjectSources.Local, Optional = true)] List<IAsyncInitializableService> asyncServices)
        {
            _services = services ?? new List<IInitializableService>();
            _asyncServices = asyncServices ?? new List<IAsyncInitializableService>();
        }

        public void Initialize()
        {
            foreach (IInitializableService service in _services)
            {
                try
                {
                    service.Initialize();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            InitializeAsync(_lifetime.Token).Forget();
        }

        public UniTask WaitAsync()
        {
            return _completion.Task;
        }

        public void Dispose()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
        }

        private async UniTaskVoid InitializeAsync(CancellationToken cancellationToken)
        {
            foreach (IAsyncInitializableService service in _asyncServices)
            {
                try
                {
                    await service.InitializeAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            _ready = true;
            _completion.TrySetResult();
        }
    }
}
