namespace Larje.Core.Audio
{
    public interface ISystemVolumeService
    {
        bool Supported { get; }

        float GetVolume();
        void SetVolume(float volume);
    }
}
