using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole.Runtime
{
    internal static class ManagedLogFile
    {
        public static LogFile Current
        {
            get
            {
                return GetCurrentLogFile();
            }
        }

        public static LogFile GetLogFile(int logFileId)
        {
            return GetOrCreateLogFile(logFileId);
            // var isValid = IsLogFileValid(logFileId);
            // if (isValid)
            // {
            //     return GetOrCreateLogFile(logFileId);
            // }
            // else
            // {
            //     UnityEngine.Debug.LogError($"[ClassifiedConsole_ManagedLogFile] {logFileId} Not Exists");
            //     return null;
            // }
        }

        public static Action OnCurrentLogFileChanged;

        public static void WriteLog(LogWriter log)
        {
            if (Current.TempLogCount >= CDebugSettings.Instance.limitLogCount)
            {
                Archive();
            }
            Current.AppendLog(log);
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void TryArchiveOnPlay()
        {
            if (CDebugConfig.ArchiveOnPlay)
            {
                Archive();
            }
        }

        public static void Archive()
        {
            // var currentDir = Application.persistentDataPath + $"/Log/{currentFileId}/";
            var currentDir = Path.Combine(LogFilePathConfig.versionRoot, currentFileId.ToString());
            if (Directory.Exists(currentDir))
            {
                var files = Directory.GetFiles(currentDir);
                if (files.Length == 1 && Path.GetFileName(files[0]) == "_Indexer_")
                {
                    return;
                }
                currentFileId++;
                DeleteNoKeepLogFile();
                OnCurrentLogFileChanged?.Invoke();
            }
        }

        private static void DeleteNoKeepLogFile()
        {
            var logDir = Path.GetFullPath(LogFilePathConfig.versionRoot);
            var dirArray = Directory.GetDirectories(logDir);
            foreach (var dir in dirArray)
            {
                var dirName = Path.GetFileName(dir);
                var isNum = int.TryParse(dirName, out int logId);
                if (isNum == false) continue;
                var isKeep = IsLogFileIdKeep(logId);
                if (isKeep)
                {
                    continue;
                }
                if (logFileDic.ContainsKey(logId))
                {
                    logFileDic[logId].ReleaseIO();
                    logFileDic.Remove(logId);
                }
                Directory.Delete(dir, true);
            }
        }

        public static bool IsLogFileIdKeep(int logFileId)
        {
            var keepLogFileCount = CDebugSettings.Instance.keepLogFileCount;

            if (currentFileId - keepLogFileCount < logFileId && logFileId <= currentFileId)
            {
                // keep
                return true;
            }
            return false;
        }

        private static int _currentFileId = -1;
        public static int currentFileId
        {
            get
            {
                if (_currentFileId == -1)
                {
                    _currentFileId = PlayerPrefs.GetInt("LogFileId", 0);
                }
                return _currentFileId;
            }
            private set
            {
                PlayerPrefs.SetInt("LogFileId", value);
                _currentFileId = value;
            }
        }

        private static Dictionary<int, LogFile> logFileDic = new Dictionary<int, LogFile>();
        private static LogFile GetCurrentLogFile()
        {
            return GetOrCreateLogFile(currentFileId);
        }

        private static LogFile GetOrCreateLogFile(int logFileId)
        {
            if (!logFileDic.ContainsKey(logFileId))
            {
                var logFile = CreateLogFile(logFileId);
                logFileDic.Add(logFileId, logFile);
            }
            return logFileDic[logFileId];
        }

        private static LogFile CreateLogFile(int fileId)
        {
            var filePath = GetLogFilePath(fileId);
            var logFile = new LogFile(filePath, fileId);
            return logFile;
        }

        private static string GetLogFilePath(int fileId)
        {
            // var filePath = Application.persistentDataPath + $"/Log/{fileId}";
            var filePath = Path.Combine(LogFilePathConfig.versionRoot, fileId.ToString());
            return filePath;
        }

        public static List<int> GetAvaliableArchivedLogFile()
        {
            var maxKeepLogFileCount = CDebugSettings.Instance.keepLogFileCount;
            var ret = new List<int>();
            for (int i = currentFileId; i > currentFileId - maxKeepLogFileCount; i--)
            {
                var logFilePath = GetLogFilePath(i);
                if (Directory.Exists(logFilePath))
                {
                    ret.Add(i);
                }
                else
                {
                    break;
                }
            }

            return ret;
        }

        public static bool IsLogFileValid(int logFileId)
        {
            var isKeep = IsLogFileIdKeep(logFileId);
            var logFilePath = GetLogFilePath(logFileId);
            var isExists = Directory.Exists(logFilePath);
            return isKeep && isExists;
        }

        public static void CleanUpManagedLogFile()
        {
            foreach (var kv in logFileDic)
            {
                kv.Value.ReleaseIO();
            }
            logFileDic.Clear();
            var logDir = Path.GetFullPath(LogFilePathConfig.root);
            if (Directory.Exists(logDir))
            {
                Directory.Delete(logDir, true);
            }

            ManagedLogFile.currentFileId = 1;

            ManagedLogFile.OnCurrentLogFileChanged?.Invoke();
        }
    }
}