using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole.Runtime
{
    public class LogReader
    {
        public int instanceId;
        public int[] subSystem;
        public LogLevel level;
        public long timeSpan;
        public const string MSG = "#Msg";
        public long msgIndex;
        public const string END = "#End";

        public int stackTrackStartIndex;
        public string logFileName;

        private const char baseSpliter = '_';
        private const char subSystemSpliter = ',';
        public void Write(StreamWriter writer)
        {
            writer.BaseStream.Position = writer.BaseStream.Length;
            writer.Write(this.instanceId); writer.Write(baseSpliter);

            for (int i = 0; i < this.subSystem.Length; i++)
            {
                var sub = this.subSystem[i];
                writer.Write(sub);
                if (i != this.subSystem.Length - 1)
                {
                    writer.Write(subSystemSpliter);
                }
                else
                {
                    writer.Write(baseSpliter);
                }
            }

            writer.Write(((int)this.level)); writer.Write(baseSpliter);
            writer.Write(this.timeSpan); writer.Write(baseSpliter);
            writer.Write(this.msgIndex); writer.Write(baseSpliter);
            writer.Write(this.stackTrackStartIndex); writer.Write(baseSpliter);
            writer.WriteLine(this.logFileName);
        }

        public bool IsBrokenReader = false;
        public static LogReader Parse(string line)
        {
            try
            {
                var spliter = line.Split(baseSpliter);
                var reader = new LogReader();
                reader.instanceId = int.Parse(spliter[0]);
                {
                    var subSystemStringArray = spliter[1].Split(subSystemSpliter);
                    reader.subSystem = new int[subSystemStringArray.Length];
                    for (int i = 0; i < subSystemStringArray.Length; i++)
                    {
                        reader.subSystem[i] = int.Parse(subSystemStringArray[i]);
                    }
                }
                reader.level = (LogLevel)int.Parse(spliter[2]);
                reader.timeSpan = long.Parse(spliter[3]);
                reader.msgIndex = long.Parse(spliter[4]);
                reader.stackTrackStartIndex = int.Parse(spliter[5]);

                // SubSystem?????????????????????_??????????????????
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
            brokenReader.subSystem = new int[] { CDebugSubSystemEnumConfig.subSystemNullName };
            var nullName = CDebugSubSystemEnumConfig.subSystemNullName;
            brokenReader.logFileName = CDebugSubSystemEnumConfig.GetSubSystemName(nullName);
            return brokenReader;
        }

        public LogIO logIO { private get; set; }

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
            remoteLog.time = this.timeSpan;
            remoteLog.msg = this.msg;
            remoteLog.stackTrackStartIndex = this.stackTrackStartIndex;
            remoteLog.logFileName = this.logFileName;

            return remoteLog;
        }
    }
}