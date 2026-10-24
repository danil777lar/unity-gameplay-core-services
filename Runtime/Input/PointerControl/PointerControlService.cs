using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Larje.Core.Tweening;
using Larje.Core.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Larje.Core.Input
{
    public sealed class PointerControlService : IPointerControlService, IDisposable
    {
        private bool _dragged;
        private Camera _camera;
        private Transform _rig;
        private RectTransformEvents _rect;
        private IPointerDragTarget _dragTarget;
        private readonly PointerControlConfig _config;
        private readonly TweenScope _hold = new TweenScope();
        private readonly List<Vector2> _deltas = new List<Vector2>();

        public event Action<PointerCommand> Command;

        public PointerControlService(PointerControlConfig config)
        {
            _config = config;
        }

        public void Dispose()
        {
            SetControlRect(null);
            _hold.Dispose();
        }

        public void SetCamera(Camera camera, Transform rig)
        {
            _camera = camera;
            _rig = rig;
        }

        public void SetControlRect(RectTransformEvents rect)
        {
            if (_rect != null)
            {
                _rect.Drag -= OnDrag;
                _rect.PointerDown -= OnPointerDown;
                _rect.PointerUp -= OnPointerUp;
            }

            _rect = rect;
            if (_rect != null)
            {
                _rect.Drag += OnDrag;
                _rect.PointerDown += OnPointerDown;
                _rect.PointerUp += OnPointerUp;
            }
        }

        private void OnDrag(PointerEventData eventData)
        {
            _dragged = true;
            _hold.Kill();

            Vector2 delta = Smooth(eventData.delta) * _config.DragSensitivity;
            if (_dragTarget != null)
            {
                _dragTarget.Transform.position += new Vector3(delta.x, 0f, delta.y);
            }
            else if (_rig != null)
            {
                _rig.position -= new Vector3(delta.x, 0f, delta.y);
            }
        }

        private void OnPointerDown(PointerEventData eventData)
        {
            _deltas.Clear();
            _dragged = false;
            if (!TryRaycast(eventData.position, out RaycastHit hit))
            {
                return;
            }

            if (hit.collider.TryGetComponent(out IPointerDragTarget dragTarget) && dragTarget.IsDraggable)
            {
                _dragTarget = dragTarget;
            }
            else if (hit.collider.TryGetComponent(out IPointerHoldTarget holdTarget))
            {
                HoldAsync(holdTarget, _hold.Restart()).SuppressCancellationThrow().Forget();
            }
        }

        private void OnPointerUp(PointerEventData eventData)
        {
            _dragTarget = null;
            if (!_dragged && TryRaycast(eventData.position, out RaycastHit hit))
            {
                IPointerClickTarget target = hit.collider.GetComponentInParent<IPointerClickTarget>();
                Command?.Invoke(target != null ? target.OnPointerClick(hit) : null);
            }

            _hold.Kill();
        }

        private async UniTask HoldAsync(IPointerHoldTarget target, CancellationToken cancellationToken)
        {
            await Tween.Delay(_config.HoldTime, true, cancellationToken);
            target.OnPointerHold();
        }

        private bool TryRaycast(Vector2 screenPosition, out RaycastHit hit)
        {
            Camera camera = _camera != null ? _camera : Camera.main;
            hit = default;
            return camera != null && Physics.Raycast(camera.ScreenPointToRay(screenPosition), out hit, _config.RaycastDistance, _config.Mask);
        }

        private Vector2 Smooth(Vector2 delta)
        {
            _deltas.Add(delta);
            if (_deltas.Count > _config.DragSmoothing)
            {
                _deltas.RemoveAt(0);
            }

            Vector2 sum = Vector2.zero;
            foreach (Vector2 entry in _deltas)
            {
                sum += entry;
            }

            return sum / _deltas.Count;
        }
    }
}
