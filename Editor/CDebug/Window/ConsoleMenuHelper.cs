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
            var path = ClassifiedConsole.Runtime.LogFilePathConfig.versionRoot;
            EditorUtility.OpenWithDefaultApp(path);
        }

        [MenuItem("Window/Classified/Reset/CleanArchive", false, 7)]
        private static void CleanArchive()
        {
            ClassifiedConsole.Runtime.ManagedLogFile.CleanUpManagedLogFile();
        }
    }
}