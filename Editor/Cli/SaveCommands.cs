using System;
using System.Collections.Generic;
using Larje.Core.Saves;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEngine;

namespace Larje.Core.Editor.Cli
{
    public static class SaveCommands
    {
        private const string GROUP = "larje.save";
        private const string READ_DESCRIPTION = "Reads Larje save data from disk outside Play Mode (default FileSaveStorage). Without a key returns the current slot, save slots and section keys; with a key returns the section JSON. Scope is Game (per slot) or System (settings).";
        private const string WRITE_DESCRIPTION = "Replaces one Larje save section on disk outside Play Mode, keeping other sections. Pass the section JSON inline with --json or as a file path with --file. Use larje.save read first. In Play Mode use the Saves debug commands instead.";

        [CliGroupCommand(GROUP, READ_DESCRIPTION, Tags = new[] { "larje", "assets" })]
        public static object Read(
            [CliArg("key", "Section key; omit for an overview")] string key = null,
            [CliArg("scope", "Game or System")] SaveScope scope = SaveScope.Game,
            [CliArg("slot", "Save slot, default from SaveConfig")] string slot = null,
            [CliArg("config", "SaveConfig asset path, needed for encrypted saves")] string config = null)
        {
            RequireEditMode();
            SaveService service = CreateService(config, slot);
            if (string.IsNullOrEmpty(key))
            {
                List<string> slots = new List<string>();
                foreach (SaveMeta meta in service.GetSaves())
                {
                    slots.Add(meta.Slot);
                }

                return new
                {
                    slot = service.CurrentSlot,
                    slots,
                    gameSections = service.GetSectionKeys(SaveScope.Game),
                    systemSections = service.GetSectionKeys(SaveScope.System)
                };
            }

            string json = service.ExportSection(scope, key);
            if (json == null)
            {
                throw new ArgumentException($"Section '{key}' not found");
            }

            return json;
        }

        [CliGroupCommand(GROUP, WRITE_DESCRIPTION, Tags = new[] { "larje", "assets" })]
        public static object Write(
            [CliArg("key", "Section key", Required = true)] string key,
            [CliArg("json", "Section JSON")] string json = null,
            [CliArg("file", "Path to a file with the section JSON, relative to the project root")] string file = null,
            [CliArg("scope", "Game or System")] SaveScope scope = SaveScope.Game,
            [CliArg("slot", "Save slot, default from SaveConfig")] string slot = null,
            [CliArg("config", "SaveConfig asset path, needed for encrypted saves")] string config = null)
        {
            RequireEditMode();
            string content = CliText.Resolve(json, file, "json");
            SaveService service = CreateService(config, slot);
            if (!service.ImportSection(scope, key, content))
            {
                throw new ArgumentException("Invalid JSON");
            }

            if (scope == SaveScope.System)
            {
                service.SaveSystem();
            }
            else
            {
                service.SaveGame();
            }

            return new { key, scope = scope.ToString(), slot = service.CurrentSlot };
        }

        private static void RequireEditMode()
        {
            if (EditorApplication.isPlaying)
            {
                throw new InvalidOperationException("Use the Saves debug commands in Play Mode");
            }
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
                config = guids.Length > 0 ? AssetDatabase.LoadAssetAtPath<SaveConfig>(AssetDatabase.GUIDToAssetPath(guids[0])) : ScriptableObject.CreateInstance<SaveConfig>();
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
    }
}
