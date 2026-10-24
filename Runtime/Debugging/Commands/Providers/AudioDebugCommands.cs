using Larje.Core.Audio;

namespace Larje.Core.Debugging
{
    public sealed class AudioDebugCommands : IDebugCommandProvider
    {
        private readonly ISoundService _sounds;

        public AudioDebugCommands(ISoundService sounds)
        {
            _sounds = sounds;
        }

        [DebugCommand("Audio", "Play sound")]
        public string Play(SoundId sound)
        {
            return _sounds.Play(sound) != null ? "Playing " + sound.Key : "Sound not configured";
        }

        [DebugCommand("Audio", "Stop all sounds")]
        public string StopAll()
        {
            _sounds.StopAll();
            return "Stopped";
        }

        [DebugCommand("Audio", "Set channel volume 0..1")]
        public string SetChannelVolume(SoundChannelId channel, float volume)
        {
            _sounds.SetChannelVolume(channel, volume);
            return $"{channel.Key} = {_sounds.GetChannelVolume(channel)}";
        }

        [DebugCommand("Audio", "Enable or disable all audio")]
        public string SetEnabled(bool enabled)
        {
            _sounds.Enabled = enabled;
            return enabled ? "Audio on" : "Audio off";
        }
    }
}
