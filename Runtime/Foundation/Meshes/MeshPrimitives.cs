using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Meshes
{
    public static class MeshPrimitives
    {
        public static Mesh GenerateCapsuleMesh(float radius, float height, int segments = 8, int rings = 4)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> triangles = new List<int>();

            float halfCylinder = Mathf.Max(0f, height - 2f * radius) * 0.5f;
            int vertsPerRing = segments + 1;

            for (int y = 0; y <= rings; y++)
            {
                float phi = Mathf.PI * 0.5f * (1f - y / (float)rings);
                AddRing(vertices, normals, segments, Mathf.Cos(phi) * radius, Mathf.Sin(phi) * radius + halfCylinder, halfCylinder);
            }

            AddRing(vertices, normals, segments, radius, halfCylinder, halfCylinder);
            AddRing(vertices, normals, segments, radius, -halfCylinder, -halfCylinder);

            for (int y = 1; y <= rings; y++)
            {
                float phi = Mathf.PI * 0.5f * (y / (float)rings);
                AddRing(vertices, normals, segments, Mathf.Cos(phi) * radius, -Mathf.Sin(phi) * radius - halfCylinder, -halfCylinder);
            }

            int rows = vertices.Count / vertsPerRing - 1;
            for (int y = 0; y < rows; y++)
            {
                for (int i = 0; i < segments; i++)
                {
                    int current = y * vertsPerRing + i;
                    int next = current + vertsPerRing;

                    triangles.Add(current);
                    triangles.Add(next);
                    triangles.Add(current + 1);

                    triangles.Add(current + 1);
                    triangles.Add(next);
                    triangles.Add(next + 1);
                }
            }

            Mesh mesh = new Mesh();
            mesh.name = "Capsule";
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static void AddRing(List<Vector3> vertices, List<Vector3> normals, int segments, float ringRadius, float y, float normalCenterY)
        {
            for (int i = 0; i <= segments; i++)
            {
                float theta = i / (float)segments * Mathf.PI * 2f;
                float x = Mathf.Cos(theta) * ringRadius;
                float z = Mathf.Sin(theta) * ringRadius;

                vertices.Add(new Vector3(x, y, z));
                normals.Add(new Vector3(x, y - normalCenterY, z).normalized);
            }
        }
    }
}
