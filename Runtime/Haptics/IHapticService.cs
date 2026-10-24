namespace Larje.Core.Haptics
{
    public interface IHapticService
    {
        bool Enabled { get; set; }

        void Play(HapticId haptic);
    }
}
