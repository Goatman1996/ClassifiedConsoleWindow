using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.IMGUI.Controls;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleSubSystemFilter : VisualElement
    {
        private List<int> subSystemList;
        private ListView subSystemLayout;

        public ConsoleSubSystemFilter()
        {
            this.style.minWidth = 170;

            var deleteAndAddContainer = new VisualElement();
            deleteAndAddContainer.style.flexDirection = FlexDirection.Row;
            deleteAndAddContainer.style.borderTopWidth = 3;
            deleteAndAddContainer.style.borderTopColor = UnityEngine.Color.grey;
            deleteAndAddContainer.style.borderBottomWidth = 3;
            deleteAndAddContainer.style.borderBottomColor = UnityEngine.Color.grey;
            deleteAndAddContainer.style.borderLeftWidth = 3;
            deleteAndAddContainer.style.borderLeftColor = UnityEngine.Color.grey;
            deleteAndAddContainer.style.borderRightWidth = 3;
            deleteAndAddContainer.style.borderRightColor = UnityEngine.Color.grey;
            this.Add(deleteAndAddContainer);

            var openAllBtn = new Button();
            openAllBtn.text = $"Open All";
            openAllBtn.clicked += () =>
            {
                CDebugSubSystemEnumConfig.SetAllState(true);
                ClassifiedConsoleWindow.windowRoot.FilterTryNotify();
            };
            deleteAndAddContainer.Add(openAllBtn);

            var closeAllBtn = new Button();
            closeAllBtn.text = $"Close All";
            closeAllBtn.clicked += () =>
            {
                CDebugSubSystemEnumConfig.SetAllState(false);
                ClassifiedConsoleWindow.windowRoot.FilterTryNotify();
            };
            deleteAndAddContainer.Add(closeAllBtn);

            this.subSystemList = new List<int>();

            this.subSystemLayout = new ListView();
            this.subSystemLayout.itemsSource = this.subSystemList;
            this.subSystemLayout.itemHeight = 23;
            this.subSystemLayout.makeItem = this.OnMakeItem;
            this.subSystemLayout.bindItem = this.BindItem;
            this.subSystemLayout.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            this.subSystemLayout.style.flexGrow = 1;
            this.Add(this.subSystemLayout);

            this.style.flexGrow = 1;

            // this.managedElement = new List<ConsoleSubSystemElement>();
            this.InitSubSystem();
        }

        // private List<ConsoleSubSystemElement> managedElement;
        private VisualElement OnMakeItem()
        {
            var element = new ConsoleSubSystemElement();
            // this.managedElement.Add(element);
            return element;
        }

        private void BindItem(VisualElement ui, int index)
        {
            var content = this.subSystemList[index];
            var element = (ui as ConsoleSubSystemElement);
            var subSystemId = this.subSystemList[index];
            element.subSystemId = subSystemId;
            element.RefreshLogCount();
        }

        private void InitSubSystem()
        {
            this.subSystemList.Clear();
            // var subSystemIEnum = ClassifiedConsoleWindow.windowRoot.editorLogFile.GetShowingSubSystem();
            var subSystemIEnum = CDebugSubSystemEnumConfig.GetAllSubSystemList();
            this.subSystemList.AddRange(subSystemIEnum);
            this.subSystemLayout.Refresh();
        }

        public void RefreeshSubSystem()
        {
            this.InitSubSystem();
            // foreach (var element in this.managedElement)
            // {
            //     element.RefreshLogCount();
            // }
        }
    }
}