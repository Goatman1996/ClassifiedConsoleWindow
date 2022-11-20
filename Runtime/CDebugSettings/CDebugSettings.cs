using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace ClassifiedConsole
{
    [Serializable]
    public class CDebugSettings
    {
        /// <summary>
        /// 是否按分类系统，分开存放Log文件（多系统的，则会放在第一个系统分类里）
        /// </summary>
        public bool SplitLogFile = true;

        [SerializeField] private int _LogWriteLine = 1;
        /// <summary>
        /// 要记录的栈信息(不算Msg)
        /// -1 不限制行数
        /// </summary>
        public int LogWriteLine
        {
            get => this._LogWriteLine;
            set
            {
                var newValue = Mathf.Clamp(value, -1, int.MaxValue);
                this._LogWriteLine = newValue;
            }
        }

        [SerializeField] private int _WarningWriteLine = 1;
        /// <summary>
        /// 要记录的栈信息(不算Msg)
        /// -1 不限制行数
        /// </summary>
        public int WarningWriteLine
        {
            get => this._WarningWriteLine;
            set
            {
                var newValue = Mathf.Clamp(value, -1, int.MaxValue);
                this._WarningWriteLine = newValue;
            }
        }

        [SerializeField] private int _ErrorWriteLine = -1;
        /// <summary>
        /// 要记录的栈信息(不算Msg)
        /// -1 不限制行数
        /// </summary>
        public int ErrorWriteLine
        {
            get => this._ErrorWriteLine;
            set
            {
                var newValue = Mathf.Clamp(value, -1, int.MaxValue);
                this._ErrorWriteLine = newValue;
            }
        }

        [SerializeField] private int _ExceptionWriteLine = -1;
        /// <summary>
        /// 要记录的栈信息(不算Msg)
        /// -1 不限制行数
        /// </summary>
        public int ExceptionWriteLine
        {
            get => this._ExceptionWriteLine;
            set
            {
                var newValue = Mathf.Clamp(value, -1, int.MaxValue);
                this._ExceptionWriteLine = newValue;
            }
        }
        /// <summary>
        /// 栈信息是否包含文件信息（建议出包前设置成false）
        /// </summary>
        public bool msgWithFileInfo = true;

        [SerializeField] private int _keepLogFileCount = 3;
        /// <summary>
        /// 要保留的LogFile数量
        /// </summary>
        public int keepLogFileCount
        {
            get => this._keepLogFileCount;
            set
            {
                var newValue = Mathf.Clamp(value, 1, int.MaxValue);
                this._keepLogFileCount = newValue;
            }
        }
        /// <summary>
        /// 是否监听运行时Log
        /// Runtime Only
        /// </summary>
        public bool catchNativeLog = false;
        /// <summary>
        /// 是否监听运行时Warning
        /// Runtime Only
        /// </summary>
        public bool catchNativeWarning = false;
        /// <summary>
        /// 是否监听运行时Error
        /// Runtime Only
        /// </summary>
        public bool catchNativeError = false;
        /// <summary>
        /// 是否监听运行时Exception
        /// Runtime Only
        /// </summary>
        public bool catchNativeException = false;

        [SerializeField] private int _port = 34599;
        /// <summary>
        /// 是否监听运行时Exception
        /// Runtime Only
        /// </summary>
        public int port
        {
            get => this._port;
            set
            {
                var newValue = Mathf.Clamp(value, 0, 65535);
                this._port = newValue;
            }
        }
        [SerializeField] private int _stackSkipLine = 3;
        /// <summary>
        /// 栈信息要跳过的行数(自定义必须大于3)
        /// Runtime Only
        /// </summary>
        public int stackSkipLine
        {
            get => this._stackSkipLine;
            set
            {
                return;
                // var newValue = Mathf.Clamp(value, 3, int.MaxValue);
                // this._stackSkipLine = newValue;
            }
        }
        /// <summary>
        /// 所有包含了[CDebugSubSystemAttribute] 设置的Assembly，实用','分割
        /// </summary>
        public string subSystemDefinedAssembly = "Assembly-CSharp";

        [SerializeField] private int _windowFPS = 30;
        public int windowFPS
        {
            get
            {
                return Mathf.Clamp(this._windowFPS, 5, 60);
            }
        }

        [SerializeField] private int _limitLogCount = (100 * 100) * 10;
        public int limitLogCount
        {
            get
            {
                // 1万 ~ 100万
                return Mathf.Clamp(this._limitLogCount, 10000, 1000000);
            }
        }

        private static CDebugSettings Load()
        {
            var textAsset = Resources.Load<TextAsset>("CDebugSettings");
            var json = "{}";
            if (textAsset == null)
            {
                json = CreateCDebugSettingsJson();
            }
            else
            {
                json = textAsset.text;
            }
            var ret = UnityEngine.JsonUtility.FromJson<CDebugSettings>(json);
            return ret;
        }

        private static string CreateCDebugSettingsJson()
        {
            var assetPath = Application.dataPath;
            var resourcesPath = Path.Combine(assetPath, "Resources");
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
            }
            var jsonPath = Path.Combine(resourcesPath, "CDebugSettings.json");
            var defaultJson = "{}";
            File.WriteAllText(jsonPath, defaultJson);
            return defaultJson;

        }

        private static CDebugSettings _Instance;
        public static CDebugSettings Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = Load();
                }
                return _Instance;
            }
        }

        public int GetWriteLine(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Log:
                    return this.LogWriteLine;
                case LogLevel.Warning:
                    return this.WarningWriteLine;
                case LogLevel.Error:
                    return this.ErrorWriteLine;
                case LogLevel.Exception:
                    return this.ExceptionWriteLine;
            }
            return -1;
        }
    }
}