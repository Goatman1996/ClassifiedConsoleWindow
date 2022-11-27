using UnityEngine;
using System.IO;

namespace ClassifiedConsole.Runtime
{
    public class LogFilePathConfig
    {
        public static string root
        {
            get
            {
                if (UnityEngine.Application.isEditor)
                {
                    var assetPath = Application.dataPath;
                    var root = Path.GetDirectoryName(assetPath);
                    var ret = Path.Combine(root, "ClassifiedConsole");
                    return ret;
                }
                else
                {
                    var ret = Path.Combine(UnityEngine.Application.persistentDataPath, "ClassifiedConsole");
                    return ret;
                }
            }
        }

        public static string versionRoot
        {
            get
            {
                var ret = Path.Combine(root, CDebugConfig.version);
                return ret;
            }
        }
    }
}