using System;
using System.IO;
using System.Text;

namespace ClassifiedConsole.Runtime
{
    [Serializable]
    public struct LogWriter
    {
        public int instanceId;
        public int logSubSystem;
        public LogLevel level;
        public long time;
        public string msg;
        public StringBuilder msgSb;

        public int stackTrackStartIndex;
        public string logFileName;

        public LogReader? Write(StreamWriter writer)
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

            // writer.WriteLine(this.msg);
            // writer.WriteLine(LogReader.END);
            if (this.msgSb != null)
            {
                var buffer = GetBuffer(this.msgSb.Capacity);
                this.msgSb.CopyTo(0, buffer, 0, this.msgSb.Length);
                writer.Write(buffer);
            }
            else
            {
                writer.Write(this.msg);
            }
            writer.WriteLine();
            if (this.msgSb != null)
            {
                logReader.msgLength = this.msgSb.Length;
            }
            else
            {
                logReader.msgLength = this.msg.Length;
            }

            writer.Write(LogReader.END);
            writer.WriteLine();

            writer.Flush();

            return logReader;
        }

        private static int _bufferCapacity = 1024;
        private static char[] _buffer = new char[_bufferCapacity];

        private static char[] GetBuffer(int capacity)
        {
            if (capacity > _bufferCapacity)
            {
                _bufferCapacity = capacity;
                _buffer = new char[_bufferCapacity];
            }

            return _buffer;
        }
    }
}