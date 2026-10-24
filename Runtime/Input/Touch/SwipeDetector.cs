using System;
using System.Collections.Generic;
using Larje.Core.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Larje.Core.Input
{
    [RequireComponent(typeof(RectTransformEvents))]
    public sealed class SwipeDetector : MonoBehaviour
    {
        [SerializeField] private SwipeConfig config;

        private bool _active = true;
        private bool _tracking;
        private Vector2 _start;
        private Vector2 _current;
        private RectTransformEvents _events;
        private readonly Dictionary<Vector2, Action> _directions = new Dictionary<Vector2, Action>();

        public void SetActive(bool active)
        {
            _active = active;
            _tracking = false;
        }

        public void Bind(Vector2 direction, Action onSwipe)
        {
            Vector2 key = direction.normalized;
            _directions[key] = _directions.TryGetValue(key, out Action existing) ? existing + onSwipe : onSwipe;
        }

        public void Clear()
        {
            _directions.Clear();
        }

        private void Awake()
        {
            _events = GetComponent<RectTransformEvents>();
            _events.PointerDown += OnPointerDown;
            _events.Drag += OnPointerMoved;
            _events.PointerMove += OnPointerMoved;
            _events.PointerUp += OnPointerUp;
        }

        private void OnDestroy()
        {
            _events.PointerDown -= OnPointerDown;
            _events.Drag -= OnPointerMoved;
            _events.PointerMove -= OnPointerMoved;
            _events.PointerUp -= OnPointerUp;
        }

        private void OnPointerDown(PointerEventData eventData)
        {
            if (_active && config.Enabled)
            {
                _tracking = true;
                _start = eventData.position;
                _current = eventData.position;
            }
        }

        private void OnPointerMoved(PointerEventData eventData)
        {
            if (!_tracking)
            {
                return;
            }

            _current = eventData.position;
            if (Length() >= config.MaxLength)
            {
                Fire();
            }
        }

        private void OnPointerUp(PointerEventData eventData)
        {
            if (!_tracking)
            {
                return;
            }

            _current = eventData.position;
            if (Length() >= config.MinLength)
            {
                Fire();
            }

            _tracking = false;
        }

        private void Fire()
        {
            _tracking = false;
            Vector2 direction = (_current - _start).normalized;
            Vector2 best = Vector2.zero;
            float bestAngle = float.MaxValue;
            foreach (Vector2 candidate in _directions.Keys)
            {
                float angle = Vector2.Angle(direction, candidate);
                if (angle < bestAngle)
                {
                    bestAngle = angle;
                    best = candidate;
                }
            }

            if (bestAngle <= config.MaxAngle && _directions.TryGetValue(best, out Action action))
            {
                action?.Invoke();
            }
        }

        private float Length()
        {
            return Vector2.Distance(_start, _current) / Screen.width;
        }
    }
}
