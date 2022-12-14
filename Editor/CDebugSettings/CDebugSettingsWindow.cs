using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

namespace ClassifiedConsole.Editor
{
    internal class CDebugSettingsWindow : EditorWindow
    {
        private VisualElement rootView;
        private void OnEnable()
        {
            this.rootView = rootVisualElement;

            {
                var splitToggle = this.CreateSplitLogToggle(out HelpBox helpBox);
                this.rootView.Add(helpBox);
                this.rootView.Add(splitToggle);
            }
            {
                var logWriteLineField = this.CreateLogWriteLineField(out HelpBox helpBox);
                this.rootView.Add(helpBox);
                this.rootView.Add(logWriteLineField);
            }
            {
                var warningWriteLineField = this.CreateWarningWriteLineField(out HelpBox helpBox);
                this.rootView.Add(helpBox);
                this.rootView.Add(warningWriteLineField);
            }
            {
                var errorWriteLineField = this.CreateErrorWriteLineField(out HelpBox helpBox);
                this.rootView.Add(helpBox);
                this.rootView.Add(errorWriteLineField);
            }
            {
                var exceptionWriteLineField = this.CreateExceptionWriteLineField(out HelpBox helpBox);
                this.rootView.Add(helpBox);
                this.rootView.Add(exceptionWriteLineField);
            }
            {
                var msgWithFileInfoToggle = this.CreateMsgWithFileInfoToggle(out HelpBox helpBox);
                this.rootView.Add(helpBox);
                this.rootView.Add(msgWithFileInfoToggle);
            }
            {
                var keepLogFileCountField = this.CreateKeepLogFileCountField(out HelpBox helpBox);
                this.rootView.Add(helpBox);
                this.rootView.Add(keepLogFileCountField);
            }
            {
                var toggle = this.CreateCatchNativeLogToggle(out HelpBox helpBox);
                this.rootView.Add(helpBox);
                this.rootView.Add(toggle);
            }
            {
                var toggle = this.CreateCatchNativeWarningToggle(out HelpBox helpBox);
                this.rootView.Add(helpBox);
                this.rootView.Add(toggle);
            }
            {
                var toggle = this.CreateCatchNativeErrorToggle(out HelpBox helpBox);
                this.rootView.Add(helpBox);
                this.rootView.Add(toggle);
            }
            {
                var toggle = this.CreateCatchNativeExceptionToggle(out HelpBox helpBox);
                this.rootView.Add(helpBox);
                this.rootView.Add(toggle);
            }
            {
                var field = this.CreatePortField(out HelpBox helpBox);
                this.rootView.Add(helpBox);
                this.rootView.Add(field);
            }
            {
                // var field = this.CreateSkipLineField(out HelpBox helpBox);
                // this.rootView.Add(helpBox);
                // this.rootView.Add(field);
            }
            {
                var field = this.CreateAssemblyFiled(out HelpBox helpBox);
                this.rootView.Add(helpBox);
                this.rootView.Add(field);
            }
        }

        private VisualElement CreateSplitLogToggle(out HelpBox helpBox)
        {
            var splitToggle = new Toggle();
            splitToggle.text = nameof(CDebugSettings.Instance.SplitLogFile);
            splitToggle.SetValueWithoutNotify(CDebugSettings.Instance.SplitLogFile);
            splitToggle.RegisterValueChangedCallback((evt) =>
            {
                CDebugSettings.Instance.SplitLogFile = evt.newValue;
                CDebugSettings.Instance.SaveAndRefreshAssets();
            });
            helpBox = new HelpBox("????????????SubSystem????????????LogFile(????????????system?????????????????????)", HelpBoxMessageType.Info);
            return splitToggle;
        }

        private IntegerField CreateLogWriteLineField(out HelpBox helpBox)
        {
            var label = nameof(CDebugSettings.Instance.LogWriteLine);
            var filed = this.CreateIntFiled(label, CDebugSettings.Instance.LogWriteLine, (value) =>
            {
                CDebugSettings.Instance.LogWriteLine = value;
                CDebugSettings.Instance.SaveAndRefreshAssets();
            });
            helpBox = new HelpBox("Log??????????????????(?????????Msg)", HelpBoxMessageType.Info);
            return filed;
        }

