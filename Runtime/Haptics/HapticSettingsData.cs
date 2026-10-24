using System;
using Larje.Core.Saves;

namespace Larje.Core.Haptics
{
    [Serializable]
    [SaveSection("haptic_settings", SaveScope.System)]
    public sealed class HapticSettingsData
    {
        public bool Enabled = true;
    }
}
