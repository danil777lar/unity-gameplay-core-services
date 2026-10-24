using System;
using Larje.Core.Saves;

namespace Larje.Core.Monetization
{
    public sealed class PurchaseContext
    {
        public readonly ISaveService Saves;
        public readonly IServiceProvider Services;

        public PurchaseContext(ISaveService saves, IServiceProvider services)
        {
            Saves = saves;
            Services = services;
        }

        public T Resolve<T>() where T : class
        {
            return Services?.GetService(typeof(T)) as T;
        }
    }
}
