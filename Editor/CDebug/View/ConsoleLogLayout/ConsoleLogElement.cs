using System;
using ClassifiedConsole.Runtime;
using UnityEngine.UIElements;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleLogElement : VisualElement
    {
        private Image img;
        private Label label;
        private Label collapseCount;

        public ConsoleLogElement()
        {
            UnityEngine.ColorUtility.TryParseHtmlString("#509EFD", out UnityEngine.Color targetColor);
            this.style.borderLeftColor = targetColor;

            this.collapseCount = new Label();
            this.collapseCount.style.display = DisplayStyle.None;
            this.collapseCount.style.unityTextAlign = UnityEngine.TextAnchor.MiddleCenter;
            this.collapseCount.style.borderRightColor = UnityEngine.Color.gray;
            this.collapseCount.style.borderRightWidth = 2;
            this.collapseCount.style.width = 37;
            this.Add(this.collapseCount);

            this.style.flexDirection = FlexDirection.Row;
            this.img = new Image();
            this.img.style.width = 32;
            this.img.style.height = 32;
            this.img.style.flexShrink = 0f;
            this.img.style.alignSelf = Align.Center;
            this.Add(this.img);

            this.label = new Label();
            this.label.style.unityTextAlign = UnityEngine.TextAnchor.MiddleLeft;
            this.label.style.flexGrow = 1f;
            this.Add(this.label);
        }

        private LogReader _logReader;
        public LogReader logReader
        {
            get => this._logReader;
            set
            {
                this._logReader = value;
                this.RefreshDisplay();
            }
        }

        public int inListViewIndex { get; set; }
        public int logIndex { get; set; }

        private void RefreshDisplay()
        {
            var logLevel = this.logReader.level;
            var subSystem = this.logReader.subSystem;
            this.img.image = ConsoleIconUtility.GetIcon(logLevel);
            var time = this.logReader.time;
            var timeString = time.ToString("HH:mm:ss");
            var content = $"[{timeString}] ";
            var shortMsg = logReader.GetMsg(2);
            shortMsg = ConsoleLogDetail.TryRootToUnityDataPath(shortMsg);
            content += shortMsg;
            this.label.text = content;

            if (CDebugConfig.Collapse)
            {
                var md5 = this.logReader.md5;
                var collapseCount = ClassifiedConsoleWindow.windowRoot.editorLogFile.GetCollapseCount(md5);
                if (collapseCount >= 1000)
                {
                    this.collapseCount.text = "999+";
                }
                else
                {
                    this.collapseCount.text = collapseCount.ToString();
                }
                this.collapseCount.style.display = DisplayStyle.Flex;
            }
            else
            {
                this.collapseCount.style.display = DisplayStyle.None;
            }
        }

        public void SetChosenState(bool chosen)
        {
            if (chosen)
            {
                this.style.borderLeftWidth = 10;
            }
            else
            {
                this.style.borderLeftWidth = 0;
            }
        }
    }
}