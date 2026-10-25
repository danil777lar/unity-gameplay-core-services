using System;
using System.Collections.Generic;
using Larje.Core.Editor.Locations;
using Larje.Core.Editor.Tools;
using Larje.Core.Ids;
using Larje.Core.Locations;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEngine;

namespace Larje.Core.Editor.Cli
{
    public static class SceneCommands
    {
        private const string SCENE_GROUP = "larje.scene";
        private const string ASSETS_GROUP = "larje.assets";
        private const string ARGS_DESCRIPTION = "Previews location arguments in the open scene: every LocationArgActivator enables only the objects whose LocationArgId is in the list. Undo-able. Pass LocationArgId keys or names as a JSON array, e.g. --args '[\"night\",\"rain\"]'.";
        private const string ICONS_DESCRIPTION = "Renders PNG sprite icons for prefab assets with a framing camera. Pass prefab asset paths as a JSON array. Returns created asset paths.";

        [CliGroupCommand(SCENE_GROUP, ARGS_DESCRIPTION, Tags = new[] { "larje", "scenes" })]
        public static object PreviewLocationArgs(
            [CliArg("args", "LocationArgId keys or names to enable, JSON array; omit to disable all")] List<string> args = null)
        {
            List<LocationArgId> ids = new List<LocationArgId>();
            if (args != null)
            {
                foreach (string key in args)
                {
                    LarjeId id = Debugging.DebugValueParser.FindId(typeof(LocationArgId), key);
                    if (id == null)
                    {
                        throw new ArgumentException($"LocationArgId '{key}' not found");
                    }

                    ids.Add((LocationArgId)id);
                }
            }

            int count = LocationArgPreviewWindow.ApplyToScene(ids);
            return new { activators = count };
        }

        [CliGroupCommand(ASSETS_GROUP, ICONS_DESCRIPTION, Tags = new[] { "larje", "assets" })]
        public static object RenderPrefabIcons(
            [CliArg("prefabs", "Prefab asset paths, JSON array", Required = true)] List<string> prefabs,
            [CliArg("size", "Icon size in pixels, default 256")] int size = 0,
            [CliArg("output_folder", "Output folder, default Assets/Icons")] string outputFolder = null)
        {
            PrefabIconRenderer.Settings settings = PrefabIconRenderer.Settings.Default;
            if (size > 0)
            {
                settings.Width = size;
                settings.Height = size;
            }

            if (!string.IsNullOrEmpty(outputFolder))
            {
                settings.OutputFolder = outputFolder;
            }

            List<string> created = new List<string>();
            foreach (string path in prefabs)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                {
                    throw new ArgumentException($"Prefab not found at {path}");
                }

                created.Add(PrefabIconRenderer.Render(prefab, settings));
            }

            return created;
        }
    }
}
