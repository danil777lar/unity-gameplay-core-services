using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Larje.Core.Input
{
    public sealed class FloatingJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Visual")]
        [SerializeField] private RectTransform handle;
        [SerializeField] private RectTransform background;

        [Header("Options")]
        [SerializeField] private float maxDistance = 100f;
        [SerializeField] private bool hideInEditor;

        private bool _touching;
        private Vector2 _origin;
        private Vector2 _current;
        private Vector3 _restPosition;

        public bool IsTouching => _touching;
        public Vector2 Value
        {
            get
            {
                if (!_touching || maxDistance <= 0f)
                {
                    return Vector2.zero;
                }

                return Vector2.ClampMagnitude((_current - _origin) / maxDistance, 1f);
            }
        }

        public event Action<Vector2> PointerDown;
        public event Action<Vector2> PointerUp;

        public void OnPointerDown(PointerEventData eventData)
        {
            _touching = true;
            _origin = eventData.position;
            _current = eventData.position;
            UpdateVisual();
            PointerDown?.Invoke(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _current = eventData.position;
            Vector2 offset = _current - _origin;
            if (offset.magnitude > maxDistance)
            {
                _origin = _current - offset.normalized * maxDistance;
            }

            UpdateVisual();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _touching = false;
            UpdateVisual();
            PointerUp?.Invoke(eventData.position);
        }

        private void Start()
        {
            _restPosition = handle.position;
            if (Application.isEditor && hideInEditor)
            {
                CanvasGroup group = GetComponent<CanvasGroup>();
                if (group == null)
                {
                    group = gameObject.AddComponent<CanvasGroup>();
                }

                group.alpha = 0f;
            }
        }

        private void OnDisable()
        {
            _touching = false;
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (handle == null || background == null)
            {
                return;
            }

            if (_touching)
            {
                handle.position = _current;
                background.position = _origin;
            }
            else
            {
                handle.position = _restPosition;
                background.position = _restPosition;
            }
        }
    }
}
