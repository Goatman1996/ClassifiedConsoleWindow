using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole.Editor
{
    internal class EditorLogReader
    {
        public readonly LogReader internalLogReader;
        public readonly LogIO logIO;

        public EditorLogReader(LogReader reader, LogIO io)
        {
            internalLogReader = reader;
            logIO = io;
        }

        public int instanceId { get => this.internalLogReader.instanceId; }
        public int subSystem { get => this.internalLogReader.subSystem; }
        public LogLevel level { get => this.internalLogReader.level; }
        public long timeSpan { get => this.internalLogReader.timeSpan; }
        public long msgIndex { get => this.internalLogReader.msgIndex; }
        public int msgLength { get => this.internalLogReader.msgLength; }
        public const string END = "#End";

        public int stackTrackStartIndex { get => this.internalLogReader.stackTrackStartIndex; }
        public string logFileName { get => this.internalLogReader.logFileName; }

        public bool IsBrokenReader { get => this.internalLogReader.IsBrokenReader; }

        private DateTime? _time;
        public DateTime time
        {
            get
            {
                if (this.IsBrokenReader) return default;
                if (this._time == null)
                {
                    var startTime = new DateTime(1970, 1, 1, 0, 0, 0);
                    this._time = startTime.AddSeconds(this.timeSpan);
                }
                return this._time.Value;
            }
        }

        private string _msg;
        public string msg
        {
            get
            {
                if (_msg == null)
                {
                    if (this.IsBrokenReader) _msg = "Broken";
                    _msg = this.logIO.ReadLog(this.msgIndex, this.msgLength);
                    // _msg = this.logIO.ReadLog(this.msgIndex);
                }
                return _msg;
            }
        }

        public string GetMsg(int lineCount)
        {
            if (this.IsBrokenReader) return "Broken";
            var content = this.logIO.ReadLines(this.msgIndex, lineCount);
            return content;
        }

        private string _md5;
        public string md5
        {
            get
            {
                if (this.IsBrokenReader) return "default";
                if (this._md5 == null)
                {
                    var md5ource = this.msg;
                    // MD5 md = MD5.Create();
                    var pwdBytes = Encoding.UTF8.GetBytes(md5ource);
                    var md5Bytes = md5Builder.ComputeHash(pwdBytes);
                    var md5String = System.BitConverter.ToString(md5Bytes);
                    this._md5 = this.level + md5String;
                }
                return this._md5;
            }
        }

        private static MD5 _md5Builder = null;
        private static MD5 md5Builder
        {
            get
            {
                if (_md5Builder == null)
                {
                    _md5Builder = MD5.Create();
                }
                return _md5Builder;
            }
        }

        public int needShowSubSystemVersion = -1;
        private bool _NeedShowSubSystem;
        public bool NeedShowSubSystem
        {
            get
            {
                if (needShowSubSystemVersion != CDebugSubSystemEnumConfig.SubSystemSettingsVersion)
                {
                    _NeedShowSubSystem = CDebugSubSystemEnumConfig.IsSubSystemOn(this.subSystem);
                    needShowSubSystemVersion = CDebugSubSystemEnumConfig.SubSystemSettingsVersion;
                }
                return _NeedShowSubSystem;
            }
        }

        private static int needShowLogLevelVersion = -1;
        private static bool _NeedShowLogLevel_Log;
        private static bool _NeedShowLogLevel_Warning;
        private static bool _NeedShowLogLevel_Error;
        private static bool _NeedShowLogLevel_Exception;
        public static bool NeedShowLogLevel(LogLevel level)
        {
            if (needShowLogLevelVersion != CDebugWindowConfig.needShowLogLevelVersion)
            {
                _NeedShowLogLevel_Log = CDebugWindowConfig.NeedShowLogLevel(LogLevel.Log);
                _NeedShowLogLevel_Warning = CDebugWindowConfig.NeedShowLogLevel(LogLevel.Warning);
                _NeedShowLogLevel_Error = CDebugWindowConfig.NeedShowLogLevel(LogLevel.Error);
                _NeedShowLogLevel_Exception = CDebugWindowConfig.NeedShowLogLevel(LogLevel.Exception);

                needShowLogLevelVersion = CDebugWindowConfig.needShowLogLevelVersion;
            }
            if (level == LogLevel.Log)
            {
                return _NeedShowLogLevel_Log;
            }
            else if (level == LogLevel.Warning)
            {
                return _NeedShowLogLevel_Warning;
            }
            else if (level == LogLevel.Error)
            {
                return _NeedShowLogLevel_Error;
            }
            else
            {
                return _NeedShowLogLevel_Exception;
            }
        }
    }
}