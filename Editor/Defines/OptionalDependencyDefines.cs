using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Compilation;

namespace Larje.Core.Editor.Defines
{
    [InitializeOnLoad]
    internal static class OptionalDependencyDefines
    {
        private static readonly Dependency[] s_dependencies =
        {
            new Dependency("Zenject", "LARJE_ZENJECT")
        };

        private static readonly NamedBuildTarget[] s_targets =
        {
            NamedBuildTarget.Standalone,
            NamedBuildTarget.Android,
            NamedBuildTarget.iOS,
            NamedBuildTarget.WebGL
        };

        static OptionalDependencyDefines()
        {
            EditorApplication.delayCall += Sync;
        }

        [MenuItem("Tools/Larje/Sync Optional Dependency Defines")]
        private static void Sync()
        {
            HashSet<string> assemblies = new HashSet<string>(
                CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies).Select(x => x.name));

            foreach (NamedBuildTarget target in s_targets)
            {
                SyncTarget(target, assemblies);
            }
        }

        private static void SyncTarget(NamedBuildTarget target, HashSet<string> assemblies)
        {
            string current = PlayerSettings.GetScriptingDefineSymbols(target);
            List<string> symbols = current
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            bool changed = false;
            foreach (Dependency dependency in s_dependencies)
            {
                bool present = assemblies.Contains(dependency.AssemblyName);
                bool defined = symbols.Contains(dependency.Define);

                if (present && !defined)
                {
                    symbols.Add(dependency.Define);
                    changed = true;
                }
                else if (!present && defined)
                {
                    symbols.Remove(dependency.Define);
                    changed = true;
                }
            }

            if (changed)
            {
                PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", symbols));
            }
        }

        private readonly struct Dependency
        {
            public readonly string AssemblyName;
            public readonly string Define;

            public Dependency(string assemblyName, string define)
            {
                AssemblyName = assemblyName;
                Define = define;
            }
        }
    }
}
