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
        private static void LogInternal(int skipLine, string msg, LogLevel logLevel, string uid, UnityEngine.Object context, params int[] subSystem)
        {
            int instanceId = 0;
            if (context != null)
            {
                instanceId = context.GetInstanceID();
            }

            var logWriter = new LogWriter();
            var writeLine = CDebugSettings.Instance.GetWriteLine(logLevel);
            logWriter.msg = LogWriterFactory.BuildLogMsg(msg, writeLine, skipLine, out int stackTrackStartIndex).ToString();
            logWriter.stackTrackStartIndex = msg.Length;

            var writeTask = new ThreadTask();
            writeTask.Task = () => LogWriterFactory.CreateLogWriter(logWriter, logLevel, uid, instanceId, subSystem);
            writeTask.callBack = (result) =>
            {
                ManagedLogFile.WriteLog((LogWriter)result.result);
            };

            ManagedLogFile.threadRunner.AddTaskToQueue(writeTask);
        }

        #region Log
        public static void Log(string msg, params int[] subSystem)
        {
            var skipLine = CDebugSettings.Instance.stackSkipLine;
            LogInternal(skipLine, msg, LogLevel.Log, null, null, subSystem);
        }

        public static void Log(string msg, UnityEngine.Object context, params int[] subSystem)
        {
            var skipLine = CDebugSettings.Instance.stackSkipLine;
            LogInternal(skipLine, msg, LogLevel.Log, null, context, subSystem);
        }

        public static void Log(int skipLine, string msg, params int[] subSystem)
        {
            LogInternal(skipLine, msg, LogLevel.Log, null, null, subSystem);
        }

        public static void Log(int skipLine, string msg, UnityEngine.Object context, params int[] subSystem)
        {
            LogInternal(skipLine, msg, LogLevel.Log, null, context, subSystem);
        }
        #endregion

        #region LogWarning
        public static void LogWarning(string msg, params int[] subSystem)
        {
            var skipLine = CDebugSettings.Instance.stackSkipLine;
            LogInternal(skipLine, msg, LogLevel.Warning, null, null, subSystem);
        }

        public static void LogWarning(string msg, UnityEngine.Object context, params int[] subSystem)
        {
            var skipLine = CDebugSettings.Instance.stackSkipLine;
            LogInternal(skipLine, msg, LogLevel.Warning, null, context, subSystem);
        }

        public static void LogWarning(int skipLine, string msg, params int[] subSystem)
        {
            LogInternal(skipLine, msg, LogLevel.Warning, null, null, subSystem);
        }

        public static void LogWarning(int skipLine, string msg, UnityEngine.Object context, params int[] subSystem)
        {
            LogInternal(skipLine, msg, LogLevel.Warning, null, context, subSystem);
        }
        #endregion

        #region LogError
        public static void LogError(string msg, params int[] subSystem)
        {
            var skipLine = CDebugSettings.Instance.stackSkipLine;
            LogInternal(skipLine, msg, LogLevel.Error, null, null, subSystem);
        }

        public static void LogError(string msg, UnityEngine.Object context, params int[] subSystem)
        {
            var skipLine = CDebugSettings.Instance.stackSkipLine;
            LogInternal(skipLine, msg, LogLevel.Error, null, context, subSystem);
        }

        public static void LogError(int skipLine, string msg, params int[] subSystem)
        {
            LogInternal(skipLine, msg, LogLevel.Error, null, null, subSystem);
        }

        public static void LogError(int skipLine, string msg, UnityEngine.Object context, params int[] subSystem)
        {
            LogInternal(skipLine, msg, LogLevel.Error, null, context, subSystem);
        }
        #endregion

        #region LogException
        public static void LogException(string msg, params int[] subSystem)
        {
            var skipLine = CDebugSettings.Instance.stackSkipLine;
            LogInternal(skipLine, msg, LogLevel.Exception, null, null, subSystem);

            if (subSystem.Length == 0)
            {
                UnityEngine.Debug.LogException(new ClassifiedException(CDebugSubSystemEnumConfig.subSystemNullName, msg), null);
            }
            else
            {
                UnityEngine.Debug.LogException(new ClassifiedException(subSystem[0], msg), null);
            }
        }

        public static void LogException(string msg, UnityEngine.Object context, params int[] subSystem)
        {
            var skipLine = CDebugSettings.Instance.stackSkipLine;
            LogInternal(skipLine, msg, LogLevel.Exception, null, context, subSystem);

            if (subSystem.Length == 0)
            {
                UnityEngine.Debug.LogException(new ClassifiedException(CDebugSubSystemEnumConfig.subSystemNullName, msg), context);
            }
            else
            {
                UnityEngine.Debug.LogException(new ClassifiedException(subSystem[0], msg), context);
            }
        }

        public static void LogException(int skipLine, string msg, params int[] subSystem)
        {
            LogInternal(skipLine, msg, LogLevel.Exception, null, null, subSystem);

            if (subSystem.Length == 0)
            {
                UnityEngine.Debug.LogException(new ClassifiedException(CDebugSubSystemEnumConfig.subSystemNullName, msg), null);
            }
            else
            {
                UnityEngine.Debug.LogException(new ClassifiedException(subSystem[0], msg), null);
            }
        }

        public static void LogException(int skipLine, string msg, UnityEngine.Object context, params int[] subSystem)
        {
            LogInternal(skipLine, msg, LogLevel.Exception, null, context, subSystem);

            if (subSystem.Length == 0)
            {
                UnityEngine.Debug.LogException(new ClassifiedException(CDebugSubSystemEnumConfig.subSystemNullName, msg), context);
            }
            else
            {
                UnityEngine.Debug.LogException(new ClassifiedException(subSystem[0], msg), context);
            }
        }
        #endregion
    }
}