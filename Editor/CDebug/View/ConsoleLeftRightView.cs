using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleLeftRightView : TwoPaneSplitView
    {
        public ConsoleSubSystemFilter subSystemFilter { get => this.left; }
        public ConsoleLogLayout logLayout { get => this.right.logLayout; }
        public ConsoleLogDetail logDetail { get => this.right.logDetail; }

        private ConsoleSubSystemFilter left;
        private ConsoleLogView right;

        public ConsoleLeftRightView()
        {
            base.fixedPaneIndex = 0;
            base.fixedPaneInitialDimension = 250;
            base.orientation = TwoPaneSplitViewOrientation.Horizontal;

            this.left = new ConsoleSubSystemFilter();
            this.Add(left);
            this.right = new ConsoleLogView();
            this.Add(this.right);
        }
    }
}