using System;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Meshes
{
    public sealed class FovMeshBuilder
    {
        private float _angleStep;
        private readonly Options _options;
        private readonly List<RaycastHit> _hits = new List<RaycastHit>();
        private readonly List<Vector3> _vertices = new List<Vector3>();

        public float AngleStep => _angleStep;
        public Options Settings => _options;
        public IReadOnlyList<RaycastHit> Hits => _hits;
        public IReadOnlyList<Vector3> Vertices => _vertices;

        public FovMeshBuilder(Options options)
        {
            _options = options;
        }

        public void BuildMesh()
        {
            _hits.Clear();
            _vertices.Clear();

            if (_options.RaysPerDegree <= 0f)
            {
                return;
            }

            int rayCount = Mathf.Max(1, Mathf.RoundToInt(_options.Angle * _options.RaysPerDegree));
            float angle = _options.Angle * 0.5f;
            _angleStep = _options.Angle / rayCount;

            Transform transform = _options.MeshFilter.transform;
            Vector3[] vertices = new Vector3[rayCount + 2];
            int[] triangles = new int[rayCount * 3];
            vertices[0] = Vector3.zero;

            for (int i = 0; i <= rayCount; i++)
            {
                Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0f, Mathf.Cos(Mathf.Deg2Rad * angle));
                direction = (Quaternion.Euler(0f, _options.DirectionRotate, 0f) * direction).normalized;

                Vector3 origin = transform.position + _options.RaycastOffset;
                Vector3 worldDirection = transform.TransformDirection(direction);
                Vector3 vertex;

                if (Physics.Raycast(origin, worldDirection, out RaycastHit hit, _options.Distance, _options.RaycastMask))
                {
                    vertex = transform.InverseTransformPoint(hit.point - _options.RaycastOffset);
                    _hits.Add(hit);
                }
                else
                {
                    vertex = direction * _options.Distance;
                }

                _vertices.Add(vertex);
                vertices[i + 1] = vertex;

                if (i > 0)
                {
                    int triangle = (i - 1) * 3;
                    triangles[triangle] = i + 1;
                    triangles[triangle + 1] = i;
                    triangles[triangle + 2] = 0;
                }

                angle -= _angleStep;
            }

            if (_options.MeshFilter.sharedMesh == null)
            {
                _options.MeshFilter.sharedMesh = new Mesh { name = "FOV" };
            }

            Mesh mesh = _options.MeshFilter.sharedMesh;
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.uv = new Vector2[vertices.Length];
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
        }

        public bool IsPositionInFov(Vector3 position)
        {
            if (_vertices.Count == 0 || _angleStep <= 0f)
            {
                return false;
            }

            Vector3 local = _options.MeshFilter.transform.InverseTransformPoint(position);
            float angle = Vector3.SignedAngle(Vector3.forward, local, Vector3.up) - _options.DirectionRotate;
            float halfAngle = _options.Angle * 0.5f;
            if (Mathf.Abs(angle) > halfAngle)
            {
                return false;
            }

            int index = Mathf.Clamp(Mathf.RoundToInt((halfAngle - angle) / _angleStep), 0, _vertices.Count - 1);
            return local.magnitude < _vertices[index].magnitude;
        }

        [Serializable]
        public sealed class Options
        {
            public float Angle;
            public float RaysPerDegree;
            public float Distance;
            public float DirectionRotate;
            public Vector3 RaycastOffset;
            public LayerMask RaycastMask;
            public MeshFilter MeshFilter;
        }
    }
}
