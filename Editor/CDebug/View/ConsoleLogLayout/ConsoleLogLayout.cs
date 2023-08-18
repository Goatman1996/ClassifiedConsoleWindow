using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Reflection;
using System.Threading.Tasks;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleLogLayout : ListView
    {
        public List<int> showingLogIndexList;
        private ScrollView internalScrollView;
        private Dictionary<int, ConsoleLogElement> elementDic;


        public ConsoleLogLayout()
        {
            this.elementDic = new Dictionary<int, ConsoleLogElement>();
            this.showingLogIndexList = new List<int>();

            base.itemsSource = this.showingLogIndexList;
            base.itemHeight = 40;
            base.makeItem = this.OnMakeItem;
            base.bindItem = this.BindItem;
            base.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            base.selectionType = SelectionType.Single;
            base.onSelectionChange += this.OnItemClick;
            base.onItemsChosen += this.DoubleClick;
            this.RegisterCallback<GeometryChangedEvent>(this.OnGeometryChanged);

            this.style.minHeight = 100f;

            var type = typeof(ListView);
            foreach (var f in type.GetRuntimeFields())
            {
                if (f.Name == "m_ScrollView")
                {
                    this.internalScrollView = f.GetValue(this) as ScrollView;
                }
            }
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            var needToBottom = this.NeedToBottom();
            if (needToBottom)
            {
                this.BackToBottom();
            }
            CDebugWindowConfig.LogLayoutAndDetailRate = evt.newRect.height;
        }

        private void DoubleClick(IEnumerable<object> obj)
        {
            var filePath = ConsoleLogDetail.firstOpenableFile;
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            OpenCsUtility.OpenCsFile(filePath, ConsoleLogDetail.firstOpenableLine);
        }

        public int? lastClick = null;
        private void OnItemClick(IEnumerable<object> obj)
        {
            var logIndex = 0;
            foreach (var o in obj)
            {
                logIndex = (int)o;
                break;
            }
            this.onLogClick?.Invoke(logIndex);

            if (this.lastClick != null)
            {
                if (this.elementDic.ContainsKey(this.lastClick.Value))
                {
                    this.elementDic[this.lastClick.Value].SetChosenState(false);
                }
            }
            this.lastClick = logIndex;

            if (this.elementDic.ContainsKey(this.lastClick.Value))
            {
                this.elementDic[this.lastClick.Value].SetChosenState(true);
            }
        }

        private VisualElement OnMakeItem()
        {
            return new ConsoleLogElement();
        }

        private void BindItem(VisualElement ui, int index)
        {
            var logIndex = this.showingLogIndexList[index];
            var logReader = ClassifiedConsoleWindow.windowRoot.editorLogFile[logIndex];
            var logElement = (ui as ConsoleLogElement);
            logElement.logReader = logReader;
            this.elementDic[logIndex] = logElement;
            logElement.SetChosenState(this.lastClick == logIndex);
        }

        public event Action<int> onLogClick;

        public void Refresh(List<int> showingLog)
        {
            this.elementDic.Clear();
            var needToBottom = this.NeedToBottom();

            this.showingLogIndexList.Clear();
            this.showingLogIndexList.AddRange(showingLog);

            base.Refresh();

            // 不能写在这。 Refresh 后 里面的value就变了
            // needToBottom = this.NeedToBottom();
            if (needToBottom)
            {
                this.BackToBottom();
            }
        }

        private bool NeedToBottom()
        {
            var maxValue = this.internalScrollView.verticalScroller.highValue;
            var currentScrollValue = this.internalScrollView.verticalScroller.value;
            var need = maxValue - currentScrollValue < base.itemHeight * 0.5f;
            return need;
        }

        private void BackToBottom()
        {
            var count = base.itemsSource.Count;
            // 手动计算，刷新最大高度
            this.internalScrollView.verticalScroller.highValue = base.itemHeight * count - this.contentRect.height;
            base.ScrollToItem(base.itemsSource.Count - 1);
        }
    }
}