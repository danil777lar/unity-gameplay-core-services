using System;
using Larje.Core.Events;

namespace Larje.Core.GameFlow
{
    public sealed class GameStateService : IGameStateService
    {
        private GameStateId _current;
        private readonly GameStatesConfig _states;
        private readonly IEventBus _eventBus;

        public GameStateId Current => _current;
        public GameStatesConfig States => _states;

        public event Action<GameStateId, GameStateId> Changed;

        public GameStateService(GameStatesConfig states, IEventBus eventBus)
        {
            _states = states;
            _eventBus = eventBus;
        }

        public void Set(GameStateId state)
        {
            if (_current == state)
            {
                return;
            }

            GameStateId previous = _current;
            _current = state;
            Changed?.Invoke(previous, state);
            _eventBus.Publish(new GameStateChangedEvent(previous, state));
        }

        public bool Is(GameStateId state)
        {
            return _current != null && _current == state;
        }
    }
}
