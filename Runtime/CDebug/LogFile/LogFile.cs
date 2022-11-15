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

            var writeTask = new ThreadTask();
            writeTask.Task = () => logIo.WriteLog(log);
            writeTask.callBack = this.OnThreadWriteTaskBack;
            ManagedLogFile.threadRunner.AddTaskToQueue(writeTask);
        }

        private void OnThreadWriteTaskBack(ThreadTask result)
        {
            var logReader = result.result as LogReader;

            var writeTask = new ThreadTask();
            writeTask.Task = () =>
            {
                this.indexer.AppendReader(logReader);
                return null;
            };
            writeTask.callBack = (result) => this.OnAppendLog?.Invoke();

            ManagedLogFile.threadRunner.AddTaskToQueue(writeTask);
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