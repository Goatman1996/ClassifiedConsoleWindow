using System;
using System.IO;

namespace ClassifiedConsole.Runtime
{
    [Serializable]
    public struct LogWriter
    {
        public int instanceId;
        public int[] logSubSystem;
        public LogLevel level;
        public long time;
        public string msg;

        public int stackTrackStartIndex;
        public string logFileName;

        public LogReader Write(StreamWriter writer)
        {
            if (writer == null || writer.BaseStream == null)
            {
                return null;
            }
            var logReader = new LogReader();
            logReader.logFileName = this.logFileName;
            logReader.stackTrackStartIndex = this.stackTrackStartIndex;

            writer.BaseStream.Position = writer.BaseStream.Length;

            // InstanceId
            logReader.instanceId = this.instanceId;
            // SUBSYSTEM
            logReader.subSystem = this.logSubSystem;
            // LEVEL
            logReader.level = this.level;
            // TIME
            logReader.timeSpan = this.time;
            // MSG
            logReader.msgIndex = writer.BaseStream.Position;
            writer.WriteLine(this.msg);
            writer.WriteLine(LogReader.END);

            writer.Flush();

            return logReader;
        }
    }
}