using UnityEditor;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleMenuHelper
    {
        [MenuItem("Window/Classified/Console", false, 6)]
        private static void OpenConsoleWindow()
        {
            EditorWindow.GetWindow<ClassifiedConsoleWindow>("ClassifiedConsole");
        }

        [MenuItem("Window/Classified/ConsoleSettings", false, 6)]
        private static void OpenSettingsWindow()
        {
            EditorWindow.GetWindow<CDebugSettingsWindow>("CDebugSettings");
        }

        [MenuItem("Window/Classified/OpenLogFilePath", false, 6)]
        private static void OpenLogFilePath()
        {
            var path = UnityEngine.Application.persistentDataPath;
            EditorUtility.OpenWithDefaultApp(path);
        }

        [MenuItem("Window/Classified/CleanArchive", false, 6)]
        private static void CleanArchive()
        {
            ClassifiedConsole.Runtime.ManagedLogFile.CleanUpManagedLogFile();
        }
    }
}