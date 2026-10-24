using Larje.Core.Events;

namespace Larje.Core.GameFlow
{
    public sealed class GameStateChangedEvent : GameEvent
    {
        public readonly GameStateId Previous;
        public readonly GameStateId Current;

        public GameStateChangedEvent(GameStateId previous, GameStateId current) : base(nameof(GameStateService))
        {
            Previous = previous;
            Current = current;
        }

        public override string ToString()
        {
            return $"GameState {(Previous != null ? Previous.name : "None")} -> {(Current != null ? Current.name : "None")}";
        }
    }
}
