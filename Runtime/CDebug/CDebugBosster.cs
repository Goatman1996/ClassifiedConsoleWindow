using System;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole
{
    internal class CDebugBosster
    {
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void RuntimeOnLoad()
        {
            UnityEngine.Debug.Log("RuntimeOnLoad");
            if (!hasListenUnityLog)
            {
                UnityEngine.Application.logMessageReceived += OnUnityLog;
            }
            hasListenUnityLog = true;
        }
        private static bool hasListenUnityLog = false;

        private static void OnUnityLog(string condition, string stackTrace, UnityEngine.LogType type)
        {
            var logLevel = LogLevel.Log;
            switch (type)
            {
                case UnityEngine.LogType.Log:
                    if (!CDebugSettings.Instance.catchNativeLog)
                    {
                        return;
                    }
                    logLevel = LogLevel.Log;
                    break;
                case UnityEngine.LogType.Warning:
                    if (!CDebugSettings.Instance.catchNativeWarning)
                    {
                        return;
                    }
                    logLevel = LogLevel.Warning;
                    break;
                case UnityEngine.LogType.Error:
                    if (!CDebugSettings.Instance.catchNativeError)
                    {
                        return;
                    }
                    logLevel = LogLevel.Error;
                    break;
                case UnityEngine.LogType.Exception:
                    if (!CDebugSettings.Instance.catchNativeException)
                    {
                        return;
                    }
                    logLevel = LogLevel.Exception;
                    break;
                default:
                    logLevel = LogLevel.Log;
                    break;
            }

            var msg = CDbugExceptionUtil.SplitCDebugException(condition, out bool isClassifiedException, out int subSystem);
            if (isClassifiedException == false)
            {
                var log = LogWriterFactory.CreateLogWriterWithStack(msg, logLevel, stackTrace, (int)UnityNativeSubSystem.Unity_Native_Log);
                ManagedLogFile.WriteLog(log);
            }
        }

        [ClassifiedConsole.CDebugSubSystem]
        public enum UnityNativeSubSystem
        {
            Unity_Native_Log = int.MaxValue,
        }
    }
}