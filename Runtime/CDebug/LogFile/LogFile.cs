using System;
using System.Collections.Generic;
using System.IO;

namespace ClassifiedConsole.Runtime
{
    public class LogFile
    {
        public int logFileId { get; private set; }
        private string logDir;
        public LogFile(string logDir, int id)
        {
            this.logFileId = id;
            this.logDir = logDir;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            this.indexer = new LogFileIndexer(this.logDir + "/_Indexer_");

            this.logIoDic = new Dictionary<string, LogIO>();
        }

        private LogFileIndexer indexer;

        public int logCount
        {
            get
            {
                return this.indexer.logCount;
            }
        }

        public LogReader this[int index]
        {
            get
            {
                var reader = this.indexer[index];
                var logIoName = reader.logFileName;
                var logIo = this.GetLogIo(logIoName);
                reader.logIO = logIo;
                return reader;
            }
        }

        public event Action OnAppendLog;
        public void AppendLog(LogWriter log)
        {
            var logIo = this.GetLogIo(log.logFileName);
            var logReader = logIo.WriteLog(log);
            this.indexer.AppendReader(logReader);
            this.OnAppendLog?.Invoke();
        }

        private Dictionary<string, LogIO> logIoDic;

        public LogIO GetLogIo(string logIoName)
        {
            if (!this.logIoDic.ContainsKey(logIoName))
            {
                this.logIoDic.Add(logIoName, new LogIO(this.logDir + $"/{logIoName}"));
            }
            return this.logIoDic[logIoName];
        }
    }
}