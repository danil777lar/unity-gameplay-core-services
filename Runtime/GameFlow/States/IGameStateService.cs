using System;

namespace Larje.Core.GameFlow
{
    public interface IGameStateService
    {
        GameStateId Current { get; }
        GameStatesConfig States { get; }

        event Action<GameStateId, GameStateId> Changed;

        void Set(GameStateId state);
        bool Is(GameStateId state);
    }
}
