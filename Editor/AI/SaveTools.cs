using System.Collections.Generic;
using Larje.Core.Saves;
using Unity.AI.MCP.Editor.Helpers;
using Unity.AI.MCP.Editor.ToolRegistry;
using UnityEditor;

namespace Larje.Core.Editor.AI
{
    public static class SaveTools
    {
        private const string READ_DESCRIPTION = "Reads Larje save data from disk outside Play Mode (default FileSaveStorage). Without a key lists save slots and section keys; with a key returns the section JSON. Scope is Game (per slot) or System (settings).";
        private const string WRITE_DESCRIPTION = "Replaces one Larje save section on disk outside Play Mode with the given JSON, keeping other sections. Use Larje.ReadSave to inspect the current JSON first. In Play Mode use the Saves debug commands instead.";

        [McpTool("Larje.ReadSave", READ_DESCRIPTION, "Read Larje Save", EnabledByDefault = true, Groups = new[] { "larje", "larje-assets" })]
        public static object Read(ReadParams parameters)
        {
            if (EditorApplication.isPlaying)
            {
                return Response.Error("Use the Saves debug commands in Play Mode");
            }

            SaveService service = CreateService(parameters.ConfigPath, parameters.Slot);
            if (string.IsNullOrEmpty(parameters.Key))
            {
                List<string> slots = new List<string>();
                foreach (SaveMeta meta in service.GetSaves())
                {
                    slots.Add(meta.Slot);
                }

                return Response.Success("Save overview", new
                {
                    slot = service.CurrentSlot,
                    slots,
                    gameSections = service.GetSectionKeys(SaveScope.Game),
                    systemSections = service.GetSectionKeys(SaveScope.System)
                });
            }

            string json = service.ExportSection(parameters.Scope, parameters.Key);
            return json != null ? Response.Success(parameters.Key, json) : Response.Error($"Section '{parameters.Key}' not found");
        }

        [McpTool("Larje.WriteSaveSection", WRITE_DESCRIPTION, "Write Larje Save Section", EnabledByDefault = true, Groups = new[] { "larje", "larje-assets" })]
        public static object Write(WriteParams parameters)
        {
            if (EditorApplication.isPlaying)
            {
                return Response.Error("Use the Saves debug commands in Play Mode");
            }

            SaveService service = CreateService(parameters.ConfigPath, parameters.Slot);
            if (!service.ImportSection(parameters.Scope, parameters.Key, parameters.Json))
            {
                return Response.Error("Invalid JSON");
            }

            if (parameters.Scope == SaveScope.System)
            {
                service.SaveSystem();
            }
            else
            {
                service.SaveGame();
            }

            return Response.Success($"Section '{parameters.Key}' written to slot '{service.CurrentSlot}'");
        }

        private static SaveService CreateService(string configPath, string slot)
        {
            SaveConfig config = null;
            if (!string.IsNullOrEmpty(configPath))
            {
                config = AssetDatabase.LoadAssetAtPath<SaveConfig>(configPath);
            }

            if (config == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:SaveConfig");
                config = guids.Length > 0 ? AssetDatabase.LoadAssetAtPath<SaveConfig>(AssetDatabase.GUIDToAssetPath(guids[0])) : UnityEngine.ScriptableObject.CreateInstance<SaveConfig>();
            }

            SaveService service = new SaveService(config, new FileSaveStorage());
            service.Reload(false);
            string target = string.IsNullOrEmpty(slot) ? config.DefaultSlot : slot;
            if (!service.LoadGame(target))
            {
                service.NewGame(target);
            }

            return service;
        }

        public sealed class ReadParams
        {
            [McpDescription("Save slot, default from SaveConfig")]
            public string Slot { get; set; }

            [McpDescription("Section key; empty for overview")]
            public string Key { get; set; }

            [McpDescription("Game or System", EnumType = typeof(SaveScope))]
            public SaveScope Scope { get; set; }

            [McpDescription("Optional SaveConfig asset path, needed for encrypted saves")]
            public string ConfigPath { get; set; }
        }

        public sealed class WriteParams
        {
            [McpDescription("Save slot, default from SaveConfig")]
            public string Slot { get; set; }

            [McpDescription("Section key", Required = true)]
            public string Key { get; set; }

            [McpDescription("Section JSON", Required = true)]
            public string Json { get; set; }

            [McpDescription("Game or System", EnumType = typeof(SaveScope))]
            public SaveScope Scope { get; set; }

            [McpDescription("Optional SaveConfig asset path, needed for encrypted saves")]
            public string ConfigPath { get; set; }
        }
    }
}
