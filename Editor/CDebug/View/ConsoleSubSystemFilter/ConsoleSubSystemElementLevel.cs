using ClassifiedConsole.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleSubSystemElementLevel : VisualElement
    {
        private Image icon;
        private Label countLabel;
        private LogLevel level;

        public ConsoleSubSystemElementLevel(LogLevel level)
        {
            this.level = level;
            var value = CDebugWindowConfig.GetShowLogLevel(this.level);

            this.icon = new Image();
            this.countLabel = new Label();
            this.Add(this.icon);
            this.Add(this.countLabel);

            this.icon.image = ConsoleIconUtility.GetIcon(level);
            this.icon.style.maxWidth = 20;

            this.countLabel.text = "0";
            this.countLabel.style.unityTextAlign = UnityEngine.TextAnchor.MiddleCenter;

            this.style.flexDirection = FlexDirection.Row;
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
                this.style.display = value == 0 ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }
    }
}