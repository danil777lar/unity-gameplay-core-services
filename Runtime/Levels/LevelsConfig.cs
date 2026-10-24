using UnityEngine;

namespace Larje.Core.Levels
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "Larje/Core/Levels/Levels Config")]
    public sealed class LevelsConfig : ScriptableObject
    {
        [SerializeField] private bool spawnOnInitialize;
        [SerializeField] private bool useSceneLevelInEditor = true;

        public bool SpawnOnInitialize => spawnOnInitialize;
        public bool UseSceneLevelInEditor => useSceneLevelInEditor;
    }
}
