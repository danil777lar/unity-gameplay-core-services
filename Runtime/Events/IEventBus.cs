using System;
using System.Collections.Generic;

namespace Larje.Core.Events
{
    public interface IEventBus
    {
        IReadOnlyList<EventRecord> History { get; }

        void Publish<T>(T gameEvent) where T : GameEvent;
        IDisposable Subscribe<T>(Action<T> handler) where T : GameEvent;
        void Unsubscribe<T>(Action<T> handler) where T : GameEvent;
    }
}
