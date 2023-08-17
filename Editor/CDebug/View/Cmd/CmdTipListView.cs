using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ClassifiedConsole.Editor
{
    internal class CmdTipListView : VisualElement
    {
        internal static CmdTipListView Instance
        {
            get;
            private set;
        }

        public const int MaxRecord = 30;
        public List<string> recordList;

        private Label title;

        public CmdTipListView()
        {
            this.DeserializeRecord();
            this.title = new Label();
            this.title.style.unityTextAlign = TextAnchor.MiddleCenter;
            this.title.style.borderBottomColor = Color.gray;
            this.title.style.borderBottomWidth = 1;
            this.title.text = "History";
            this.Add(this.title);

            this.borderWidth = 1;
            this.borderColor = Color.grey;

            this.display = false;

            Instance = this;
        }

        public void SerializeRecord()
        {
            for (int i = 0; i < MaxRecord; i++)
            {
                var record = "";
                if (recordList.Count > i)
                {
                    record = recordList[i];
                }
                PlayerPrefs.SetString($"CmdTipListView{i}", record);
            }
            this._index = -1;
        }

        private void DeserializeRecord()
        {
            this.recordList = new List<string>(MaxRecord);
            for (int i = 0; i < MaxRecord; i++)
            {
                var record = PlayerPrefs.GetString($"CmdTipListView{i}", "");
                this.recordList.Add(record);
            }
            this.recordList.RemoveAll(x => x == "");
            this._index = -1;
        }

        public void AddRecord(string cmd)
        {
            if (this.recordList.Contains(cmd))
            {
                this.recordList.Remove(cmd);
            }

            if (this.recordList.Count == MaxRecord)
            {
                this.recordList.RemoveAt(MaxRecord - 1);
            }

            this.recordList.Insert(0, cmd);

        }

        private int borderWidth
        {
            set
            {
                this.style.borderTopWidth = value;
                this.style.borderLeftWidth = value;
                this.style.borderRightWidth = value;
                this.style.borderBottomWidth = value;
            }
        }

        private Color borderColor
        {
            set
            {
                this.style.borderTopColor = value;
                this.style.borderLeftColor = value;
                this.style.borderRightColor = value;
                this.style.borderBottomColor = value;
            }
        }

        private int _index = -1;
        public int index
        {
            get
            {
                return this._index;
            }
            set
            {
                _index = Mathf.Clamp(value, -1, this.recordList.Count - 1);
            }
        }

        public string GetUp()
        {
            this.index++;
            if (this.index == -1)
            {
                return "";
            }
            else
            {
                return this.recordList[index];
            }
        }

        public string GetDown()
        {
            this.index--;
            if (this.index == -1)
            {
                return "";
            }
            else
            {
                return this.recordList[index];
            }
        }

        public bool display
        {
            get => this.style.display == DisplayStyle.Flex;
            set => this.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}