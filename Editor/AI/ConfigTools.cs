using System;
using System.Collections.Generic;
using Larje.Core.Ids;
using Unity.AI.MCP.Editor.Helpers;
using Unity.AI.MCP.Editor.ToolRegistry;
using UnityEditor;
using UnityEngine;

namespace Larje.Core.Editor.AI
{
    public static class ConfigTools
    {
        private const string LIST_DESCRIPTION = "Lists Larje ScriptableObject configs in the project (UIConfig, SoundConfig, CurrencyConfig, SaveConfig, LocationsConfig, LevelsConfig, InputConfig and others from Larje packages) with asset paths. Use Unity asset tools to edit them.";
        private const string VALIDATE_DESCRIPTION = "Validates all Larje configs and id assets: missing required references, unconfigured ids, duplicate id keys, level prefabs without LevelProcessor, sounds without clips. Returns a list of problems per asset.";

        [McpTool("Larje.ListConfigs", LIST_DESCRIPTION, "List Larje Configs", EnabledByDefault = true, Groups = new[] { "larje", "larje-assets" })]
        public static object List(ListParams parameters)
        {
            List<object> configs = new List<object>();
            foreach (ScriptableObject config in FindConfigs())
            {
                string type = config.GetType().Name;
                if (string.IsNullOrEmpty(parameters.Type) || type == parameters.Type)
                {
                    configs.Add(new { type, name = config.name, path = AssetDatabase.GetAssetPath(config) });
                }
            }

            return Response.Success($"{configs.Count} configs", configs);
        }

        [McpTool("Larje.ValidateProject", VALIDATE_DESCRIPTION, "Validate Larje Project", EnabledByDefault = true, Groups = new[] { "larje", "larje-assets" })]
        public static object Validate()
        {
            List<object> reports = new List<object>();
            int total = 0;
            foreach (ScriptableObject config in FindConfigs())
            {
                if (config is IConfigValidatable validatable)
                {
                    List<string> problems = new List<string>();
                    validatable.Validate(problems);
                    if (problems.Count > 0)
                    {
                        total += problems.Count;
                        reports.Add(new { asset = AssetDatabase.GetAssetPath(config), problems });
                    }
                }
            }

            foreach (string duplicate in FindDuplicateIdKeys())
            {
                total++;
                reports.Add(new { asset = "ids", problems = new[] { duplicate } });
            }

            return Response.Success(total == 0 ? "No problems found" : $"{total} problems", reports);
        }

        private static IEnumerable<ScriptableObject> FindConfigs()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:ScriptableObject"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (asset != null && !(asset is LarjeId) && IsLarjeType(asset.GetType()))
                {
                    yield return asset;
                }
            }
        }

        private static IEnumerable<string> FindDuplicateIdKeys()
        {
            foreach (Type type in TypeCache.GetTypesDerivedFrom<LarjeId>())
            {
                if (!type.IsAbstract)
                {
                    HashSet<string> keys = new HashSet<string>();
                    foreach (LarjeId id in Ids.LarjeIdAssets.FindAll(type))
                    {
                        if (!keys.Add(id.Key))
                        {
                            yield return $"Duplicate {type.Name} key '{id.Key}' at {AssetDatabase.GetAssetPath(id)}";
                        }
                    }
                }
            }
        }

        private static bool IsLarjeType(Type type)
        {
            return type.Namespace != null && type.Namespace.StartsWith("Larje", StringComparison.Ordinal);
        }

        public sealed class ListParams
        {
            [McpDescription("Optional config type name filter, e.g. UIConfig")]
            public string Type { get; set; }
        }
    }
}
