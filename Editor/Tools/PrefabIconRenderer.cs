using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Larje.Core.Editor.Tools
{
    public static class PrefabIconRenderer
    {
        public static string Render(GameObject prefab, Settings settings)
        {
            EnsureFolder(settings.OutputFolder);

            RenderTexture texture = new RenderTexture(settings.Width, settings.Height, 24, RenderTextureFormat.ARGB32) { antiAliasing = 4 };
            texture.Create();

            GameObject cameraObject = EditorUtility.CreateGameObjectWithHideFlags("__IconCamera__", HideFlags.HideAndDontSave);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = settings.Transparent ? Color.clear : settings.Background;
            camera.targetTexture = texture;
            camera.fieldOfView = 30f;
            camera.nearClipPlane = 0.001f;
            camera.farClipPlane = 10000f;
            camera.enabled = false;

            GameObject lightObject = EditorUtility.CreateGameObjectWithHideFlags("__IconLight__", HideFlags.HideAndDontSave);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.hideFlags = HideFlags.HideAndDontSave;
            instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            try
            {
                Bounds bounds = CalculateBounds(instance);
                Quaternion rotation = Quaternion.Euler(settings.CameraAngle);
                float distance = bounds.extents.magnitude * (1f + settings.Padding) / Mathf.Sin(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                cameraObject.transform.SetPositionAndRotation(bounds.center - rotation * Vector3.forward * distance, rotation);

                RenderTexture previous = RenderTexture.active;
                camera.Render();
                RenderTexture.active = texture;
                Texture2D readback = new Texture2D(settings.Width, settings.Height, TextureFormat.ARGB32, false);
                readback.ReadPixels(new Rect(0, 0, settings.Width, settings.Height), 0, 0);
                readback.Apply();
                RenderTexture.active = previous;

                string assetPath = $"{settings.OutputFolder}/{settings.FilePrefix}{prefab.name}.png";
                File.WriteAllBytes(Path.GetFullPath(assetPath), readback.EncodeToPNG());
                Object.DestroyImmediate(readback);

                AssetDatabase.ImportAsset(assetPath);
                ApplySpriteSettings(assetPath);
                return assetPath;
            }
            finally
            {
                Object.DestroyImmediate(instance);
                Object.DestroyImmediate(cameraObject);
                Object.DestroyImmediate(lightObject);
                texture.Release();
                Object.DestroyImmediate(texture);
            }
        }

        private static void ApplySpriteSettings(string assetPath)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        private static Bounds CalculateBounds(GameObject target)
        {
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                return new Bounds(target.transform.position, Vector3.one);
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds;
        }

        private static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder))
            {
                return;
            }

            string[] parts = folder.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }

        [Serializable]
        public struct Settings
        {
            public int Width;
            public int Height;
            public Vector3 CameraAngle;
            public float Padding;
            public bool Transparent;
            public Color Background;
            public string OutputFolder;
            public string FilePrefix;

            public static Settings Default => new Settings
            {
                Width = 256,
                Height = 256,
                CameraAngle = new Vector3(30f, -45f, 0f),
                Padding = 0.15f,
                Transparent = true,
                Background = new Color(0.2f, 0.2f, 0.2f, 1f),
                OutputFolder = "Assets/Icons",
                FilePrefix = string.Empty
            };
        }
    }
}
