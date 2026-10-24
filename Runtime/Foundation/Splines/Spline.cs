using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Splines
{
    public abstract class Spline
    {
        private const float LENGTH_STEP = 0.01f;

        protected Vector3[] _points = new Vector3[0];

        public bool Closed { get; private set; }
        public float Length { get; private set; }
        public IReadOnlyList<Vector3> Points => _points;

        public Spline SetPoints(Vector3[] points)
        {
            _points = points ?? new Vector3[0];
            Length = CalculateLength();
            return this;
        }

        public Spline SetClosed(bool closed)
        {
            Closed = closed;
            Length = CalculateLength();
            return this;
        }

        public Vector3 Evaluate(float t)
        {
            if (_points.Length == 0)
            {
                return Vector3.zero;
            }

            if (_points.Length == 1)
            {
                return _points[0];
            }

            int segments = _points.Length - (Closed ? 0 : 1);
            float scaled = Mathf.Clamp01(t) * segments;
            int segment = Mathf.Min(Mathf.FloorToInt(scaled), segments - 1);
            return EvaluateSegment(segment, scaled - segment);
        }

        public void DrawLineGizmo(Color color, float step = 0.1f)
        {
            Gizmos.color = color;
            Vector3[] steps = GetDrawLineSteps(step);
            for (int i = 0; i < steps.Length - 1; i++)
            {
                Gizmos.DrawLine(steps[i], steps[i + 1]);
            }

            foreach (Vector3 point in _points)
            {
                Gizmos.DrawWireSphere(point, 0.1f);
            }
        }

        public void DrawLineDebug(Color color, float step = 0.1f)
        {
            Vector3[] steps = GetDrawLineSteps(step);
            for (int i = 0; i < steps.Length - 1; i++)
            {
                Debug.DrawLine(steps[i], steps[i + 1], color);
            }
        }

        protected abstract Vector3 EvaluateSegment(int segment, float localT);

        protected virtual float CalculateLength()
        {
            if (_points.Length < 2)
            {
                return 0f;
            }

            float length = 0f;
            Vector3 previous = Evaluate(0f);
            for (float t = LENGTH_STEP; t <= 1f; t += LENGTH_STEP)
            {
                Vector3 current = Evaluate(t);
                length += Vector3.Distance(previous, current);
                previous = current;
            }

            return length + Vector3.Distance(previous, Evaluate(1f));
        }

        protected virtual Vector3[] GetDrawLineSteps(float step)
        {
            List<Vector3> steps = new List<Vector3>();
            for (float t = 0f; t < 1f; t += step)
            {
                steps.Add(Evaluate(t));
            }

            steps.Add(Evaluate(1f));
            return steps.ToArray();
        }

        protected Vector3 GetPointAt(int index)
        {
            if (Closed)
            {
                return _points[(index % _points.Length + _points.Length) % _points.Length];
            }

            return _points[Mathf.Clamp(index, 0, _points.Length - 1)];
        }
    }
}
