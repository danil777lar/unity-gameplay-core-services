using Larje.Core.Saves;
using UnityEngine;

namespace Larje.Core.Haptics
{
    public sealed class HapticService : IHapticService
    {
        private readonly ISaveService _saves;

        public bool Enabled
        {
            get => _saves.Get<HapticSettingsData>().Enabled;
            set
            {
                _saves.Get<HapticSettingsData>().Enabled = value;
                _saves.SaveSystem();
            }
        }

        public HapticService(ISaveService saves)
        {
            _saves = saves;
        }

        public void Play(HapticId haptic)
        {
            if (!Enabled)
            {
                return;
            }

#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        }
    }
}