        private IntegerField CreateWarningWriteLineField(out HelpBox helpBox)
        {
            var label = nameof(CDebugSettings.Instance.WarningWriteLine);
            var filed = this.CreateIntFiled(label, CDebugSettings.Instance.WarningWriteLine, (value) =>
            {
                CDebugSettings.Instance.WarningWriteLine = value;
                CDebugSettings.Instance.SaveAndRefreshAssets();
            });
            helpBox = new HelpBox("Warning??????????????????(?????????Msg)", HelpBoxMessageType.Info);
            return filed;
        }

        private IntegerField CreateErrorWriteLineField(out HelpBox helpBox)
        {
            var label = nameof(CDebugSettings.Instance.ErrorWriteLine);
            var filed = this.CreateIntFiled(label, CDebugSettings.Instance.ErrorWriteLine, (value) =>
            {
                CDebugSettings.Instance.ErrorWriteLine = value;
                CDebugSettings.Instance.SaveAndRefreshAssets();
            });
            helpBox = new HelpBox("Error??????????????????(?????????Msg)", HelpBoxMessageType.Info);
            return filed;
        }

        private IntegerField CreateExceptionWriteLineField(out HelpBox helpBox)
        {
            var label = nameof(CDebugSettings.Instance.ExceptionWriteLine);
            var filed = this.CreateIntFiled(label, CDebugSettings.Instance.ExceptionWriteLine, (value) =>
            {
                CDebugSettings.Instance.ExceptionWriteLine = value;
                CDebugSettings.Instance.SaveAndRefreshAssets();
            });
            helpBox = new HelpBox("Exception??????????????????(?????????Msg)", HelpBoxMessageType.Info);
            return filed;
        }

        private IntegerField CreateIntFiled(string label, int defaultValue, Action<int> onValueChanged)
        {
            var filed = new IntegerField();
            filed.label = label;
            filed.isDelayed = true;
            filed.SetValueWithoutNotify(defaultValue);
            filed.RegisterValueChangedCallback((evt) =>
            {
                onValueChanged?.Invoke(evt.newValue);
            });
            return filed;
        }

        private Toggle CreateMsgWithFileInfoToggle(out HelpBox helpBox)
        {
            var splitToggle = new Toggle();
            splitToggle.text = nameof(CDebugSettings.Instance.msgWithFileInfo);
            splitToggle.SetValueWithoutNotify(CDebugSettings.Instance.msgWithFileInfo);
            splitToggle.RegisterValueChangedCallback((evt) =>
            {
                CDebugSettings.Instance.msgWithFileInfo = evt.newValue;
                CDebugSettings.Instance.SaveAndRefreshAssets();
            });
            helpBox = new HelpBox("????????????????????????(IL2Cpp???????????????'????????????)", HelpBoxMessageType.Info);
            return splitToggle;
        }

        private IntegerField CreateKeepLogFileCountField(out HelpBox helpBox)
        {
            var label = nameof(CDebugSettings.Instance.keepLogFileCount);
            var filed = this.CreateIntFiled(label, CDebugSettings.Instance.keepLogFileCount, (value) =>
            {
                CDebugSettings.Instance.keepLogFileCount = value;
                CDebugSettings.Instance.SaveAndRefreshAssets();
            });
            helpBox = new HelpBox("????????????LogFile??????(??????????????????????????? ???2)", HelpBoxMessageType.Info);
            return filed;
        }

        private Toggle CreateCatchNativeLogToggle(out HelpBox helpBox)
        {
            var toggle = new Toggle();
            toggle.text = nameof(CDebugSettings.Instance.catchNativeLog);
            toggle.SetValueWithoutNotify(CDebugSettings.Instance.catchNativeLog);
            toggle.RegisterValueChangedCallback((evt) =>
            {
                CDebugSettings.Instance.catchNativeLog = evt.newValue;
                CDebugSettings.Instance.SaveAndRefreshAssets();
            });
            helpBox = new HelpBox("????????????Unity Native Log", HelpBoxMessageType.Info);
            return toggle;
        }

