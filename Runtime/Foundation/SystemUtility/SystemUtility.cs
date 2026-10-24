using System.Diagnostics;
using UnityEngine;

namespace Larje.Core.SystemUtility
{
    public static class SystemUtility
    {
        public static bool FileExplorerAvailable
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                return true;
#else
                return false;
#endif
            }
        }

        public static void CopyToClipboard(string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }

        public static void OpenInFileExplorer(string path)
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            Process.Start("explorer.exe", "/select," + path.Replace('/', '\\'));
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            Process.Start("open", "-R \"" + path + "\"");
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
            Process.Start(new ProcessStartInfo
            {
                FileName = "xdg-open",
                Arguments = "\"" + System.IO.Path.GetDirectoryName(path) + "\"",
                UseShellExecute = false
            });
#endif
        }
    }
}
