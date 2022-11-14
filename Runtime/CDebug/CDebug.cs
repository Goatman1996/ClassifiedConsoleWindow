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
        private static void LogInternal(string msg, LogLevel logLevel, string uid, UnityEngine.Object context, params int[] subSystem)
        {
            int instanceId = 0;
            if (context != null)
            {
                instanceId = context.GetInstanceID();
            }
            var log = LogWriterFactory.CreateLogWriter(msg, logLevel, uid, instanceId, subSystem);
            ManagedLogFile.WriteLog(log);
        }

        public static void Log(string msg, params int[] subSystem)
        {
            LogInternal(msg, LogLevel.Log, null, null, subSystem);
        }

        public static void Log(string msg, UnityEngine.Object context, params int[] subSystem)
        {
            LogInternal(msg, LogLevel.Log, null, context, subSystem);
        }

        public static void LogWarning(string msg, params int[] subSystem)
        {
            LogInternal(msg, LogLevel.Warning, null, null, subSystem);
        }

        public static void LogWarning(string msg, UnityEngine.Object context, params int[] subSystem)
        {
            LogInternal(msg, LogLevel.Warning, null, context, subSystem);
        }

        public static void LogError(string msg, params int[] subSystem)
        {
            LogInternal(msg, LogLevel.Error, null, null, subSystem);
        }

        public static void LogError(string msg, UnityEngine.Object context, params int[] subSystem)
        {
            LogInternal(msg, LogLevel.Error, null, context, subSystem);
        }

        public static void LogException(string msg, params int[] subSystem)
        {
            LogInternal(msg, LogLevel.Exception, null, null, subSystem);

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
            LogInternal(msg, LogLevel.Exception, null, context, subSystem);

            if (subSystem.Length == 0)
            {
                UnityEngine.Debug.LogException(new ClassifiedException(CDebugSubSystemEnumConfig.subSystemNullName, msg), context);
            }
            else
            {
                UnityEngine.Debug.LogException(new ClassifiedException(subSystem[0], msg), context);
            }
        }

        // public static void Log(string msg, UnityEngine.Object context, params int[] subSystem)
        // {
        //     var instanceId = context.GetInstanceID();
        //     var instance = UnityEditor.EditorUtility.InstanceIDToObject(instanceId);
        //     UnityEditor.EditorGUIUtility.PingObject(instance);
        // }
    }
}