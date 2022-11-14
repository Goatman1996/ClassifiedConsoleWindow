using System;
using ClassifiedConsole.Runtime;
using UnityEngine.UIElements;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleSubSystemElement : VisualElement
    {
        private Label systemName;
        private Toggle showToggle;
        private Label logCount;

        public ConsoleSubSystemElement()
        {
            this.systemName = new Label();
            this.systemName.style.unityTextAlign = UnityEngine.TextAnchor.MiddleLeft;
            this.Add(this.systemName);


            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            this.logCount = new Label();
            this.logCount.style.unityTextAlign = UnityEngine.TextAnchor.MiddleCenter;
            this.systemLogCount = 1000;
            this.showToggle = new Toggle();
            this.showToggle.SetValueWithoutNotify(true);
            container.Add(this.logCount);
            container.Add(this.showToggle);

            this.Add(container);

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

                this.systemName.text = CDebugSubSystemEnumConfig.GetSubSystemName(this._subSystemId);
            }
        }

        private int systemLogCount
        {
            set
            {
                if (value >= 1000)
                {
                    this.logCount.text = "999+";
                }
                else
                {
                    this.logCount.text = value.ToString();
                }
            }
        }

        public void RefreshLogCount()
        {
            var logCount = ClassifiedConsoleWindow.windowRoot.editorLogFile.GetLogCount(this.subSystemId);
            this.systemLogCount = logCount;
        }
    }
}