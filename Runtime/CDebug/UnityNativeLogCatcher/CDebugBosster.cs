using System;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole
{
    internal class CDebugBosster
    {
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void RuntimeOnLoad()
        {
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
                    return;
            }

            var isException = logLevel == LogLevel.Exception;
            var IsClassifiedException = false;
            if (isException)
            {
                IsClassifiedException = CDebugAttributeHelper.IsClassifiedException(condition, out int subSystem, out string realMsg);
                if (IsClassifiedException)
                {
                    CDebug.LogInternalWithStack(realMsg, stackTrace, logLevel, null, subSystem);
                    return;
                }
            }

            var subSystemNative = (int)UnityNativeSubSystem.Unity_Native_Log;
            CDebug.LogInternalWithStack(condition, stackTrace, logLevel, null, subSystemNative);
        }

        [ClassifiedConsole.CDebugSubSystem]
        public enum UnityNativeSubSystem
        {
            Unity_Native_Log = int.MaxValue,
        }
    }
}
