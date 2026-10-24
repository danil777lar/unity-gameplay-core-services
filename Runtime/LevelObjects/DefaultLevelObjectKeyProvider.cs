using UnityEngine.SceneManagement;

namespace Larje.Core.LevelObjects
{
    public sealed class DefaultLevelObjectKeyProvider : ILevelObjectKeyProvider
    {
        public string GetKeyPrefix()
        {
            return SceneManager.GetActiveScene().name;
        }
    }
}
