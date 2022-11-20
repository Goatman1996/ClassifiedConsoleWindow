using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleLogDetail : IMGUIContainer
    {
        public ConsoleLogDetail()
        {
            this.onGUIHandler += this.OnGUI;
            this.style.minHeight = 100f;
            this.RegisterCallback<GeometryChangedEvent>(this.onGuiChanged);
            this.herfBuider = new StringBuilder();

            if (!hasAddLinkClicker)
            {
                OpenCsUtility.hyperLinkClicked += this.OnClickHyperLink;
                hasAddLinkClicker = true;
            }
        }

        private static bool hasAddLinkClicker = false;
        private void OnClickHyperLink(object sender, EventArgs e)
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

            int line = Int32.Parse(lineString);
            OpenCsUtility.OpenCsFile(filePath, line);
            // LogEntries.OpenFileOnSpecificLineAndColumn(filePath, line, -1);
        }

        Rect rect = new Rect();
        private void onGuiChanged(GeometryChangedEvent evt)
        {
            this.rect = evt.newRect;
        }

        Vector2 m_TextScroll = Vector2.zero;
        GUIContent msgContent = new GUIContent();
        GUIStyle MessageStyle;
        private void OnGUI()
        {
            if (MessageStyle == null)
            {
                MessageStyle = "CN Message";
            }

            m_TextScroll = GUILayout.BeginScrollView(m_TextScroll, "CN Box");

            msgContent.text = detail;
            float height = MessageStyle.CalcHeight(msgContent, rect.width);
            EditorGUILayout.SelectableLabel(detail, MessageStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MinHeight(height + 10));

            GUILayout.EndScrollView();
        }

        private StringBuilder herfBuider;
        public static string firstOpenableFile { get; private set; }
        public static int firstOpenableLine { get; private set; }

        private string GetStackTrackHerfString(string msg, int stackTrackStart)
        {
            firstOpenableFile = null;
            var gettedFirstHref = false;
            this.herfBuider.Clear();
            this.herfBuider.Append(msg.Substring(0, stackTrackStart));
            var stackTrackLines = msg.Substring(stackTrackStart).Split('\n');
            foreach (var line in stackTrackLines)
            {
                var fpIndex = line.IndexOf(") (at ");
                if (fpIndex == -1 || line.StartsWith("ClassifiedConsole.CDebug:LogException"))
                {
                    // 没有文件名
                    // 或者是需要跳过CDebug。LogExceotion的
                    this.herfBuider.AppendLine(line);
                    continue;
                }
                fpIndex += 6;
                // 类型/方法 信息
                this.herfBuider.Append(line.Substring(0, fpIndex));
                // 文件 信息
                var fileAndLineString = line.Substring(fpIndex);
                var fileLineSpliterIndex = fileAndLineString.LastIndexOf(':');
                var filePathString = fileAndLineString.Substring(0, fileLineSpliterIndex);
                var lineString = fileAndLineString.Substring(fileLineSpliterIndex + 1);
                lineString = lineString.Substring(0, lineString.IndexOf(')'));
                this.herfBuider.Append("<a hrefPath=\"" + filePathString + "\" line=\"" + lineString + "\">");
                this.herfBuider.Append(fileAndLineString);
                this.herfBuider.Append("</a>");
                this.herfBuider.AppendLine();

                if (!gettedFirstHref)
                {
                    firstOpenableFile = filePathString;
                    firstOpenableLine = int.Parse(lineString);
                    gettedFirstHref = true;
                }
            }
            return this.herfBuider.ToString();
        }

        public string detail { get; set; }
        public int stackTrackStart { get; set; }

        public void ShowLog(int logIndex)
        {
            var logReader = ClassifiedConsoleWindow.windowRoot.editorLogFile[logIndex];
            this.detail = logReader.msg;
            this.stackTrackStart = logReader.stackTrackStartIndex;
            this.detail = this.GetStackTrackHerfString(this.detail, this.stackTrackStart);

            var instanceId = logReader.instanceId;
            var obj = UnityEditor.EditorUtility.InstanceIDToObject(instanceId);
            UnityEditor.EditorGUIUtility.PingObject(obj);
        }

        public void ClearMsg()
        {
            this.detail = null;
        }
    }
}