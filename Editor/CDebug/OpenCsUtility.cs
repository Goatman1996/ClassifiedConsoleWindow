using System;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace ClassifiedConsole.Editor
{
    internal static class OpenCsUtility
    {
        public static event Action<Dictionary<string, string>> hyperLinkClicked;

        public static void Init()
        {
#if UNITY_2021_1_OR_NEWER
            EditorGUI.hyperLinkClicked += OnClickHyperLink;
#else
            var type = typeof(EditorGUI);
            var evt = type.GetEvent("hyperLinkClicked", BindingFlags.Static | BindingFlags.NonPublic);
            if (evt == null) return;
            var addMethod = evt.GetAddMethod(true);
            var evtArg = new EventHandler(OnClickHyperLink);
            addMethod.Invoke(null, new object[] { evtArg });
#endif
        }

#if UNITY_2021_1_OR_NEWER
        private static void OnClickHyperLink(EditorWindow window, UnityEditor.HyperLinkClickedEventArgs args)
        {
            var infos = args.hyperLinkData;

            bool hasFilePath = infos.TryGetValue("hrefPath", out string filePath);
            bool hasLineString = infos.TryGetValue("line", out string lineString);
            if (!hasFilePath || !hasLineString)
            {
                return;
            }

            hyperLinkClicked?.Invoke(infos);
        }
#else
        private static void OnClickHyperLink(object sender, EventArgs e)
        {
            var evtArgsType = e.GetType();
            var infosPropty = evtArgsType.GetProperty("hyperlinkInfos");
            var infos = infosPropty.GetValue(e) as Dictionary<string, string>;

            bool hasFilePath = infos.TryGetValue("hrefPath", out string filePath);
            bool hasLineString = infos.TryGetValue("line", out string lineString);
            if (!hasFilePath || !hasLineString)
            {
                return;
            }

            hyperLinkClicked?.Invoke(infos);
        }
#endif

        public static void OpenCsFile(string filePath, int line)
        {
            var editorAssembly = typeof(UnityEditor.EditorGUI).Assembly;
            var internalOpenClass = editorAssembly.GetType("UnityEditor.LogEntries");
            var openMethd = internalOpenClass.GetMethod("OpenFileOnSpecificLineAndColumn", BindingFlags.Static | BindingFlags.Public);
            openMethd.Invoke(null, new object[] { filePath, line, -1 });
        }
    }
}