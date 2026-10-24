using System.IO;
using UnityEditor;
using UnityEngine;
#if LARJE_ADDRESSABLES
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace Larje.Core.Editor.Tools
{
    public static class AddressablesEditorUtility
    {
        public static bool Available
        {
            get
            {
#if LARJE_ADDRESSABLES
                return true;
#else
                return false;
#endif
            }
        }

        public static bool AddToGroup(string assetPath, string groupName)
        {
#if LARJE_ADDRESSABLES
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogWarning("Addressables settings not found");
                return false;
            }

            AddressableAssetGroup group = string.IsNullOrWhiteSpace(groupName) ? settings.DefaultGroup : settings.FindGroup(groupName);
            if (group == null)
            {
                group = settings.DefaultGroup;
            }

            AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(assetPath), group, false, false);
            entry.address = Path.GetFileNameWithoutExtension(assetPath);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
            return true;
#else
            return false;
#endif
        }
    }
}
