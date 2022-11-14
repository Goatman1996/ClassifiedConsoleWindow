using UnityEngine;
using UnityEditor;
using System.IO;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole.Editor
{
    internal static class ConsoleIconUtility
    {
        private static Texture _LogIcon;
        public static Texture LogIcon
        {
            get
            {
                if (_LogIcon == null)
                {
                    _LogIcon = Resources.Load<Texture>("CDebug_LogIcon");
                }
                return _LogIcon;
            }
        }

        private static Texture _WarningIcon;
        public static Texture WarningIcon
        {
            get
            {
                if (_WarningIcon == null)
                {
                    _WarningIcon = Resources.Load<Texture>("CDebug_WarningIcon");
                }
                return _WarningIcon;
            }
        }

        private static Texture _ErrorIcon;
        public static Texture ErrorIcon
        {
            get
            {
                if (_ErrorIcon == null)
                {
                    _ErrorIcon = Resources.Load<Texture>("CDebug_ErrorIcon");
                }
                return _ErrorIcon;
            }
        }

        private static Texture _ExceptionIcon;
        public static Texture ExceptionIcon
        {
            get
            {
                if (_ExceptionIcon == null)
                {
                    _ExceptionIcon = Resources.Load<Texture>("CDebug_ExceptionIcon");
                }
                return _ExceptionIcon;
            }
        }

        public static Texture GetIcon(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Log)
            {
                return LogIcon;
            }
            switch (logLevel)
            {
                case LogLevel.Log:
                    return LogIcon;
                case LogLevel.Warning:
                    return WarningIcon;
                case LogLevel.Error:
                    return ErrorIcon;
                case LogLevel.Exception:
                    return ExceptionIcon;
                default:
                    return null;
            }
        }
    }
}