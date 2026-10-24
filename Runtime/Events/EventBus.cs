using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Events
{
    public sealed class EventBus : IEventBus
    {
        private const int HISTORY_SIZE = 64;

        private readonly Dictionary<Type, IHandlerList> _handlers = new Dictionary<Type, IHandlerList>();
        private readonly List<EventRecord> _history = new List<EventRecord>();

        public IReadOnlyList<EventRecord> History => _history;

        public void Publish<T>(T gameEvent) where T : GameEvent
        {
            if (gameEvent == null || !gameEvent.IsValid)
            {
                return;
            }

            Record(gameEvent);
            if (_handlers.TryGetValue(gameEvent.GetType(), out IHandlerList list))
            {
                list.Invoke(gameEvent);
            }
        }

        public IDisposable Subscribe<T>(Action<T> handler) where T : GameEvent
        {
            if (!_handlers.TryGetValue(typeof(T), out IHandlerList list))
            {
                list = new HandlerList<T>();
                _handlers[typeof(T)] = list;
            }

            ((HandlerList<T>)list).Add(handler);
            return new Subscription<T>(this, handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : GameEvent
        {
            if (_handlers.TryGetValue(typeof(T), out IHandlerList list))
            {
                ((HandlerList<T>)list).Remove(handler);
            }
        }

        private void Record(GameEvent gameEvent)
        {
            if (_history.Count >= HISTORY_SIZE)
            {
                _history.RemoveAt(0);
            }

            _history.Add(new EventRecord(Time.unscaledTime, gameEvent.GetType().Name, gameEvent.Source, gameEvent.ToString()));
        }

        private interface IHandlerList
        {
            void Invoke(GameEvent gameEvent);
        }

        private sealed class HandlerList<T> : IHandlerList where T : GameEvent
        {
            private readonly List<Action<T>> _handlers = new List<Action<T>>();
            private readonly List<Action<T>> _snapshot = new List<Action<T>>();

            public void Add(Action<T> handler)
            {
                if (!_handlers.Contains(handler))
                {
                    _handlers.Add(handler);
                }
            }

            public void Remove(Action<T> handler)
            {
                _handlers.Remove(handler);
            }

            public void Invoke(GameEvent gameEvent)
            {
                T typed = (T)gameEvent;
                _snapshot.Clear();
                _snapshot.AddRange(_handlers);
                foreach (Action<T> handler in _snapshot)
                {
                    try
                    {
                        handler(typed);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }
            }
        }

        private sealed class Subscription<T> : IDisposable where T : GameEvent
        {
            private EventBus _bus;
            private readonly Action<T> _handler;

            public Subscription(EventBus bus, Action<T> handler)
            {
                _bus = bus;
                _handler = handler;
            }

            public void Dispose()
            {
                _bus?.Unsubscribe(_handler);
                _bus = null;
            }
        }
    }
}
