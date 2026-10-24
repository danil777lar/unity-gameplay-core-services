using TMPro;
using UnityEngine;

namespace Larje.Core.Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class LocalizedText : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private string key;
        [SerializeField] private string prefix;
        [SerializeField] private string suffix;

        private bool _subscribed;
        private TMP_Text _text;
        private ILocalizationService _localization;

        public string Key
        {
            get => key;
            set
            {
                key = value;
                Refresh();
            }
        }

        private TMP_Text Text
        {
            get
            {
                if (_text == null)
                {
                    _text = GetComponent<TMP_Text>();
                }

                return _text;
            }
        }

#if LARJE_ZENJECT
        [Zenject.Inject]
#endif
        public void Construct(ILocalizationService localization)
        {
            _localization = localization;
            Subscribe();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            if (_subscribed)
            {
                _localization.Changed -= Refresh;
                _subscribed = false;
            }
        }

        private void Subscribe()
        {
            if (_subscribed || _localization == null || !isActiveAndEnabled)
            {
                return;
            }

            _localization.Changed += Refresh;
            _subscribed = true;
            Refresh();
        }

        private void Refresh()
        {
            if (_localization != null)
            {
                Text.text = prefix + _localization.Get(key) + suffix;
            }
        }
    }
}
