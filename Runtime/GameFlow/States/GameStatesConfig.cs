using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.GameFlow
{
    [CreateAssetMenu(fileName = "GameStatesConfig", menuName = "Larje/Core/Game Flow/Game States Config")]
    public sealed class GameStatesConfig : ScriptableObject, IConfigValidatable
    {
        [Header("Flow")]
        [SerializeField] private GameStateId splash;
        [SerializeField] private GameStateId loading;
        [SerializeField] private GameStateId transition;
        [SerializeField] private GameStateId menu;

        [Header("Gameplay")]
        [SerializeField] private GameStateId playing;
        [SerializeField] private GameStateId paused;
        [SerializeField] private GameStateId win;
        [SerializeField] private GameStateId fail;

        [Header("Custom")]
        [SerializeField] private List<GameStateId> custom = new List<GameStateId>();

        public GameStateId Splash => splash;
        public GameStateId Loading => loading;
        public GameStateId Transition => transition;
        public GameStateId Menu => menu;
        public GameStateId Playing => playing;
        public GameStateId Paused => paused;
        public GameStateId Win => win;
        public GameStateId Fail => fail;
        public IReadOnlyList<GameStateId> Custom => custom;

        public IEnumerable<GameStateId> All
        {
            get
            {
                GameStateId[] standard = { splash, loading, transition, menu, playing, paused, win, fail };
                foreach (GameStateId state in standard)
                {
                    if (state != null)
                    {
                        yield return state;
                    }
                }

                foreach (GameStateId state in custom)
                {
                    if (state != null)
                    {
                        yield return state;
                    }
                }
            }
        }

        public void Validate(List<string> problems)
        {
            GameStateId[] required = { splash, loading, transition, menu, playing, paused, win, fail };
            string[] names = { "Splash", "Loading", "Transition", "Menu", "Playing", "Paused", "Win", "Fail" };
            for (int i = 0; i < required.Length; i++)
            {
                if (required[i] == null)
                {
                    problems.Add($"Game state '{names[i]}' is not assigned");
                }
            }
        }
    }
}
