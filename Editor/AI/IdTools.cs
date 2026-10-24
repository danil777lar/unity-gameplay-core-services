using System;
using System.Collections.Generic;
using Larje.Core.Editor.Ids;
using Larje.Core.Ids;
using Unity.AI.MCP.Editor.Helpers;
using Unity.AI.MCP.Editor.ToolRegistry;
using UnityEditor;

namespace Larje.Core.Editor.AI
{
    public static class IdTools
    {
        private const string LIST_DESCRIPTION = "Lists Larje id assets (ScriptableObject identifiers such as CurrencyId, SoundId, UIScreenId, GameStateId, LocationId, TriggerId). Without a type returns available id types with counts; with a type returns each id key, name and asset path.";
        private const string CREATE_DESCRIPTION = "Creates a new Larje id asset of the given id type (e.g. CurrencyId) under Assets/Configs/Ids/<Type> or a custom folder. Returns the asset path.";

        [McpTool("Larje.ListIds", LIST_DESCRIPTION, "List Larje Ids", EnabledByDefault = true, Groups = new[] { "larje", "larje-assets" })]
        public static object List(ListParams parameters)
        {
            if (string.IsNullOrEmpty(parameters.Type))
            {
                List<object> types = new List<object>();
                foreach (Type type in TypeCache.GetTypesDerivedFrom<LarjeId>())
                {
                    if (!type.IsAbstract)
                    {
                        types.Add(new { type = type.Name, fullName = type.FullName, count = LarjeIdAssets.FindAll(type).Count });
                    }
                }

                return Response.Success($"{types.Count} id types", types);
            }

            Type idType = ResolveType(parameters.Type);
            if (idType == null)
            {
                return Response.Error($"Unknown id type '{parameters.Type}'");
            }

            List<object> ids = new List<object>();
            foreach (LarjeId id in LarjeIdAssets.FindAll(idType))
            {
                ids.Add(new { key = id.Key, name = id.name, path = AssetDatabase.GetAssetPath(id) });
            }

            return Response.Success($"{ids.Count} {idType.Name} assets", ids);
        }

        [McpTool("Larje.CreateId", CREATE_DESCRIPTION, "Create Larje Id", EnabledByDefault = true, Groups = new[] { "larje", "larje-assets" })]
        public static object Create(CreateParams parameters)
        {
            Type idType = ResolveType(parameters.Type);
            if (idType == null)
            {
                return Response.Error($"Unknown id type '{parameters.Type}'");
            }

            if (string.IsNullOrWhiteSpace(parameters.Name))
            {
                return Response.Error("Name is required");
            }

            LarjeId id = string.IsNullOrEmpty(parameters.Folder)
                ? LarjeIdAssets.Create(idType, parameters.Name)
                : LarjeIdAssets.Create(idType, parameters.Name, parameters.Folder);
            return Response.Success("Created", new { key = id.Key, name = id.name, path = AssetDatabase.GetAssetPath(id) });
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

        public sealed class ListParams
        {
            [McpDescription("Id type name, e.g. CurrencyId; empty to list types")]
            public string Type { get; set; }
        }

        public sealed class CreateParams
        {
            [McpDescription("Id type name, e.g. SoundId", Required = true)]
            public string Type { get; set; }

            [McpDescription("Asset name; also used as key", Required = true)]
            public string Name { get; set; }

            [McpDescription("Optional root folder, default Assets/Configs/Ids")]
            public string Folder { get; set; }
        }
    }
}
