using UnityEngine;

namespace Larje.Core.GameFlow
{
    [CreateAssetMenu(fileName = "AppFlowConfig", menuName = "Larje/Core/Game Flow/App Flow Config")]
    public sealed class AppFlowConfig : ScriptableObject
    {
        [Header("Flow")]
        [SerializeField] private bool runOnInitialize = true;
        [SerializeField] private string splashScene;
        [SerializeField] private float splashDuration = 2f;
        [SerializeField] private string menuScene;

        [Header("Application")]
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private bool neverSleep = true;
        [SerializeField] private bool logsInBuild;

        public bool RunOnInitialize => runOnInitialize;
        public string SplashScene => splashScene;
        public float SplashDuration => splashDuration;
        public string MenuScene => menuScene;
        public int TargetFrameRate => targetFrameRate;
        public bool NeverSleep => neverSleep;
        public bool LogsInBuild => logsInBuild;
    }
}
