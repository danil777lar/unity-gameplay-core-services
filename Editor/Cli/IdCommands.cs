using System;
using System.Collections.Generic;
using Larje.Core.Editor.Ids;
using Larje.Core.Ids;
using Unity.Pipeline.Commands;
using UnityEditor;

namespace Larje.Core.Editor.Cli
{
    public static class IdCommands
    {
        private const string GROUP = "larje.ids";
        private const string LIST_DESCRIPTION = "Lists Larje id assets (ScriptableObject identifiers such as CurrencyId, SoundId, UIScreenId, GameStateId, LocationId, TriggerId). Without a type returns available id types with counts; with a type returns each id key, name and asset path.";
        private const string CREATE_DESCRIPTION = "Creates a new Larje id asset of the given id type (e.g. CurrencyId) under Assets/Configs/Ids/<Type> or a custom folder. Returns the key and asset path.";

        [CliGroupCommand(GROUP, LIST_DESCRIPTION, Tags = new[] { "larje", "assets" })]
        public static object List(
            [CliArg("type", "Id type name, e.g. CurrencyId; omit to list types")] string type = null)
        {
            if (string.IsNullOrEmpty(type))
            {
                List<object> types = new List<object>();
                foreach (Type idType in TypeCache.GetTypesDerivedFrom<LarjeId>())
                {
                    if (!idType.IsAbstract)
                    {
                        types.Add(new { type = idType.Name, fullName = idType.FullName, count = LarjeIdAssets.FindAll(idType).Count });
                    }
                }

                return types;
            }

            Type resolved = ResolveType(type);
            if (resolved == null)
            {
                throw new ArgumentException($"Unknown id type '{type}'");
            }

            List<object> ids = new List<object>();
            foreach (LarjeId id in LarjeIdAssets.FindAll(resolved))
            {
                ids.Add(new { key = id.Key, name = id.name, path = AssetDatabase.GetAssetPath(id) });
            }

            return ids;
        }

        [CliGroupCommand(GROUP, CREATE_DESCRIPTION, Tags = new[] { "larje", "assets" })]
        public static object Create(
            [CliArg("type", "Id type name, e.g. SoundId", Required = true)] string type,
            [CliArg("name", "Asset name; also used as key", Required = true)] string name,
            [CliArg("folder", "Root folder, default Assets/Configs/Ids")] string folder = null)
        {
            Type resolved = ResolveType(type);
            if (resolved == null)
            {
                throw new ArgumentException($"Unknown id type '{type}'");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name is required");
            }

            LarjeId id = string.IsNullOrEmpty(folder)
                ? LarjeIdAssets.Create(resolved, name)
                : LarjeIdAssets.Create(resolved, name, folder);
            return new { key = id.Key, name = id.name, path = AssetDatabase.GetAssetPath(id) };
        }

        internal static Type ResolveType(string name)
        {
            foreach (Type type in TypeCache.GetTypesDerivedFrom<LarjeId>())
            {
                if (!type.IsAbstract && (type.Name == name || type.FullName == name))
                {
                    return type;
                }
            }

            return null;
        }
    }
}
