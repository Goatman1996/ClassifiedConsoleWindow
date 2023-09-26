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
#if UNITY_2021_1_OR_NEWER
            base.selectionChanged += this.OnItemClick;
            base.itemsChosen += this.DoubleClick;
            base.fixedItemHeight = 40;
#else
            base.onSelectionChange += this.OnItemClick;
            base.onItemsChosen += this.DoubleClick;
            base.itemHeight = 40;
#endif
            base.makeItem = this.OnMakeItem;
            base.bindItem = this.BindItem;
            base.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            base.selectionType = SelectionType.Single;


            this.RegisterCallback<GeometryChangedEvent>(this.OnGeometryChanged);
            this.RegisterCallback<KeyDownEvent>(this.OnFKeyDown);

            this.style.minHeight = 100f;

            var type = typeof(ListView);
            while (this.internalScrollView == null && type != typeof(object))
            {
                foreach (var f in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (f.Name == "m_ScrollView")
                    {
                        this.internalScrollView = f.GetValue(this) as ScrollView;
                        break;
                    }
                }
                type = type.BaseType;
            }

        }

        private void OnFKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == UnityEngine.KeyCode.F)
            {
                if (this.lastClick != null)
                {
                    base.ScrollToItem(this.lastClick.Value);
                }
            }
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
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
            this.needToBottom = this.NeedToBottom();

            this.showingLogIndexList.Clear();
            this.showingLogIndexList.AddRange(showingLog);

#if UNITY_2021_1_OR_NEWER
            base.Rebuild();
#else
            base.Refresh();
#endif
        }

        bool needToBottom;
        private bool NeedToBottom()
        {
            if (this.internalScrollView == null) return false;
            var maxValue = this.internalScrollView.verticalScroller.highValue;
            var currentScrollValue = this.internalScrollView.verticalScroller.value;
#if UNITY_2021_1_OR_NEWER
            var need = maxValue - currentScrollValue < base.fixedItemHeight * 0.5f;
            return need;
#else
            var need = maxValue - currentScrollValue < base.itemHeight * 0.5f;
            return need;
#endif

        }

        private void BackToBottom()
        {
            base.ScrollToItem(-1);
        }

        public void TryBackToBottom()
        {
            if (needToBottom)
            {
                BackToBottom();
            }
        }
    }
}