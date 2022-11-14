using UnityEngine;
using UnityEditor;
using System.IO;

namespace ClassifiedConsole
{
    public static class CDebugSettingsWriter
    {
        public static void Save(this CDebugSettings settings)
        {
            var json = UnityEngine.JsonUtility.ToJson(settings, true);

            var assetPath = Application.dataPath;
            var resourcesPath = Path.Combine(assetPath, "Resources");
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
            }
            var jsonPath = Path.Combine(resourcesPath, "CDebugSettings.json");
            File.WriteAllText(jsonPath, json);
        }

        public static void SaveAndRefreshAssets(this CDebugSettings settings)
        {
            settings.Save();
            AssetDatabase.Refresh();
        }
    }
}