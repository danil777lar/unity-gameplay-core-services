namespace Larje.Core.Saves
{
    public static class SaveStorageFactory
    {
        public static ISaveStorage CreateDefault()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return new PlayerPrefsSaveStorage();
#else
            return new FileSaveStorage();
#endif
        }
    }
}
