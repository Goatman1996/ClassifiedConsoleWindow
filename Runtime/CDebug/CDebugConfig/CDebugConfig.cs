using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClassifiedConsole.Runtime
{
    public class CDebugConfig
    {
        public const string version = "0.1.3";

        public static bool ArchiveOnPlay
        {
            get
            {
                var value = PlayerPrefs.GetInt("CDebugConfig.ArchiveOnPlay", 1);
                return value == 1;
            }
            set
            {
                var intValue = value ? 1 : 0;
                PlayerPrefs.SetInt("CDebugConfig.ArchiveOnPlay", intValue);
            }
        }

        public static Action<bool> OnCollapseChanged;
        private static bool? _Collapse;
        public static bool Collapse
        {
            get
            {
                if (_Collapse == null)
                {
                    var value = PlayerPrefs.GetInt("CDebugConfig.Collapse", 0);
                    _Collapse = value == 1;
                }
                return _Collapse.Value;
            }
            set
            {
                if (Collapse == value) return;
                var intValue = value ? 1 : 0;
                PlayerPrefs.SetInt("CDebugConfig.Collapse", intValue);
                OnCollapseChanged?.Invoke(value);
                _Collapse = value;
            }
        }

        public static int needShowLogLevelVersion = 0;
        private static bool? _ShowLog;
        public static bool ShowLog
        {
            get
            {
                if (_ShowLog == null)
                {
                    var value = PlayerPrefs.GetInt("CDebugConfig.ShowLog", 1);
                    _ShowLog = value == 1;
                }
                return _ShowLog.Value;

            }
            set
            {
                var intValue = value ? 1 : 0;
                PlayerPrefs.SetInt("CDebugConfig.ShowLog", intValue);
                _ShowLog = value;
                needShowLogLevelVersion++;
            }
        }

        private static bool? _ShowWarning;
        public static bool ShowWarning
        {
            get
            {
                if (_ShowWarning == null)
                {
                    var value = PlayerPrefs.GetInt("CDebugConfig.ShowWarning", 1);
                    _ShowWarning = value == 1;
                }
                return _ShowWarning.Value;
            }
            set
            {
                var intValue = value ? 1 : 0;
                PlayerPrefs.SetInt("CDebugConfig.ShowWarning", intValue);
                _ShowWarning = value;
                needShowLogLevelVersion++;
            }
        }

        private static bool? _ShowError;
        public static bool ShowError
        {
            get
            {
                if (_ShowError == null)
                {
                    var value = PlayerPrefs.GetInt("CDebugConfig.ShowError", 1);
                    _ShowError = value == 1;
                }
                return _ShowError.Value;
            }
            set
            {
                var intValue = value ? 1 : 0;
                PlayerPrefs.SetInt("CDebugConfig.ShowError", intValue);
                _ShowError = value;
                needShowLogLevelVersion++;
            }
        }

        private static bool? _ShowException;
        public static bool ShowException
        {
            get
            {
                if (_ShowException == null)
                {
                    var value = PlayerPrefs.GetInt("CDebugConfig.ShowException", 1);
                    _ShowException = value == 1;
                }
                return _ShowException.Value;
            }
            set
            {
                var intValue = value ? 1 : 0;
                PlayerPrefs.SetInt("CDebugConfig.ShowException", intValue);
                _ShowException = value;
                needShowLogLevelVersion++;
            }
        }

        private static bool? _PauseOnError;
        public static bool PauseOnError
        {
            get
            {
                if (_PauseOnError == null)
                {
                    var value = PlayerPrefs.GetInt("CDebugConfig.PauseOnError", 0);
                    _PauseOnError = value == 1;
                }
                return _PauseOnError.Value;
            }
            set
            {
                var intValue = value ? 1 : 0;
                PlayerPrefs.SetInt("CDebugConfig.PauseOnError", intValue);
                _PauseOnError = value;
            }
        }

        private static bool? _PauseOnException;
        public static bool PauseOnException
        {
            get
            {
                if (_PauseOnException == null)
                {
                    var value = PlayerPrefs.GetInt("CDebugConfig.PauseOnException", 0);
                    _PauseOnException = value == 1;
                }
                return _PauseOnException.Value;
            }
            set
            {
                var intValue = value ? 1 : 0;
                PlayerPrefs.SetInt("CDebugConfig.PauseOnException", intValue);
                _PauseOnException = value;
            }
        }

        #region LogLevel
        public static bool GetShowLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Log:
                    return ShowLog;
                case LogLevel.Warning:
                    return ShowWarning;
                case LogLevel.Error:
                    return ShowError;
                case LogLevel.Exception:
                    return ShowException;
                default:
                    return false;
            }
        }

        public static void SetShowLogLevel(LogLevel level, bool value)
        {
            switch (level)
            {
                case LogLevel.Log:
                    ShowLog = value;
                    break;
                case LogLevel.Warning:
                    ShowWarning = value;
                    break;
                case LogLevel.Error:
                    ShowError = value;
                    break;
                case LogLevel.Exception:
                    ShowException = value;
                    break;
            }
        }

        public static bool NeedShowLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Log:
                    return ShowLog;
                case LogLevel.Warning:
                    return ShowWarning;
                case LogLevel.Error:
                    return ShowError;
                case LogLevel.Exception:
                    return ShowException;
                default:
                    return true;
            }
        }
        #endregion

        #region ConsoleLogView Prefs
        private static float? _LogLayoutAndDetailRate;
        public static float LogLayoutAndDetailRate
        {
            get
            {
                if (_LogLayoutAndDetailRate == null)
                {
                    _LogLayoutAndDetailRate = PlayerPrefs.GetFloat("CDebugConfig.LogLayoutAndDetailRate", 300f); ;
                }
                return _LogLayoutAndDetailRate.Value;
            }
            set
            {
                PlayerPrefs.SetFloat("CDebugConfig.LogLayoutAndDetailRate", value);
                _LogLayoutAndDetailRate = value;
            }
        }
        #endregion

        #region Ip Prefs
        private static string _IpPrefs;
        public static string IpPrefs
        {
            get
            {
                if (_IpPrefs == null)
                {
                    _IpPrefs = PlayerPrefs.GetString("CDebugConfig.IpPrefs", "");
                }
                return _IpPrefs;
            }
            set
            {
                PlayerPrefs.SetString("CDebugConfig.IpPrefs", value);
                _IpPrefs = value;
            }
        }
        #endregion
    }
}