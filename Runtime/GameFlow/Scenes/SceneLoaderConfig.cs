using UnityEngine;

namespace Larje.Core.GameFlow
{
    [CreateAssetMenu(fileName = "SceneLoaderConfig", menuName = "Larje/Core/Game Flow/Scene Loader Config")]
    public sealed class SceneLoaderConfig : ScriptableObject
    {
        [SerializeField] private float minimumDelay = 1f;
        [SerializeField] private float progressSmoothSpeed = 0.5f;

        public float MinimumDelay => minimumDelay;
        public float ProgressSmoothSpeed => progressSmoothSpeed;
    }
}
