using System;
using Larje.Core.Saves;

namespace Larje.Core.Localization
{
    [Serializable]
    [SaveSection("localization_settings", SaveScope.System)]
    public sealed class LocalizationSettingsData
    {
        public string Language;
    }
}
