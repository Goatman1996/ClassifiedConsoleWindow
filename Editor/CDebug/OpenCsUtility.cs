using System;
using UnityEditor;
using System.Reflection;

namespace ClassifiedConsole.Editor
{
    internal static class OpenCsUtility
    {
        public static event EventHandler hyperLinkClicked
        {
            add
            {
                var type = typeof(EditorGUI);
                var evt = type.GetEvent("hyperLinkClicked", BindingFlags.Static | BindingFlags.NonPublic);
                if (evt == null) return;
                var addMethod = evt.GetAddMethod(true);
                addMethod.Invoke(null, new object[] { value });
            }
            remove
            {
                var type = typeof(EditorGUI);
                var evt = type.GetEvent("hyperLinkClicked", BindingFlags.Static | BindingFlags.NonPublic);
                if (evt == null) return;
                var removeMethod = evt.GetRemoveMethod();
                removeMethod.Invoke(null, new object[] { value });
            }
        }

        public static void OpenCsFile(string filePath, int line)
        {
            var editorAssembly = typeof(UnityEditor.EditorGUI).Assembly;
            var internalOpenClass = editorAssembly.GetType("UnityEditor.LogEntries");
            var openMethd = internalOpenClass.GetMethod("OpenFileOnSpecificLineAndColumn", BindingFlags.Static | BindingFlags.Public);
            openMethd.Invoke(null, new object[] { filePath, line, -1 });
        }
    }
}