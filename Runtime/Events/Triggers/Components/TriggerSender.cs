using System;
using UnityEngine;

namespace Larje.Core.Events
{
    public abstract class TriggerSender : MonoBehaviour
    {
        [Header("Trigger")]
        [SerializeField] private TriggerId trigger;
        [SerializeField] private bool sendOnStart = true;

        private float _value;
        private bool _sent;
        private IEventBus _events;

        public TriggerId Trigger => trigger;
        public float Value
        {
            get => isActiveAndEnabled ? _value : 0f;
            protected set
            {
                if (_sent && Mathf.Approximately(_value, value))
                {
                    return;
                }

                _value = value;
                _sent = true;
                if (trigger != null && _events != null)
                {
                    _events.Publish(new TriggerEvent(trigger, _value, gameObject.name));
                }

                ValueChanged?.Invoke(_value);
            }
        }

        public event Action<float> ValueChanged;

#if LARJE_ZENJECT
        [Zenject.Inject]
#endif
        public void Construct(IEventBus events)
        {
            _events = events;
        }

        protected virtual void Start()
        {
            if (sendOnStart)
            {
                _sent = false;
                Value = _value;
            }
        }
    }
}
