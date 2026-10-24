using System;
using UnityEngine;
using UnityEngine.Events;

namespace Larje.Core.Events
{
    public sealed class TriggerReceiver : MonoBehaviour
    {
        [Header("Trigger")]
        [SerializeField] private TriggerId trigger;

        [Header("Float")]
        [SerializeField] private UnityEvent<float> valueChanged = new UnityEvent<float>();
        [SerializeField] private UnityEvent<float> valueChangedReversed = new UnityEvent<float>();

        [Header("Bool")]
        [SerializeField] private UnityEvent<bool> stateChanged = new UnityEvent<bool>();
        [SerializeField] private UnityEvent<bool> stateChangedReversed = new UnityEvent<bool>();
        [SerializeField] private UnityEvent activated = new UnityEvent();

        private IDisposable _subscription;

#if LARJE_ZENJECT
        [Zenject.Inject]
#endif
        public void Construct(IEventBus events)
        {
            _subscription?.Dispose();
            _subscription = events.Subscribe<TriggerEvent>(OnTrigger);
        }

        public void Receive(float value)
        {
            valueChanged.Invoke(value);
            valueChangedReversed.Invoke(1f - value);

            bool state = value >= 1f;
            stateChanged.Invoke(state);
            stateChangedReversed.Invoke(!state);
            if (state)
            {
                activated.Invoke();
            }
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }

        private void OnTrigger(TriggerEvent triggerEvent)
        {
            if (triggerEvent.Trigger == trigger)
            {
                Receive(triggerEvent.Value);
            }
        }
    }
}
