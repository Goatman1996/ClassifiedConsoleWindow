using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole
{
    public static class CDebug
    {
        internal static void LogInternalWithStack(string msg, string stack, LogLevel logLevel, UnityEngine.Object context, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            int instanceId = 0;
            if (context != null)
            {
                instanceId = context.GetInstanceID();
            }

            var logWriter = new LogWriter();
            var writeLine = CDebugSettings.Instance.GetWriteLine(logLevel);
            logWriter.msgSb = LogWriterFactory.BuildLogMsgWithIn(msg, stack, writeLine, out int stackTrackStartIndex);
            logWriter.stackTrackStartIndex = msg.Length;

            // GC
            // var writeTask = new ThreadTask();
            // writeTask.Task = () => LogWriterFactory.CreateLogWriter(logWriter, logLevel, instanceId, subSystem);
            // writeTask.callBack = (result) =>
            // {
            //     ManagedLogFile.WriteLog((LogWriter)result.result);
            // };

            // 解 GC
            // var task = new ThreadTask_LogInternal
            // {
            //     logWriter = logWriter,
            //     logLevel = logLevel,
            //     instanceId = instanceId,
            //     subSystem = subSystem
            // };

            // ManagedLogFile.threadRunner.AddTaskToQueue(task);

            // 单线程
            var result = LogWriterFactory.CreateLogWriter(logWriter, logLevel, instanceId, subSystem);
            ManagedLogFile.WriteLog(result);
        }



        internal static void LogInternal(int skipLine, string msg, LogLevel logLevel, UnityEngine.Object context, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            int instanceId = 0;
            if (context != null)
            {
                instanceId = context.GetInstanceID();
            }

            var logWriter = new LogWriter();
            var writeLine = CDebugSettings.Instance.GetWriteLine(logLevel);
            if (writeLine == 0)
            {
                logWriter.msg = msg;
            }
            else
            {
                logWriter.msgSb = LogWriterFactory.BuildLogMsg(msg, writeLine, skipLine, out int stackTrackStartIndex);
            }
            logWriter.stackTrackStartIndex = msg.Length;

            // GC
            // var writeTask = new ThreadTask();
            // writeTask.Task = () => LogWriterFactory.CreateLogWriter(logWriter, logLevel, instanceId, subSystem);
            // writeTask.callBack = (result) =>
            // {
            //     ManagedLogFile.WriteLog((LogWriter)result.result);
            // };

            // 解 GC
            // var task = new ThreadTask_LogInternal
            // {
            //     logWriter = logWriter,
            //     logLevel = logLevel,
            //     instanceId = instanceId,
            //     subSystem = subSystem
            // };

            // ManagedLogFile.threadRunner.AddTaskToQueue(task);

            // 单线程
            var result = LogWriterFactory.CreateLogWriter(logWriter, logLevel, instanceId, subSystem);
            ManagedLogFile.WriteLog(result);
        }

        #region Log
        public static void Log(string msg, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            var skipLine = CDebugSettings.Instance.stackSkipLine;
            LogInternal(skipLine, msg, LogLevel.Log, null, subSystem);
        }

        public static void Log(string msg, UnityEngine.Object context, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            var skipLine = CDebugSettings.Instance.stackSkipLine;
            LogInternal(skipLine, msg, LogLevel.Log, context, subSystem);
        }

        public static void Log(int skipLine, string msg, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            LogInternal(skipLine, msg, LogLevel.Log, null, subSystem);
        }

        public static void Log(int skipLine, string msg, UnityEngine.Object context, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            LogInternal(skipLine, msg, LogLevel.Log, context, subSystem);
        }
        #endregion

        #region LogWarning
        public static void LogWarning(string msg, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            var skipLine = CDebugSettings.Instance.stackSkipLine;
            LogInternal(skipLine, msg, LogLevel.Warning, null, subSystem);
        }

        public static void LogWarning(string msg, UnityEngine.Object context, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            var skipLine = CDebugSettings.Instance.stackSkipLine;
            LogInternal(skipLine, msg, LogLevel.Warning, context, subSystem);
        }

        public static void LogWarning(int skipLine, string msg, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            LogInternal(skipLine, msg, LogLevel.Warning, null, subSystem);
        }

        public static void LogWarning(int skipLine, string msg, UnityEngine.Object context, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            LogInternal(skipLine, msg, LogLevel.Warning, context, subSystem);
        }
        #endregion

        #region LogError
        public static void LogError(string msg, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            var skipLine = CDebugSettings.Instance.stackSkipLine;
            LogInternal(skipLine, msg, LogLevel.Error, null, subSystem);
        }

        public static void LogError(string msg, UnityEngine.Object context, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            var skipLine = CDebugSettings.Instance.stackSkipLine;
            LogInternal(skipLine, msg, LogLevel.Error, context, subSystem);
        }

        public static void LogError(int skipLine, string msg, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            LogInternal(skipLine, msg, LogLevel.Error, null, subSystem);
        }

        public static void LogError(int skipLine, string msg, UnityEngine.Object context, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            LogInternal(skipLine, msg, LogLevel.Error, context, subSystem);
        }
        #endregion

        #region LogException
        public static void LogException(string msg, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            if (UnityEngine.Application.isEditor && !UnityEngine.Application.isPlaying)
            {
                var skipLine = CDebugSettings.Instance.stackSkipLine;
                LogInternal(skipLine, msg, LogLevel.Exception, null, subSystem);
            }
            else if (UnityEngine.Application.isPlaying && !CDebugSettings.Instance.catchNativeException)
            {
                var skipLine = CDebugSettings.Instance.stackSkipLine;
                LogInternal(skipLine, msg, LogLevel.Exception, null, subSystem);
            }
            else
            {
                UnityEngine.Debug.LogException(new ClassifiedException(subSystem, msg), null);
            }
        }

        public static void LogException(string msg, UnityEngine.Object context, int subSystem = (int)UnDefinedSubSystem.Not_Classified)
        {
            if (UnityEngine.Application.isEditor && !UnityEngine.Application.isPlaying)
            {
                var skipLine = CDebugSettings.Instance.stackSkipLine;
                LogInternal(skipLine, msg, LogLevel.Exception, context, subSystem);
            }
            else if (UnityEngine.Application.isPlaying && !CDebugSettings.Instance.catchNativeException)
            {
                var skipLine = CDebugSettings.Instance.stackSkipLine;
                LogInternal(skipLine, msg, LogLevel.Exception, context, subSystem);
            }
            else
            {
                UnityEngine.Debug.LogException(new ClassifiedException(subSystem, msg), context);
            }
        }
        #endregion
    }
}