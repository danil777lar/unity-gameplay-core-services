using System;
using System.Collections.Generic;

namespace Larje.Core.Localization
{
    public interface ILocalizationService
    {
        bool IsLoaded { get; }
        float Progress { get; }
        string Language { get; }
        IReadOnlyCollection<string> Languages { get; }

        event Action Changed;

        string Get(string key);
        bool TryGet(string key, out string text);
        void SetLanguage(string languageCode);
    }
}
