using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Compilation;

namespace Larje.Core.Editor.Defines
{
    [InitializeOnLoad]
    public static class OptionalDependencyDefines
    {
        private static readonly List<Dependency> s_dependencies = new List<Dependency>
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

        public static void Register(string assemblyName, string define)
        {
            if (!s_dependencies.Exists(x => x.AssemblyName == assemblyName && x.Define == define))
            {
                s_dependencies.Add(new Dependency(assemblyName, define));
            }
        }

        [MenuItem("Tools/Larje/Sync Optional Dependency Defines")]
        private static void Sync()
        {
            HashSet<string> assemblies = new HashSet<string>(
                CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies).Select(x => x.name));
            foreach (string path in CompilationPipeline.GetPrecompiledAssemblyPaths(CompilationPipeline.PrecompiledAssemblySources.UserAssembly))
            {
                assemblies.Add(Path.GetFileNameWithoutExtension(path));
            }

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
            foreach (string define in s_dependencies.Select(x => x.Define).Distinct())
            {
                bool present = s_dependencies.Exists(x => x.Define == define && assemblies.Contains(x.AssemblyName));
                bool defined = symbols.Contains(define);

                if (present && !defined)
                {
                    symbols.Add(define);
                    changed = true;
                }
                else if (!present && defined)
                {
                    symbols.Remove(define);
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
