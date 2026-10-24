using System;
using Zenject;

namespace Larje.Core.Zenject
{
    public sealed class ZenjectServiceProvider : IServiceProvider
    {
        private readonly DiContainer _container;

        public ZenjectServiceProvider(DiContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return _container.TryResolve(serviceType);
        }
    }
}
