using ClassifiedConsole.Runtime;
using UnityEngine.UIElements;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleLogView : TwoPaneSplitView
    {
        public ConsoleLogLayout logLayout { get => this.top; }
        public ConsoleLogDetail logDetail { get => this.down; }

        private ConsoleLogLayout top;
        private ConsoleLogDetail down;

        public ConsoleLogView()
        {
            base.fixedPaneIndex = 0;
            base.fixedPaneInitialDimension = CDebugConfig.LogLayoutAndDetailRate;
            base.orientation = TwoPaneSplitViewOrientation.Vertical;

            this.top = new ConsoleLogLayout();
            this.Add(top);
            this.down = new ConsoleLogDetail();
            this.Add(this.down);

            this.top.onLogClick += this.down.ShowLog;
        }
    }
}
