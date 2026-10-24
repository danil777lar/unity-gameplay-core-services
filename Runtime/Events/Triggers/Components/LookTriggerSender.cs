using UnityEngine;

namespace Larje.Core.Events
{
    public sealed class LookTriggerSender : TriggerSender
    {
        [Header("Viewport")]
        [SerializeField] private Vector2 viewportMin = Vector2.zero;
        [SerializeField] private Vector2 viewportMax = Vector2.one;

        private Camera _camera;

        protected override void Start()
        {
            _camera = Camera.main;
            base.Start();
        }

        private void Update()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            Value = _camera != null && IsVisible(transform.position) ? 1f : 0f;
        }

        private bool IsVisible(Vector3 point)
        {
            Vector3 viewport = _camera.WorldToViewportPoint(point);
            return viewport.z > 0f
                && viewport.x >= viewportMin.x && viewport.x <= viewportMax.x
                && viewport.y >= viewportMin.y && viewport.y <= viewportMax.y;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            if (Value >= 1f)
            {
                Gizmos.DrawSphere(transform.position, 0.25f);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, 0.25f);
            }
        }
    }
}
