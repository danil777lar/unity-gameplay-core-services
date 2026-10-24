using UnityEngine;

namespace Larje.Core.Saves
{
    [CreateAssetMenu(fileName = "SaveConfig", menuName = "Larje/Core/Saves/Save Config")]
    public sealed class SaveConfig : ScriptableObject
    {
        [Header("Slots")]
        [SerializeField] private string defaultSlot = "default";
        [SerializeField] private bool loadGameOnInitialize = true;
        [SerializeField] private int version = 1;

        [Header("Autosave")]
        [SerializeField] private bool saveOnPause = true;
        [SerializeField] private bool saveOnQuit = true;

        [Header("Encryption")]
        [SerializeField] private bool encrypt;
        [SerializeField] private string aesKey;
        [SerializeField] private string aesIv;

        public string DefaultSlot => defaultSlot;
        public bool LoadGameOnInitialize => loadGameOnInitialize;
        public int Version => version;
        public bool SaveOnPause => saveOnPause;
        public bool SaveOnQuit => saveOnQuit;
        public bool Encrypt => encrypt && !string.IsNullOrEmpty(aesKey) && !string.IsNullOrEmpty(aesIv);
        public string AesKey => aesKey;
        public string AesIv => aesIv;

        [ContextMenu("Generate AES Key")]
        private void GenerateAesKey()
        {
            aesKey = AesUtility.GenerateKey();
            aesIv = AesUtility.GenerateIv();
        }
    }
}
