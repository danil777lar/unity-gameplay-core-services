using System;
using System.IO;

namespace Larje.Core.Editor.Cli
{
    public static class CliText
    {
        public static string Resolve(string inline, string file, string inlineName)
        {
            if (!string.IsNullOrEmpty(inline) && !string.IsNullOrEmpty(file))
            {
                throw new ArgumentException($"Pass either --{inlineName} or --file, not both");
            }

            if (!string.IsNullOrEmpty(inline))
            {
                return inline;
            }

            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentException($"--{inlineName} or --file is required");
            }

            string path = Path.GetFullPath(file);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File not found: {path}");
            }

            return File.ReadAllText(path);
        }
    }
}
