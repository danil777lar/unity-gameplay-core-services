using System.Collections.Generic;

namespace Larje.Core.Audio
{
    public interface ISoundService
    {
        bool Enabled { get; set; }
        IReadOnlyList<Sound> Playing { get; }
        SoundConfig Config { get; }

        Sound Play(SoundId id);
        void Stop(Sound sound);
        void StopAll(object target);
        void StopAll();
        float GetChannelVolume(SoundChannelId channel);
        void SetChannelVolume(SoundChannelId channel, float volume);
    }
}
