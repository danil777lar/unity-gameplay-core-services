using System.Collections.Generic;
using Larje.Core.Editor.Locations;
using Larje.Core.Editor.Tools;
using Larje.Core.Locations;
using Larje.Core.Ids;
using Unity.AI.MCP.Editor.Helpers;
using Unity.AI.MCP.Editor.ToolRegistry;
using UnityEditor;
using UnityEngine;

namespace Larje.Core.Editor.AI
{
    public static class SceneTools
    {
        private const string ARGS_DESCRIPTION = "Previews location arguments in the open scene: every LocationArgActivator enables only the objects whose LocationArgId is in the list. Undo-able. Pass LocationArgId keys or names.";
        private const string ICONS_DESCRIPTION = "Renders PNG sprite icons for prefab assets with an orthographic-like framing camera. Returns created asset paths.";

        [McpTool("Larje.PreviewLocationArgs", ARGS_DESCRIPTION, "Preview Location Args", EnabledByDefault = true, Groups = new[] { "larje", "larje-scene" })]
        public static object PreviewLocationArgs(ArgsParams parameters)
        {
            List<LocationArgId> args = new List<LocationArgId>();
            if (parameters.Args != null)
            {
                foreach (string key in parameters.Args)
                {
                    LarjeId id = Debugging.DebugValueParser.FindId(typeof(LocationArgId), key);
                    if (id == null)
                    {
                        return Response.Error($"LocationArgId '{key}' not found");
                    }

                    args.Add((LocationArgId)id);
                }
            }

            int count = LocationArgPreviewWindow.ApplyToScene(args);
            return Response.Success($"Applied to {count} activators");
        }

        [McpTool("Larje.RenderPrefabIcons", ICONS_DESCRIPTION, "Render Prefab Icons", EnabledByDefault = true, Groups = new[] { "larje", "larje-assets" })]
        public static object RenderPrefabIcons(IconParams parameters)
        {
            PrefabIconRenderer.Settings settings = PrefabIconRenderer.Settings.Default;
            if (parameters.Size > 0)
            {
                settings.Width = parameters.Size;
                settings.Height = parameters.Size;
            }

            if (!string.IsNullOrEmpty(parameters.OutputFolder))
            {
                settings.OutputFolder = parameters.OutputFolder;
            }

            List<string> created = new List<string>();
            foreach (string path in parameters.PrefabPaths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                {
                    return Response.Error($"Prefab not found at {path}");
                }

                created.Add(PrefabIconRenderer.Render(prefab, settings));
            }

            return Response.Success($"Rendered {created.Count}", created);
        }

        public sealed class ArgsParams
        {
            [McpDescription("LocationArgId keys or names to enable")]
            public List<string> Args { get; set; }
        }

        public sealed class IconParams
        {
            [McpDescription("Prefab asset paths", Required = true)]
            public List<string> PrefabPaths { get; set; }

            [McpDescription("Icon size in pixels, default 256")]
            public int Size { get; set; }

            [McpDescription("Output folder, default Assets/Icons")]
            public string OutputFolder { get; set; }
        }
    }
}
