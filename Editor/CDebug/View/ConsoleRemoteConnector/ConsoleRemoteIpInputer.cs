using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleRemoteIpInputer : EditorWindow
    {
        public static void Show(Rect rect, Action<string> onInputFinish)
        {
            rect = GUIUtility.GUIToScreenRect(rect);
            rect = new Rect(rect.x, rect.yMax, 300, 50);
            var window = EditorWindow.GetWindowWithRect<ConsoleRemoteIpInputer>(rect, true);
            window.position = rect;
            window.FocusFiedld();

            window.onInputFinish = onInputFinish;
        }

        private Action<string> onInputFinish;
        private VisualElement rootView;
        private TextField ipFiled;
        private Button okBtn;
        private void OnEnable()
        {
            this.rootView = rootVisualElement;

            this.ipFiled = new TextField();
            this.rootView.Add(this.ipFiled);

            this.okBtn = new Button();
            this.okBtn.text = "OK";
            this.okBtn.clicked += this.OnClickOkBtn;
            this.rootView.Add(this.okBtn);
        }

        private void OnClickOkBtn()
        {
            var ip = this.ipFiled.value;
            this.onInputFinish?.Invoke(ip);
            this.Close();
        }

        public void FocusFiedld()
        {
            this.ipFiled.Focus();
        }
    }
}