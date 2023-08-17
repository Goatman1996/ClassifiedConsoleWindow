using System;
using System.Reflection;
using ClassifiedConsole.Runtime;
using UnityEngine.UIElements;
using UnityEngine;

namespace ClassifiedConsole.Editor
{
    internal class CmdView : VisualElement
    {
        private TextField cmdInput;
        private Button executeBtn;

        public ClassifiedConsole.Runtime.Cmd cmdExecuter;

        public CmdView()
        {
            this.style.flexDirection = FlexDirection.Row;

            this.cmdInput = new TextField();
            this.cmdInput.RegisterCallback<UnityEngine.UIElements.KeyDownEvent>(this.OnKeyDown);
            this.cmdInput.RegisterCallback<UnityEngine.UIElements.FocusInEvent>(this.OnInputFocusInChanged);
            this.cmdInput.RegisterCallback<UnityEngine.UIElements.FocusOutEvent>(this.OnInputFocusOutChanged);
            this.cmdInput.RegisterValueChangedCallback(this.OnCmdInputChanged);
            this.cmdInput.style.flexGrow = 1;
            this.Add(this.cmdInput);

            this.executeBtn = new Button();
            this.executeBtn.text = "Run";
            this.executeBtn.clicked += this.OnClickExecuteBtn;
            this.Add(this.executeBtn);

            this.cmdExecuter = new Cmd();

            this.ExecuteBorderColor = Color.red;
        }

        private void OnInputFocusInChanged(FocusInEvent evt)
        {
            CmdTipListView.Instance.display = false;
        }

        private void OnInputFocusOutChanged(FocusOutEvent evt)
        {
            CmdTipListView.Instance.display = false;
        }

        private void OnKeyDown(UnityEngine.UIElements.KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return)
            {
                this.OnClickExecuteBtn();
            }
            if (evt.keyCode == KeyCode.UpArrow)
            {
                this.cmdInput.value = CmdTipListView.Instance.GetUp();
            }
            if (evt.keyCode == KeyCode.DownArrow)
            {
                this.cmdInput.value = CmdTipListView.Instance.GetDown();
            }
        }

        private void OnCmdInputChanged(ChangeEvent<string> evt)
        {
            this.cmdExecuter.cmdContent = evt.newValue;

            if (this.cmdExecuter.cmdValid)
            {
                this.ExecuteBorderColor = Color.green;
            }
            else
            {
                this.ExecuteBorderColor = Color.red;
            }
        }


        private void OnClickExecuteBtn()
        {
            if (string.IsNullOrEmpty(this.cmdExecuter.cmdContent))
            {
                return;
            }
            if (ClassifiedConsoleWindow.windowRoot.editorLogFile.isRemote)
            {
                this.CallRemote();
                return;
            }
            if (this.cmdExecuter.cmdValid)
            {
                var result = this.cmdExecuter.ExecuteCmd();
                if (result)
                {
                    CmdTipListView.Instance.AddRecord(this.cmdExecuter.cmdContent);
                    CmdTipListView.Instance.SerializeRecord();
                    this.cmdInput.value = "";
                }
            }
        }

        private async void CallRemote()
        {
            var requestParam = new LogFileNetRequestParam();
            requestParam.Cmd = this.cmdExecuter.cmdContent;
            var requestJson = requestParam.ToJson();
            var result = await ClassifiedConsoleWindow.windowRoot.editorLogFile.remoteLogRequestor.PostAsync(requestJson, false);
            var responseParam = LogFileNetResponseParam.FromJson(result);
            if (responseParam.cmdExecuteSuccess)
            {
                CmdTipListView.Instance.AddRecord(this.cmdExecuter.cmdContent);
                CmdTipListView.Instance.SerializeRecord();
                this.cmdInput.value = "";
            }
        }

        private Color ExecuteBorderColor
        {
            set
            {
                this.executeBtn.style.borderTopColor = value;
                this.executeBtn.style.borderBottomColor = value;
                this.executeBtn.style.borderLeftColor = value;
                this.executeBtn.style.borderRightColor = value;
            }
        }
    }
}