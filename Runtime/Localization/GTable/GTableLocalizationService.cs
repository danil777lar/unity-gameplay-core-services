using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Larje.Core.Saves;
using UnityEngine;
using UnityEngine.Networking;

namespace Larje.Core.Localization
{
    public sealed class GTableLocalizationService : ILocalizationService, IAsyncInitializableService
    {
        private bool _isLoaded;
        private float _progress;
        private string _language;
        private readonly GTableLocalizationConfig _config;
        private readonly ISaveService _saves;
        private readonly HashSet<string> _languages = new HashSet<string>();
        private readonly Dictionary<string, Dictionary<string, string>> _entries = new Dictionary<string, Dictionary<string, string>>();

        public bool IsLoaded => _isLoaded;
        public float Progress => _progress;
        public string Language => _language;
        public IReadOnlyCollection<string> Languages => _languages;

        public event Action Changed;

        public GTableLocalizationService(GTableLocalizationConfig config, ISaveService saves)
        {
            _config = config;
            _saves = saves;
        }

        public async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            _language = ResolveLanguage();
            LoadCache();

            if (_config.LoadFromWeb && !string.IsNullOrEmpty(_config.Url))
            {
                await LoadFromWebAsync(cancellationToken);
            }

            RebuildLanguages();
            _progress = 1f;
            _isLoaded = true;
            Changed?.Invoke();
        }

        public string Get(string key)
        {
            return TryGet(key, out string text) ? text : key;
        }

        public bool TryGet(string key, out string text)
        {
            text = null;
            if (string.IsNullOrEmpty(key) || !_entries.TryGetValue(key, out Dictionary<string, string> values))
            {
                return false;
            }

            if (values.TryGetValue(_language, out text) && !string.IsNullOrEmpty(text))
            {
                return true;
            }

            return values.TryGetValue(_config.DefaultLanguage, out text);
        }

        public void SetLanguage(string languageCode)
        {
            string code = languageCode.ToLowerInvariant();
            if (code == _language)
            {
                return;
            }

            _language = code;
            _saves.Get<LocalizationSettingsData>().Language = code;
            _saves.SaveSystem();
            Changed?.Invoke();
        }

        private string ResolveLanguage()
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(_config.EditorLanguageOverride))
            {
                return _config.EditorLanguageOverride.ToLowerInvariant();
            }
#endif
            string saved = _saves.Get<LocalizationSettingsData>().Language;
            return string.IsNullOrEmpty(saved) ? LanguageCodes.FromSystem() : saved;
        }

        private void LoadCache()
        {
            foreach (GTableLocalizationConfig.TableCache cache in _config.Cache)
            {
                if (cache.Asset != null)
                {
                    GTableParser.Parse(cache.Asset.text, _entries);
                }
            }
        }

        private async UniTask LoadFromWebAsync(CancellationToken cancellationToken)
        {
            int count = _config.Tables.Count;
            for (int i = 0; i < count; i++)
            {
                string table = _config.Tables[i];
                using (UnityWebRequest request = UnityWebRequest.Get(_config.GetTableUrl(table)))
                {
                    request.timeout = Mathf.CeilToInt(_config.Timeout);
                    try
                    {
                        await request.SendWebRequest().ToUniTask(Cysharp.Threading.Tasks.Progress.Create<float>(x => _progress = (i + x) / count), cancellationToken: cancellationToken);
                        GTableParser.Parse(request.downloadHandler.text, _entries);
                    }
                    catch (UnityWebRequestException exception)
                    {
                        Debug.LogWarning($"GTableLocalizationService: failed to load table '{table}': {exception.Message}");
                    }
                }
            }
        }

        private void RebuildLanguages()
        {
            _languages.Clear();
            foreach (Dictionary<string, string> values in _entries.Values)
            {
                _languages.UnionWith(values.Keys);
            }
        }
    }
}
