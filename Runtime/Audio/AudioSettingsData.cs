using System;
using System.Collections.Generic;
using Larje.Core.Saves;

namespace Larje.Core.Audio
{
    [Serializable]
    [SaveSection("audio_settings", SaveScope.System)]
    public sealed class AudioSettingsData
    {
        public bool Enabled = true;
        public Dictionary<string, float> ChannelVolumes = new Dictionary<string, float>();
    }
}
