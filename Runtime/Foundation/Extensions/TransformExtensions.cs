using UnityEngine;

namespace Larje.Core.Extensions
{
    public static class TransformExtensions
    {
        public static string GetPath(this Transform transform)
        {
            string path = transform.gameObject.name;
            Transform current = transform.parent;
            while (current != null)
            {
                path = current.gameObject.name + "/" + path;
                current = current.parent;
            }

            return path;
        }

        public static void DestroyAllChildren(this Transform transform)
        {
            if (transform == null)
            {
                return;
            }

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (Application.isPlaying)
                {
                    Object.Destroy(child);
                }
                else
                {
                    Object.DestroyImmediate(child);
                }
            }
        }
    }
}
