using System;
using System.Collections.Generic;
using Larje.Core.Ids;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEngine;

namespace Larje.Core.Editor.Cli
{
    public static class ConfigCommands
    {
        private const string GROUP = "larje.configs";
        private const string ROOT_GROUP = "larje";
        private const string LIST_DESCRIPTION = "Lists Larje ScriptableObject configs in the project (UIConfig, SoundConfig, CurrencyConfig, SaveConfig, LocationsConfig, LevelsConfig, InputConfig and others from Larje packages) with asset paths.";
        private const string VALIDATE_DESCRIPTION = "Validates all Larje configs and id assets: missing required references, unconfigured ids, duplicate id keys, level prefabs without LevelProcessor, sounds without clips. Returns the problem count and the problems per asset.";

        [CliGroupCommand(GROUP, LIST_DESCRIPTION, Tags = new[] { "larje", "assets" })]
        public static object List(
            [CliArg("type", "Config type name filter, e.g. UIConfig")] string type = null)
        {
            List<object> configs = new List<object>();
            foreach (ScriptableObject config in FindConfigs())
            {
                string configType = config.GetType().Name;
                if (string.IsNullOrEmpty(type) || configType == type)
                {
                    configs.Add(new { type = configType, name = config.name, path = AssetDatabase.GetAssetPath(config) });
                }
            }

            return configs;
        }

        [CliGroupCommand(ROOT_GROUP, VALIDATE_DESCRIPTION, Tags = new[] { "larje", "assets" })]
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

            return new { problems = total, reports };
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
    }
}