        private Toggle CreateCatchNativeWarningToggle(out HelpBox helpBox)
        {
            var toggle = new Toggle();
            toggle.text = nameof(CDebugSettings.Instance.catchNativeWarning);
            toggle.SetValueWithoutNotify(CDebugSettings.Instance.catchNativeWarning);
            toggle.RegisterValueChangedCallback((evt) =>
            {
                CDebugSettings.Instance.catchNativeWarning = evt.newValue;
                CDebugSettings.Instance.SaveAndRefreshAssets();
            });
            helpBox = new HelpBox("????????????Unity Native Warning", HelpBoxMessageType.Info);
            return toggle;
        }

        private Toggle CreateCatchNativeErrorToggle(out HelpBox helpBox)
        {
            var toggle = new Toggle();
            toggle.text = nameof(CDebugSettings.Instance.catchNativeError);
            toggle.SetValueWithoutNotify(CDebugSettings.Instance.catchNativeError);
            toggle.RegisterValueChangedCallback((evt) =>
            {
                CDebugSettings.Instance.catchNativeError = evt.newValue;
                CDebugSettings.Instance.SaveAndRefreshAssets();
            });
            helpBox = new HelpBox("????????????Unity Native Error", HelpBoxMessageType.Info);
            return toggle;
        }

        private Toggle CreateCatchNativeExceptionToggle(out HelpBox helpBox)
        {
            var toggle = new Toggle();
            toggle.text = nameof(CDebugSettings.Instance.catchNativeException);
            toggle.SetValueWithoutNotify(CDebugSettings.Instance.catchNativeException);
            toggle.RegisterValueChangedCallback((evt) =>
            {
                CDebugSettings.Instance.catchNativeException = evt.newValue;
                CDebugSettings.Instance.SaveAndRefreshAssets();
            });
            helpBox = new HelpBox("????????????Unity Native Exception", HelpBoxMessageType.Info);
            return toggle;
        }

        private IntegerField CreatePortField(out HelpBox helpBox)
        {
            var label = nameof(CDebugSettings.Instance.port);
            var filed = this.CreateIntFiled(label, CDebugSettings.Instance.port, (value) =>
            {
                CDebugSettings.Instance.port = value;
                CDebugSettings.Instance.SaveAndRefreshAssets();
            });
            helpBox = new HelpBox("????????????????????????????????????", HelpBoxMessageType.Info);
            return filed;
        }

        private IntegerField CreateSkipLineField(out HelpBox helpBox)
        {
            var label = nameof(CDebugSettings.Instance.stackSkipLine);
            var filed = this.CreateIntFiled(label, CDebugSettings.Instance.stackSkipLine, (value) =>
            {
                CDebugSettings.Instance.stackSkipLine = value;
                CDebugSettings.Instance.SaveAndRefreshAssets();
            });
            helpBox = new HelpBox("????????????????????????(??????3?????????>=3)", HelpBoxMessageType.Info);
            return filed;
        }

        private TextField CreateAssemblyFiled(out HelpBox helpBox)
        {
            var filed = new TextField();
            filed.label = nameof(CDebugSettings.Instance.subSystemDefinedAssembly);
            filed.SetValueWithoutNotify(CDebugSettings.Instance.subSystemDefinedAssembly);

            filed.isDelayed = true;
            filed.RegisterValueChangedCallback((evt) =>
            {
                CDebugSettings.Instance.subSystemDefinedAssembly = evt.newValue;
                CDebugSettings.Instance.SaveAndRefreshAssets();
            });
            helpBox = new HelpBox("????????????????????????ClassifiedConsole.CDebugSubSystem & CDebugException???????????????????????????", HelpBoxMessageType.Info);
            return filed;
        }
    }

}