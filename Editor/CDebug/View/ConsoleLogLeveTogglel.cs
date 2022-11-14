using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleLogLeveTogglel : ToolbarToggle
    {
        private Image icon;
        private Label countLabel;
        private LogLevel level;

        public ConsoleLogLeveTogglel(LogLevel level)
        {
            this.level = level;
            var value = CDebugConfig.GetShowLogLevel(this.level);
            base.SetValueWithoutNotify(value);

            this.icon = new Image();
            this.countLabel = new Label();
            this.Add(this.icon);
            this.Add(this.countLabel);

            this.icon.image = ConsoleIconUtility.GetIcon(level);
            this.icon.style.maxWidth = 20;

            this.countLabel.text = "0";
            this.countLabel.style.unityTextAlign = UnityEngine.TextAnchor.MiddleCenter;

            this.style.flexDirection = FlexDirection.Row;

            this.RegisterValueChangedCallback(this.OnValueChanged);

        }

        private void OnValueChanged(ChangeEvent<bool> evt)
        {
            CDebugConfig.SetShowLogLevel(this.level, evt.newValue);
            ClassifiedConsoleWindow.windowRoot.FilterTryNotify();
        }

        public int count
        {
            set
            {
                if (value >= 1000)
                {
                    this.countLabel.text = "999+";
                }
                else
                {
                    this.countLabel.text = value.ToString();
                }
            }
        }
    }
}