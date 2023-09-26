using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.IMGUI.Controls;
using ClassifiedConsole.Runtime;
using System;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleSubSystemFilter : VisualElement
    {
        private List<int> subSystemList;
        private ListView subSystemLayout;
        private List<ConsoleSubSystemElement> managedElement;

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
#if UNITY_2021_1_OR_NEWER
            this.subSystemLayout.fixedItemHeight = 23;
#else
            this.subSystemLayout.itemHeight = 23;
#endif
            this.subSystemLayout.makeItem = this.OnMakeItem;
            this.subSystemLayout.bindItem = this.BindItem;
            this.subSystemLayout.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            this.subSystemLayout.style.flexGrow = 1;
            this.Add(this.subSystemLayout);

            this.style.flexGrow = 1;

            this.managedElement = new List<ConsoleSubSystemElement>();
            this.InitSubSystem();
        }

        // private List<ConsoleSubSystemElement> managedElement;
        private VisualElement OnMakeItem()
        {
            var element = new ConsoleSubSystemElement();
            this.managedElement.Add(element);
            return element;
        }

        private void BindItem(VisualElement ui, int index)
        {
            var content = this.subSystemList[index];
            var element = (ui as ConsoleSubSystemElement);
            var subSystemId = this.subSystemList[index];
            element.subSystemId = subSystemId;
            element.name = subSystemId.ToString();
            element.RefreshLogCount();
        }

        private void InitSubSystem()
        {
            if (this.subSystemList.Count != ClassifiedConsoleWindow.windowRoot.editorLogFile.subSystem_Dic.Count)
            {
                this.managedElement.Clear();
                this.subSystemList.Clear();
                // var subSystemIEnum = ClassifiedConsoleWindow.windowRoot.editorLogFile.GetShowingSubSystem();
                // var subSystemIEnum = CDebugSubSystemEnumConfig.GetAllSubSystemList();
                this.subSystemList.AddRange(ClassifiedConsoleWindow.windowRoot.editorLogFile.subSystem_Dic.Keys);
#if UNITY_2021_1_OR_NEWER
                subSystemLayout.Rebuild();
#else
                subSystemLayout.Refresh();
#endif
            }
            else
            {
                foreach (var element in this.managedElement)
                {
                    element.RefreshLogCount();
                }
            }
        }

        public void RefreeshSubSystem()
        {
            this.InitSubSystem();
        }
    }
}
