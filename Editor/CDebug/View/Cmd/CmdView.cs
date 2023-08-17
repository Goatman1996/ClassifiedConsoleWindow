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
        private Assembly asm;

        public CmdView()
        {
            this.style.flexDirection = FlexDirection.Row;

            this.cmdInput = new TextField();
            this.cmdInput.style.fontSize = 24;
            // this.cmdInput.isDelayed = true;
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

            this.asm = Assembly.Load("Assembly-CSharp");

            this.ExecuteBorderColor = Color.red;
        }

        private void OnInputFocusInChanged(FocusInEvent evt)
        {
            // Debug.Log("In");
        }

        private void OnInputFocusOutChanged(FocusOutEvent evt)
        {
            // Debug.Log("Out");
        }

        private void OnKeyDown(UnityEngine.UIElements.KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return)
            {
                if (this.CheckContentValid)
                {
                    this.OnClickExecuteBtn();
                }
            }
        }

        private void OnCmdInputChanged(ChangeEvent<string> evt)
        {
            if (this.CheckContentValid)
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
            if (CheckContentValid)
            {
                this.inputMethod.Invoke(null, null);
            }
        }

        private bool CheckContentValid
        {
            get
            {
                return inputMethod != null;
            }
        }

        private Type inputType
        {
            get
            {
                var content = this.cmdInput.text;

                var dotSplit = content.Split('.');
                if (dotSplit.Length <= 1) return null;

                var lastDot = content.LastIndexOf('.');
                var typeString = content.Substring(0, lastDot);
                var methodString = content.Substring(lastDot + 1);

                if (string.IsNullOrEmpty(methodString)) return null;

                var type = this.asm.GetType(typeString);
                if (type == null) return null;

                return type;
            }
        }


        private MethodInfo inputMethod
        {
            get
            {
                var content = this.cmdInput.text;

                var dotSplit = content.Split('.');
                if (dotSplit.Length <= 1) return null;

                var lastDot = content.LastIndexOf('.');
                var typeString = content.Substring(0, lastDot);
                var methodString = content.Substring(lastDot + 1);

                if (string.IsNullOrEmpty(methodString)) return null;

                var type = this.asm.GetType(typeString);
                if (type == null) return null;

                var method = type.GetMethod(methodString, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (method == null) return null;

                return method;
            }
        }

        private Color ExecuteBorderColor
        {
            get => this.executeBtn.style.borderTopColor.value;
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