using System;
using System.Collections.Generic;
using System.IO;

namespace ClassifiedConsole.Runtime
{
    internal class LogFileNetRequestParam
    {
        public bool IsGetLogFileList;
        public bool IsGetLogCount;
        public bool IsGetLog;
        public bool IsGetSubSystem;

        public string Cmd;

        public int LogFileId;
        public int GetLogStartIndex;
        public int GetLogLength;

        public string ToJson()
        {
            var json = UnityEngine.JsonUtility.ToJson(this);
            return json;
        }
    }

    [Serializable]
    internal class LogFileNetResponseParam
    {
        public int currentLogFileID;
        public List<int> LogFileIdList;

        public int logCount;

        public List<LogWriter> remoteLogList;

        public List<int> subSystemKey;
        public List<string> subSystemName;
        public List<string> subSystemLabel;

        public bool cmdExecuteSuccess;

        public static LogFileNetResponseParam FromJson(string json)
        {
            var obj = UnityEngine.JsonUtility.FromJson<LogFileNetResponseParam>(json);
            return obj;
        }
    }
}