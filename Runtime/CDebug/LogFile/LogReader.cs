using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole.Runtime
{
    public class LogReader
    {
        public const string INSTANCEID = "#InstanceId";
        public long instanceIdIndex;
        public const string SUBSYSTEM = "#SubSystem";
        public long subSystemIndex;
        public const string LEVEL = "#Level";
        public long levelIndex;
        public const string TIME = "#Time";
        public long timeIndex;
        public const string MSG = "#Msg";
        public long msgIndex;
        public const string END = "#End";

        public int stackTrackStartIndex;
        public string logFileName;

        private const string spliter = "_";
        public void Write(StreamWriter writer)
        {
            writer.BaseStream.Position = writer.BaseStream.Length;
            writer.Write(this.instanceIdIndex); writer.Write(spliter);
            writer.Write(this.subSystemIndex); writer.Write(spliter);
            writer.Write(this.levelIndex); writer.Write(spliter);
            writer.Write(this.timeIndex); writer.Write(spliter);
            writer.Write(this.msgIndex); writer.Write(spliter);
            writer.Write(this.stackTrackStartIndex); writer.Write(spliter);
            writer.WriteLine(this.logFileName);
        }

        public bool IsBrokenReader = false;
        public static LogReader Parse(string line)
        {
            try
            {
                var spliter = line.Split('_');
                var reader = new LogReader();
                reader.instanceIdIndex = long.Parse(spliter[0]);
                reader.subSystemIndex = long.Parse(spliter[1]);
                reader.levelIndex = long.Parse(spliter[2]);
                reader.timeIndex = long.Parse(spliter[3]);
                reader.msgIndex = long.Parse(spliter[4]);
                reader.stackTrackStartIndex = int.Parse(spliter[5]);

                // SubSystem中可能会带有“_”，特殊处理
                reader.logFileName = "";
                for (int s = 6; s < spliter.Length; s++)
                {
                    reader.logFileName += spliter[s];
                    reader.logFileName += "_";
                }
                reader.logFileName = reader.logFileName.Remove(reader.logFileName.Length - 1, 1);
                return reader;
            }
            catch
            {
                return GetBroken();
            }
        }


        public static LogReader GetBroken()
        {
            var brokenReader = new LogReader();
            brokenReader.IsBrokenReader = true;
            brokenReader.stackTrackStartIndex = 0;
            var nullName = CDebugSubSystemEnumConfig.subSystemNullName;
            brokenReader.logFileName = CDebugSubSystemEnumConfig.GetSubSystemName(nullName);
            return brokenReader;
        }

        public LogIO logIO { private get; set; }

        private int? _instanceId;
        public int instanceId
        {
            get
            {
                if (this.IsBrokenReader) return default;
                if (this._instanceId == null)
                {
                    var instanceIdContext = this.logIO.ReadLog(this.instanceIdIndex, SUBSYSTEM);
                    this._instanceId = int.Parse(instanceIdContext);
                }
                return this._instanceId.Value;
            }
        }

        private int[] _subSystem;
        public int[] subSystem
        {
            get
            {
                if (this.IsBrokenReader && this._subSystem == null)
                {
                    this._subSystem = new int[] { CDebugSubSystemEnumConfig.subSystemNullName };
                }
                if (this._subSystem == null)
                {
                    var content = this.logIO.ReadLog(this.subSystemIndex, LEVEL);
                    var spliter = content.Split('\n');
                    this._subSystem = new int[spliter.Length];
                    for (int i = 0; i < spliter.Length; i++)
                    {
                        var line = spliter[i];
                        this._subSystem[i] = int.Parse(line);
                    }
                }
                return this._subSystem;
            }
        }

        private int _level = -1;
        public LogLevel level
        {
            get
            {
                if (this.IsBrokenReader) return LogLevel.Error;
                if (this._level == -1)
                {
                    var content = this.logIO.ReadLog(this.levelIndex, TIME);
                    var contentInt = int.Parse(content);
                    this._level = contentInt;
                }
                return (LogLevel)this._level;
            }
        }

        private long? _timeTick;
        private long timeTick
        {
            get
            {
                if (this.IsBrokenReader) return default;
                if (this._timeTick == null)
                {
                    var content = this.logIO.ReadLog(this.timeIndex, MSG);
                    this._timeTick = long.Parse(content);
                }
                return this._timeTick.Value;
            }
        }

        private DateTime? _time;
        public DateTime time
        {
            get
            {
                if (this.IsBrokenReader) return default;
                if (this._time == null)
                {
                    var startTime = new DateTime(1970, 1, 1, 0, 0, 0);
                    this._time = startTime.AddSeconds(this.timeTick);
                }
                return this._time.Value;
            }
        }

        public string msg
        {
            get
            {
                if (this.IsBrokenReader) return "Broken";
                var content = this.logIO.ReadLog(this.msgIndex, END);
                return content;
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
                    MD5 md = MD5.Create();
                    var pwdBytes = Encoding.UTF8.GetBytes(md5ource);
                    var md5Bytes = md.ComputeHash(pwdBytes);
                    var md5String = System.BitConverter.ToString(md5Bytes);
                    this._md5 = this.level + md5String;
                }
                return this._md5;
            }
        }

        private int needShowLogLevelVersion = -1;
        private bool _NeedShowLogLevel;
        public bool NeedShowLogLevel
        {
            get
            {
                if (needShowLogLevelVersion != CDebugConfig.needShowLogLevelVersion)
                {
                    _NeedShowLogLevel = CDebugConfig.NeedShowLogLevel(this.level);
                    needShowLogLevelVersion = CDebugConfig.needShowLogLevelVersion;
                }
                return _NeedShowLogLevel;
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

        public LogWriter ToRemoteLog()
        {
            var remoteLog = new LogWriter();

            remoteLog.instanceId = this.instanceId;
            remoteLog.logSubSystem = this.subSystem;
            remoteLog.level = this.level;
            remoteLog.time = this.timeTick;
            remoteLog.msg = this.msg;
            remoteLog.stackTrackStartIndex = this.stackTrackStartIndex;
            remoteLog.logFileName = this.logFileName;

            return remoteLog;
        }
    }
}