using Larje.Core.GameFlow;

namespace Larje.Core.Debugging
{
    public sealed class GameFlowDebugCommands : IDebugCommandProvider
    {
        private readonly IGameStateService _gameState;
        private readonly ISceneLoader _scenes;

        public GameFlowDebugCommands(IGameStateService gameState, ISceneLoader scenes)
        {
            _gameState = gameState;
            _scenes = scenes;
        }

        [DebugCommand("Game Flow", "Print current game state")]
        public string CurrentState()
        {
            return _gameState.Current != null ? _gameState.Current.Key : "None";
        }

        [DebugCommand("Game Flow", "Force game state")]
        public string SetState(GameStateId state)
        {
            _gameState.Set(state);
            return "State " + (state != null ? state.Key : "None");
        }

        [DebugCommand("Game Flow", "Load scene by name")]
        public string LoadScene(string scene)
        {
            _scenes.Load(scene);
            return "Loading " + scene;
        }
    }
}
