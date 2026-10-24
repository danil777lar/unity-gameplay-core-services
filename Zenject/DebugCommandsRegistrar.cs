using System;
using System.Collections.Generic;
using Larje.Core.Debugging;
using Zenject;

namespace Larje.Core.Zenject
{
    public sealed class DebugCommandsRegistrar : IInitializable, IDisposable
    {
        private readonly IDebugCommandRegistry _registry;
        private readonly List<IDebugCommandProvider> _providers;

        public DebugCommandsRegistrar(
            IDebugCommandRegistry registry,
            [Inject(Source = InjectSources.Local, Optional = true)] List<IDebugCommandProvider> providers)
        {
            _registry = registry;
            _providers = providers ?? new List<IDebugCommandProvider>();
        }

        public void Initialize()
        {
            foreach (IDebugCommandProvider provider in _providers)
            {
                _registry.Register(provider);
            }
        }

        public void Dispose()
        {
            foreach (IDebugCommandProvider provider in _providers)
            {
                _registry.Unregister(provider);
            }
        }
    }
}
