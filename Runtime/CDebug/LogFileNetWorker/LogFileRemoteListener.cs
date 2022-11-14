using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
using HttpRemoteConnector;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole
{
    public class LogFileRemoteListener : IRemoteListener
    {
        private int _port = CDebugSettings.Instance.port;
        public int port => this._port;

        public string GetHandler(string[] paramArray)
        {
            return "Get";
        }

        public string PostHandler(string param)
        {
            var requestParam = UnityEngine.JsonUtility.FromJson<LogFileNetRequestParam>(param);
            var responseParam = new LogFileNetResponseParam();
            if (requestParam.IsGetLogFileList)
            {
                responseParam.currentLogFileID = ManagedLogFile.currentFileId;
                responseParam.LogFileIdList = ManagedLogFile.GetAvaliableArchivedLogFile();
                return UnityEngine.JsonUtility.ToJson(responseParam);
            }
            else if (requestParam.IsGetLogCount)
            {
                var logFileId = requestParam.LogFileId;
                var logFile = ManagedLogFile.GetLogFile(logFileId);
                responseParam.logCount = logFile.logCount;
                return UnityEngine.JsonUtility.ToJson(responseParam);
            }
            else if (requestParam.IsGetLog)
            {
                responseParam.remoteLogList = new List<LogWriter>();

                var logFileId = requestParam.LogFileId;
                var logFile = ManagedLogFile.GetLogFile(logFileId);
                var logCount = logFile.logCount;
                var startLogIndex = requestParam.GetLogStartIndex;
                var getLogCount = requestParam.GetLogLength;
                for (int i = startLogIndex; i < startLogIndex + getLogCount; i++)
                {
                    if (i >= logCount)
                    {
                        break;
                    }

                    var logReader = logFile[i];
                    var remoteLogWriter = logReader.ToRemoteLog();
                    responseParam.remoteLogList.Add(remoteLogWriter);
                }

                return UnityEngine.JsonUtility.ToJson(responseParam);
            }
            else if (requestParam.IsGetSubSystem)
            {
                var keyIenum = CDebugSubSystemEnumConfig.GetAllSubSystemList();
                var keyList = new List<int>(keyIenum);
                var valueList = new List<string>();
                foreach (var key in keyList)
                {
                    var value = CDebugSubSystemEnumConfig.GetSubSystemName(key);
                    valueList.Add(value);
                }

                responseParam.subSystemKey = keyList;
                responseParam.subSystemName = valueList;
                return UnityEngine.JsonUtility.ToJson(responseParam);
            }
            else
            {
                return "";
            }
        }

        public static void Start(bool ignoreEditor = true)
        {
            if (ignoreEditor && Application.isEditor)
            {
                return;
            }
            else
            {
                var listener = new LogFileRemoteListener();
                listener.StartListen();
            }
        }
    }
}