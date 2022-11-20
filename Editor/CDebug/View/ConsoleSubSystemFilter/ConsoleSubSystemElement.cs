using System;
using ClassifiedConsole.Runtime;
using UnityEngine.UIElements;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleSubSystemElement : VisualElement
    {
        private Label systemName;
        private Toggle showToggle;

        private ConsoleSubSystemElementLevel logCountView;
        private ConsoleSubSystemElementLevel warningCountView;
        private ConsoleSubSystemElementLevel errorCountView;
        private ConsoleSubSystemElementLevel exceptionCountView;

        private VisualElement logCountContainer;

        public ConsoleSubSystemElement()
        {
            this.systemName = new Label();
            this.systemName.style.unityTextAlign = UnityEngine.TextAnchor.MiddleLeft;
            this.Add(this.systemName);


            this.logCountContainer = new VisualElement();
            this.logCountContainer.style.flexDirection = FlexDirection.Row;
            // this.logCount = new Label();
            // this.logCount.style.unityTextAlign = UnityEngine.TextAnchor.MiddleCenter;

            this.logCountView = new ConsoleSubSystemElementLevel(LogLevel.Log);
            this.warningCountView = new ConsoleSubSystemElementLevel(LogLevel.Warning);
            this.errorCountView = new ConsoleSubSystemElementLevel(LogLevel.Error);
            this.exceptionCountView = new ConsoleSubSystemElementLevel(LogLevel.Exception);
            this.logCountContainer.Add(this.logCountView);
            this.logCountContainer.Add(this.warningCountView);
            this.logCountContainer.Add(this.errorCountView);
            this.logCountContainer.Add(this.exceptionCountView);

            this.showToggle = new Toggle();
            this.showToggle.SetValueWithoutNotify(true);
            this.logCountContainer.Add(this.showToggle);

            this.Add(this.logCountContainer);

            this.style.flexDirection = FlexDirection.Row;
            this.style.justifyContent = Justify.SpaceBetween;

            this.RegisterCallback<MouseDownEvent>(OnElementClick);
            this.showToggle.RegisterValueChangedCallback(this.OnToggleClick);
        }

        private void OnToggleClick(ChangeEvent<bool> evt)
        {
            var newValue = evt.newValue;
            this.showToggle.SetValueWithoutNotify(newValue);
            CDebugSubSystemEnumConfig.SetSubSystemPrefs(subSystemId, newValue);
            ClassifiedConsoleWindow.windowRoot.FilterTryNotify();
        }

        private void OnElementClick(MouseDownEvent evt)
        {
            var newValue = !this.showToggle.value;
            this.showToggle.SetValueWithoutNotify(newValue);
            CDebugSubSystemEnumConfig.SetSubSystemPrefs(subSystemId, newValue);
            ClassifiedConsoleWindow.windowRoot.FilterTryNotify();
        }

        private int _subSystemId;
        public int subSystemId
        {
            get => this._subSystemId;
            set
            {
                this._subSystemId = value;
                var isOn = CDebugSubSystemEnumConfig.IsSubSystemOn(this._subSystemId);
                this.showToggle.SetValueWithoutNotify(isOn);

                this.systemName.text = CDebugSubSystemEnumConfig.GetSubSystemLabel(this._subSystemId);
            }
        }

        public void RefreshLogCount()
        {
            var editorLogFile = ClassifiedConsoleWindow.windowRoot.editorLogFile;
            var logCount = editorLogFile.GetLogCount(this.subSystemId, LogLevel.Log);
            var warningCount = editorLogFile.GetLogCount(this.subSystemId, LogLevel.Warning);
            var errorCount = editorLogFile.GetLogCount(this.subSystemId, LogLevel.Error);
            var exceptionCount = editorLogFile.GetLogCount(this.subSystemId, LogLevel.Exception);


            this.logCountView.count = logCount;
            this.warningCountView.count = warningCount;
            this.errorCountView.count = errorCount;
            this.exceptionCountView.count = exceptionCount;

            // if (logCount == 0)
            // {
            //     if (this.logCountContainer.Contains(this.logCountView))
            //     {
            //         this.logCountContainer.Remove(this.logCountView);
            //     }
            // }
            // else
            // {
            //     if (!this.logCountContainer.Contains(this.logCountView))
            //     {
            //         this.logCountContainer.Add(this.logCountView);
            //     }
            // }
            // this.systemLogCount = logCount;
        }
    }
}