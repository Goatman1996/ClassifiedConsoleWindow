using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ClassifiedConsole.Editor
{
    internal class CmdHistory
    {
        internal static CmdHistory Instance
        {
            get;
            private set;
        }

        public const int MaxRecord = 30;
        public List<string> recordList;

        public CmdHistory()
        {
            this.DeserializeRecord();

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
    }
}