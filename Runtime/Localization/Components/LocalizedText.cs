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

#if LARJE_ZENJECT
        [Zenject.Inject]
#endif
        public void Construct(ILocalizationService localization)
        {
            _localization = localization;
            if (isActiveAndEnabled)
            {
                _localization.Changed += Refresh;
                Refresh();
            }
        }

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            if (_localization != null)
            {
                _localization.Changed += Refresh;
                Refresh();
            }
        }

        private void OnDisable()
        {
            if (_localization != null)
            {
                _localization.Changed -= Refresh;
            }
        }

        private void Refresh()
        {
            if (_text == null || _localization == null)
            {
                return;
            }

            _text.text = prefix + _localization.Get(key) + suffix;
        }
    }
}
