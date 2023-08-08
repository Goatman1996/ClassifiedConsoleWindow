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

        public int TempLogCount
        {
            get => this.indexer.TempLogCount;
        }

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

        public void CallOnAppendLog()
        {
            this.OnAppendLog?.Invoke();
        }

        public event Action OnAppendLog;
        public void AppendLog(LogWriter log)
        {
            var logIo = this.GetLogIo(log.logFileName);

            // var writeTask = new ThreadTask();
            // writeTask.Task = () => logIo.WriteLog(log);
            // writeTask.callBack = this.OnThreadWriteTaskBack;
            var task = new ThreadTask_WriterAppendLog()
            {
                logIO = logIo,
                logWriter = log,
                logFile = this
            };
            ManagedLogFile.threadRunner.AddTaskToQueue(task);
        }

        private void OnThreadWriteTaskBack(ThreadTask result)
        {
            if (result.result == null) return;
            var logReader = result.result as LogReader;

            this.OnThreadWriteTaskBack(logReader);
        }

        public void OnThreadWriteTaskBack(LogReader logReader)
        {
            // var writeTask = new ThreadTask();
            // writeTask.Task = () =>
            // {
            //     this.indexer?.AppendReader(logReader);
            //     return null;
            // };
            // writeTask.callBack = (result) => this.OnAppendLog?.Invoke();

            var task = new ThreadTask_OnThreadWriteTaskBack
            {
                indexer = this.indexer,
                logReader = logReader,
                logFile = this
            };

            ManagedLogFile.threadRunner.AddTaskToQueue(task);
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

        public void ReleaseIO()
        {
            foreach (var io in this.logIoDic.Values)
            {
                io.ReleaseIO();
            }
            this.logIoDic.Clear();

            this.indexer.ReleaseIO();
            this.indexer = null;
        }
    }
}