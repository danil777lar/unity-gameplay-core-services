using System;
using Larje.Core.Saves;

namespace Larje.Core.Debugging
{
    [Serializable]
    [SaveSection("debug_console", SaveScope.System)]
    public sealed class DebugConsoleSettingsData
    {
        public bool OverlayVisible;
        public float OverlayOpacity = 1f;
    }
}
