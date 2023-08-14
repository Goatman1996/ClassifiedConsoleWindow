using UnityEngine;

namespace ClassifiedConsole
{
    public partial class CDebugSettings : ScriptableObject
    {
        [Header("是否按分类系统,分开存放Log文件")]
        public bool SplitLogFile = true;

        [Header("要记录的栈信息(不算Msg)-1 不限制行数")]
        [SerializeField]
        private int _LogWriteLine = 1;
        public int LogWriteLine
        {
            get => this._LogWriteLine;
            set
            {
                var newValue = Mathf.Clamp(value, -1, int.MaxValue);
                this._LogWriteLine = newValue;
            }
        }

        [Header("要记录的栈信息(不算Msg)-1 不限制行数")]
        [SerializeField]
        private int _WarningWriteLine = 1;
        public int WarningWriteLine
        {
            get => this._WarningWriteLine;
            set
            {
                var newValue = Mathf.Clamp(value, -1, int.MaxValue);
                this._WarningWriteLine = newValue;
            }
        }

        [Header("要记录的栈信息(不算Msg)-1 不限制行数")]
        [SerializeField] private int _ErrorWriteLine = -1;
        public int ErrorWriteLine
        {
            get => this._ErrorWriteLine;
            set
            {
                var newValue = Mathf.Clamp(value, -1, int.MaxValue);
                this._ErrorWriteLine = newValue;
            }
        }

        [Header("要记录的栈信息(不算Msg)-1 不限制行数")]
        [SerializeField]
        private int _ExceptionWriteLine = -1;
        public int ExceptionWriteLine
        {
            get => this._ExceptionWriteLine;
            set
            {
                var newValue = Mathf.Clamp(value, -1, int.MaxValue);
                this._ExceptionWriteLine = newValue;
            }
        }

        [Header("栈信息是否包含文件信息(建议出包前设置成false)")]
        public bool msgWithFileInfo = true;

        [Header("要保留的LogFile数量")]
        [SerializeField]
        private int _keepLogFileCount = 3;
        public int keepLogFileCount
        {
            get => this._keepLogFileCount;
            set
            {
                var newValue = Mathf.Clamp(value, 1, int.MaxValue);
                this._keepLogFileCount = newValue;
            }
        }

        [Header("是否监听运行时Log")]
        public bool catchNativeLog = false;
        [Header("是否监听运行时Warning")]
        public bool catchNativeWarning = false;
        [Header("是否监听运行时Error")]
        public bool catchNativeError = false;
        [Header("是否监听运行时Exception")]
        public bool catchNativeException = false;

        [Header("调试真机时，开放的端口号")]
        [SerializeField]
        private int _port = 34599;
        public int port
        {
            get => this._port;
            set
            {
                var newValue = Mathf.Clamp(value, 0, 65535);
                this._port = newValue;
            }
        }

        [Header("自定义跳过栈行数(默认3且应该>=3)")]
        [SerializeField]
        private int _stackSkipLine = 3;
        public int stackSkipLine
        {
            get => this._stackSkipLine;
            set
            {
                return;
            }
        }

        [Header("所有包含了[CDebugSubSystemAttribute] 设置的Assembly,实用','分割")]
        public string subSystemDefinedAssembly = "Assembly-CSharp";

        [Header("windowFPS")]
        [SerializeField] private int _windowFPS = 30;
        public int windowFPS
        {
            get
            {
                return Mathf.Clamp(this._windowFPS, 5, 60);
            }
        }

        [Header("limitLogCount")]
        [SerializeField] private int _limitLogCount = (100 * 100) * 10;
        public int limitLogCount
        {
            get
            {
                // 1万 ~ 100万
                return Mathf.Clamp(this._limitLogCount, 10000, 1000000);
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