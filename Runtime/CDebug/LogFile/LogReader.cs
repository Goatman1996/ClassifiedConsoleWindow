using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole.Runtime
{
    public struct LogReader
    {
        public int instanceId;
        public int subSystem;
        public LogLevel level;
        public long timeSpan;
        public const string MSG = "#Msg";
        public long msgIndex;
        public int msgLength;
        public const string END = "#End";

        public int stackTrackStartIndex;
        public string logFileName;

        private const char baseSpliter = '_';
        public void Write(StreamWriter writer)
        {
            writer.BaseStream.Position = writer.BaseStream.Length;
            writer.Write(this.instanceId); writer.Write(baseSpliter);

            // writer.Write(this.subSystem);
            // 解GC
            var subNumberString = new NumberString(this.subSystem);
            for (int j = subNumberString.Length - 1; j >= 0; j--)
            {
                writer.Write(subNumberString[j]);
            }

            writer.Write(baseSpliter);

            // writer.Write(((int)this.level)); 
            // 解GC
            {
                var numberString = new NumberString((int)this.level);
                for (int j = numberString.Length - 1; j >= 0; j--)
                {
                    writer.Write(numberString[j]);
                }
            }

            writer.Write(baseSpliter);
            // writer.Write(this.timeSpan); 
            // 解GC
            {
                var numberString = new NumberString(this.timeSpan);
                for (int j = numberString.Length - 1; j >= 0; j--)
                {
                    writer.Write(numberString[j]);
                }
            }

            writer.Write(baseSpliter);
            // writer.Write(this.msgIndex); 
            // 解GC
            {
                var numberString = new NumberString(this.msgIndex);
                for (int j = numberString.Length - 1; j >= 0; j--)
                {
                    writer.Write(numberString[j]);
                }
            }

            writer.Write(baseSpliter);

            // writer.Write(this.stackTrackStartIndex);
            // 解GC
            {
                var numberString = new NumberString(this.stackTrackStartIndex);
                for (int j = numberString.Length - 1; j >= 0; j--)
                {
                    writer.Write(numberString[j]);
                }
            }

            writer.Write(baseSpliter);

            // writer.Write(this.msgLength);
            // 解GC
            {
                var numberString = new NumberString(this.msgLength);
                for (int j = numberString.Length - 1; j >= 0; j--)
                {
                    writer.Write(numberString[j]);
                }
            }

            writer.Write(baseSpliter);

            writer.Write(this.logFileName);
            writer.WriteLine();
        }

        public bool IsBrokenReader;
        public static LogReader Parse(string line)
        {
            try
            {
                var spliter = line.Split(baseSpliter);
                var reader = new LogReader();
                reader.instanceId = int.Parse(spliter[0]);
                reader.subSystem = int.Parse(spliter[1]);
                reader.level = (LogLevel)int.Parse(spliter[2]);
                reader.timeSpan = long.Parse(spliter[3]);
                reader.msgIndex = long.Parse(spliter[4]);
                reader.stackTrackStartIndex = int.Parse(spliter[5]);
                reader.msgLength = int.Parse(spliter[6]);

                // SubSystem中可能会带有“_”，特殊处理
                reader.logFileName = "";
                for (int s = 7; s < spliter.Length; s++)
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
            brokenReader.subSystem = CDebugSubSystemEnumConfig.subSystemNullName;
            var nullName = CDebugSubSystemEnumConfig.subSystemNullName;
            brokenReader.logFileName = CDebugSubSystemEnumConfig.GetSubSystemName(nullName);
            return brokenReader;
        }

        public LogIO logIO { private get; set; }

        public string msg
        {
            get
            {
                if (this.IsBrokenReader) return "Broken";
                var content = this.logIO.ReadLog(this.msgIndex, this.msgLength);
                return content;
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