using System.IO;
using UnityEngine;

namespace ClassifiedConsole
{
    public partial class CDebugSettings : ScriptableObject
    {
        private static CDebugSettings LoadOrCreate()
        {
            var ret = Resources.Load<CDebugSettings>("CDebugSettings");
            if (ret == null && Application.isEditor)
            {
                ret = CDebugSettings.CreateInstance<CDebugSettings>();
#if UNITY_EDITOR
                if (!Directory.Exists("Assets/Resources"))
                {
                    Directory.CreateDirectory("Assets/Resources");
                }
                UnityEditor.AssetDatabase.CreateAsset(ret, "Assets/Resources/CDebugSettings.asset");
#endif
            }
            return ret;
        }

        private static CDebugSettings _Instance;
        public static CDebugSettings Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = LoadOrCreate();
                }
                return _Instance;
            }
        }

#if UNITY_EDITOR
        public static void Save()
        {
            UnityEditor.EditorUtility.SetDirty(Instance);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(Instance);
        }
#endif
    }
}