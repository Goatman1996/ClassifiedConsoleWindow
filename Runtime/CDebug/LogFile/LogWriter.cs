using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedConsole.Runtime
{
    [Serializable]
    public class LogWriter
    {
        public string uid;
        public int instanceId;
        public int[] logSubSystem;
        public LogLevel level;
        public long time;
        public string msg;

        public int stackTrackStartIndex;
        public string logFileName;

        public LogReader Write(StreamWriter writer)
        {
            var logIndexer = new LogReader();
            logIndexer.logFileName = this.logFileName;
            logIndexer.stackTrackStartIndex = this.stackTrackStartIndex;

            writer.BaseStream.Position = writer.BaseStream.Length;

            // UID
            logIndexer.uidIndex = this.WriteGetLastIndex(writer, LogReader.UID);
            writer.WriteLine(this.uid);

            // InstanceId
            logIndexer.instanceIdIndex = this.WriteGetLastIndex(writer, LogReader.INSTANCEID);
            writer.WriteLine(this.instanceId);

            // SUBSYSTEM
            logIndexer.subSystemIndex = this.WriteGetLastIndex(writer, LogReader.SUBSYSTEM);
            foreach (var system in this.logSubSystem)
            {
                writer.WriteLine(system);
            }

            // LEVEL
            logIndexer.levelIndex = this.WriteGetLastIndex(writer, LogReader.LEVEL);
            writer.WriteLine(((int)this.level).ToString());

            // TIME
            logIndexer.timeIndex = this.WriteGetLastIndex(writer, LogReader.TIME);
            writer.WriteLine(this.time.ToString());

            // MSG
            logIndexer.msgIndex = this.WriteGetLastIndex(writer, LogReader.MSG);
            writer.WriteLine(this.msg);

            // END
            writer.WriteLine(LogReader.END);
            writer.WriteLine("");

            writer.Flush();

            return logIndexer;
        }

        private long WriteGetLastIndex(StreamWriter writer, string content)
        {
            writer.WriteLine(content);
            writer.Flush();
            return writer.BaseStream.Position;
        }
    }
}