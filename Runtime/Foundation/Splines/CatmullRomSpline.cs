using UnityEngine;

namespace Larje.Core.Splines
{
    public sealed class CatmullRomSpline : Spline
    {
        protected override Vector3 EvaluateSegment(int segment, float localT)
        {
            Vector3 p0 = GetPointAt(segment - 1);
            Vector3 p1 = GetPointAt(segment);
            Vector3 p2 = GetPointAt(segment + 1);
            Vector3 p3 = GetPointAt(segment + 2);

            float t2 = localT * localT;
            float t3 = t2 * localT;

            return 0.5f * (2f * p1
                + (p2 - p0) * localT
                + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2
                + (3f * p1 - p0 - 3f * p2 + p3) * t3);
        }
    }
}
