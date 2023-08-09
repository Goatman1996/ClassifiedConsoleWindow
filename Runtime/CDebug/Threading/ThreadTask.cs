using System;

namespace ClassifiedConsole.Runtime
{
    // public class ThreadTask
    // {
    //     public Func<object> Task;
    //     public object result;
    //     public Action<ThreadTask> callBack;
    // }

    public interface IThreadTask
    {
        public void Run();
        public void CallBack();
    }

    public struct ThreadTask_WriterAppendLog : IThreadTask
    {
        public LogIO logIO;
        public LogWriter logWriter;

        public void Run()
        {
            this.result = this.logIO.WriteLog(logWriter);
        }

        public LogFile logFile;

        public void CallBack()
        {
            if (this.result == null)
            {
                return;
            }
            this.logFile.OnThreadWriteTaskBack(result);
        }

        private LogReader result;
    }

    public struct ThreadTask_OnThreadWriteTaskBack : IThreadTask
    {
        public LogFileIndexer indexer;
        public LogReader logReader;

        public void Run()
        {
            this.indexer?.AppendReader(logReader);
        }

        public LogFile logFile;

        public void CallBack()
        {
            this.logFile.CallOnAppendLog();
        }
    }

    public struct ThreadTask_LogInternal : IThreadTask
    {
        public LogWriter logWriter;
        public LogLevel logLevel;
        public int instanceId;
        public int subSystem;

        public void Run()
        {
            this.result = LogWriterFactory.CreateLogWriter(logWriter, logLevel, instanceId, subSystem);
        }

        private LogWriter result;

        public void CallBack()
        {
            ManagedLogFile.WriteLog(this.result);
        }
    }
}
