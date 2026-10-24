using UnityEngine;

namespace Larje.Core.Splines
{
    public sealed class LinearSpline : Spline
    {
        protected override Vector3 EvaluateSegment(int segment, float localT)
        {
            return Vector3.Lerp(GetPointAt(segment), GetPointAt(segment + 1), localT);
        }

        protected override float CalculateLength()
        {
            float length = 0f;
            int segments = _points.Length - (Closed ? 0 : 1);
            for (int i = 0; i < segments; i++)
            {
                length += Vector3.Distance(GetPointAt(i), GetPointAt(i + 1));
            }

            return length;
        }

        protected override Vector3[] GetDrawLineSteps(float step)
        {
            if (!Closed || _points.Length < 2)
            {
                return _points;
            }

            Vector3[] steps = new Vector3[_points.Length + 1];
            _points.CopyTo(steps, 0);
            steps[_points.Length] = _points[0];
            return steps;
        }
    }
}
