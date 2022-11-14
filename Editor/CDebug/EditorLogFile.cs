using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ClassifiedConsole.Runtime;
using HttpRemoteConnector;
using UnityEngine;

namespace ClassifiedConsole.Editor
{
    internal class EditorLogFile
    {
        public EditorLogFile()
        {
            ManagedLogFile.OnCurrentLogFileChanged += this.OnLocalLogFileChanged;
        }

        private void OnLocalLogFileChanged()
        {
            if (this.isRemote == false)
            {
                var currentLogFile = ManagedLogFile.Current;
                this.InitFromLocal(currentLogFile);
            }
        }

        private List<int> logReaderIndexList = new List<int>();
        private Dictionary<int, LogReader> logReaderDic = new Dictionary<int, LogReader>();

        public List<int> showingLogIndexList = new List<int>();
        public Dictionary<int, int> subSystemLogCount = new Dictionary<int, int>();
        public Dictionary<string, int> md5CountDic = new Dictionary<string, int>();

        public int logCount;
        public int warningCount;
        public int errorCount;
        public int exceptionCount;

        private RemoteRequestor remoteLogRequestor;
        private LogFile remoteLogFile;
        private LogFile localLogFile;
        private LogFile targetLogFile
        {
            get
            {
                if (this.remoteLogRequestor != null)
                {
                    return this.remoteLogFile;
                }
                else
                {
                    return this.localLogFile;
                }
            }
        }

        private int CurrentLogFileId
        {
            get
            {
                if (this.isRemote)
                {
                    return this._RemoteCurrentLogFileId;
                }
                else
                {
                    return ManagedLogFile.currentFileId;
                }
            }
        }

        public bool IsShowingCurrentLogFile
        {
            get
            {
                if (this.targetLogFile == null)
                {
                    return true;
                }
                return this.targetLogFile.logFileId == this.CurrentLogFileId;
            }
        }

        public int TargetLogFileID
        {
            get
            {
                if (this.targetLogFile == null)
                {
                    return -1;
                }
                return this.targetLogFile.logFileId;
            }
        }

        public List<int> ArchivedLogFileIdList
        {
            get
            {
                if (this.isRemote && this._RemoteLogFileIdList == null)
                {
                    return new List<int>();
                }
                else if (this.isRemote)
                {
                    return this._RemoteLogFileIdList;
                }
                else
                {
                    return ManagedLogFile.GetAvaliableArchivedLogFile();
                }
            }
        }

        private bool isRemote { get => this.remoteLogRequestor != null; }

        public event Action OnTargetLogFileChanged;
        public void InitFromLocal(LogFile logFile)
        {
            this.UnRegistsLogFile();
            this.remoteLogRequestor = null;

            this.localLogFile = logFile;
            this.remoteLogFile = null;

            CDebugSubSystemEnumConfig.remote_SubSystemEnumDic = null;

            this.ClearEditorLogFileState();
            this.PrepareEditorLogData();

            this.RegistsLogFile();
            this.OnTargetLogFileChanged?.Invoke();
        }

        public void InitFromRemote(RemoteRequestor remoteRequestor, int? targetLogFileId = null)
        {
            this.UnRegistsLogFile();
            this.remoteLogRequestor = remoteRequestor;
            this.remoteLogFile = null;
            this.localLogFile = null;

            this.ClearEditorLogFileState();
            this.OnTargetLogFileChanged?.Invoke();

            this.CreateRemoteLogFileInBackground(targetLogFileId);
        }

        #region Remote Part
        private int _RemoteCurrentLogFileId;
        private List<int> _RemoteLogFileIdList;
        private TaskCompletionSource<bool> receivingLoopTask;
        private async void CreateRemoteLogFileInBackground(int? targetLogFileId = null)
        {
            if (this.receivingLoopTask != null)
            {
                await this.receivingLoopTask.Task;
                this.receivingLoopTask = null;
            }
            this.receivingLoopTask = new TaskCompletionSource<bool>();

            // 拉取SubSystem信息
            if (this.isRemote)
            {
                var requestParam = new LogFileNetRequestParam();
                requestParam.IsGetSubSystem = true;
                var requestJson = requestParam.ToJson();
                LogFileNetResponseParam responseParam = null;
                try
                {
                    var respneseJson = await this.remoteLogRequestor.PostAsync(requestJson, true);
                    responseParam = LogFileNetResponseParam.FromJson(respneseJson);
                }
                catch { }
                if (responseParam == null)
                {
                    this.receivingLoopTask.SetResult(false);
                    return;
                }
                CDebugSubSystemEnumConfig.remote_SubSystemEnumDic = new Dictionary<int, string>();
                for (int i = 0; i < responseParam.subSystemKey.Count; i++)
                {
                    var key = responseParam.subSystemKey[i];
                    var value = responseParam.subSystemName[i];
                    CDebugSubSystemEnumConfig.remote_SubSystemEnumDic.Add(key, value);
                }
            }

            // 拉取LogFile信息
            if (this.isRemote)
            {
                var requestParam = new LogFileNetRequestParam();
                requestParam.IsGetLogFileList = true;
                var requestJson = requestParam.ToJson();
                LogFileNetResponseParam responseParam = null;
                try
                {
                    var respneseJson = await this.remoteLogRequestor.PostAsync(requestJson, true);
                    responseParam = LogFileNetResponseParam.FromJson(respneseJson);
                }
                catch { }
                if (responseParam == null)
                {
                    this.receivingLoopTask.SetResult(false);
                    return;
                }
                this._RemoteCurrentLogFileId = responseParam.currentLogFileID;
                if (targetLogFileId == null)
                {
                    targetLogFileId = responseParam.currentLogFileID;
                }
                this._RemoteLogFileIdList = responseParam.LogFileIdList;
            }

            var tempPath = Application.persistentDataPath;
            tempPath = Path.Combine(tempPath, "RemoteLog");
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            tempPath = Path.Combine(tempPath, $"{targetLogFileId.Value}");
            var remoteLogFile = new LogFile(tempPath, targetLogFileId.Value);
            this.UnRegistsLogFile();
            this.remoteLogFile = remoteLogFile;
            this.RegistsLogFile();
            this.PrepareEditorLogData();

            this.OnTargetLogFileChanged?.Invoke();
            this.LoopReceiver(targetLogFileId.Value);
        }

        private bool NeedReceiving(int receivingId)
        {
            var ret = true;
            if (this.isRemote == false) ret = false;
            if (this.TargetLogFileID != receivingId) ret = false;
            if (ret == false)
            {
                this.receivingLoopTask.TrySetResult(true);
            }
            return ret;
        }

        private async void LoopReceiver(int receivingId)
        {
            while (true)
            {
                var requestParam = new LogFileNetRequestParam();
                requestParam.IsGetLogCount = true;
                requestParam.LogFileId = receivingId;
                var requestJson = requestParam.ToJson();

                LogFileNetResponseParam responseParam = null;
                try
                {
                    var respneseJson = await this.remoteLogRequestor.PostAsync(requestJson, true);
                    responseParam = LogFileNetResponseParam.FromJson(respneseJson);
                }
                catch { }
                if (responseParam == null)
                {
                    this.receivingLoopTask.SetResult(false);
                    return;
                }

                if (this.NeedReceiving(receivingId) == false) return;

                var remoteLogCount = responseParam.logCount;

                if (this.targetLogFile.logCount < remoteLogCount)
                {
                    await ReceiveNewLogAsync(this.targetLogFile.logCount, receivingId);
                    // 如果Remote 还有剩余的 Log 等待时间短一点
                    await Task.Delay(200);
                    if (this.NeedReceiving(receivingId) == false) return;
                }
                else
                {
                    // 当前没有剩余Log 等待时间长一点
                    await Task.Delay(5000);
                }
                if (this.NeedReceiving(receivingId) == false) return;
            }
        }

        private async Task ReceiveNewLogAsync(int startIndex, int logFileId)
        {
            var requestParam = new LogFileNetRequestParam();
            requestParam.IsGetLog = true;
            requestParam.LogFileId = logFileId;
            requestParam.GetLogStartIndex = startIndex;
            requestParam.GetLogLength = 1000;
            var requestJson = requestParam.ToJson();

            LogFileNetResponseParam responseParam = null;
            try
            {
                var respneseJson = await this.remoteLogRequestor.PostAsync(requestJson, true);
                responseParam = LogFileNetResponseParam.FromJson(respneseJson);
            }
            catch { }
            if (responseParam == null)
            {
                this.receivingLoopTask.SetResult(false);
                return;
            }

            if (this.NeedReceiving(logFileId) == false) return;

            foreach (var logWriter in responseParam.remoteLogList)
            {
                this.targetLogFile.AppendLog(logWriter);
            }
        }
        #endregion

        private void ClearEditorLogFileState()
        {
            this._RemoteCurrentLogFileId = -1;
            this._RemoteLogFileIdList = null;

            this.logReaderIndexList.Clear();
            this.logReaderDic.Clear();
            this.showingLogIndexList.Clear();
            this.subSystemLogCount.Clear();
            this.md5CountDic.Clear();
            this.SearchFilter = null;
        }

        private void PrepareEditorLogData()
        {
            var logCount = this.targetLogFile.logCount;
            for (int index = 0; index < logCount; index++)
            {
                this.OnAppendLogInternal(index);
            }
        }

        public void Dispose()
        {
            this.UnRegistsLogFile();
        }

        private void RegistsLogFile()
        {
            if (this.targetLogFile != null)
            {
                this.targetLogFile.OnAppendLog += this.AppendLog_Notify;
            }
        }

        private void UnRegistsLogFile()
        {
            if (this.targetLogFile != null)
            {
                this.targetLogFile.OnAppendLog -= this.AppendLog_Notify;
            }
        }

        public event Action OnEditorLogFileRefresh;
        private void AppendLog_Notify()
        {
            var index = this.targetLogFile.logCount - 1;
            this.OnAppendLogInternal(index);
            this.OnEditorLogFileRefresh?.Invoke();
        }

        public void RefreshWithOutNotify()
        {
            this.RefreshShowingList();
        }

        private void OnAppendLogInternal(int index)
        {
            var logReader = this.targetLogFile[index];
            this.logReaderIndexList.Add(index);
            this.logReaderDic.Add(index, logReader);

            var subSystems = logReader.subSystem;
            foreach (var subSystem in subSystems)
            {
                if (!this.subSystemLogCount.ContainsKey(subSystem))
                {
                    this.subSystemLogCount[subSystem] = 0;
                }
                this.subSystemLogCount[subSystem] += 1;
            }
        }

        private void RefreshShowingList()
        {
            this.logCount = 0;
            this.warningCount = 0;
            this.errorCount = 0;
            this.exceptionCount = 0;
            this.showingLogIndexList.Clear();
            this.md5CountDic.Clear();
            foreach (var index in this.logReaderIndexList)
            {
                if (!string.IsNullOrEmpty(this.SearchFilter))
                {
                    var msg = this.logReaderDic[index].msg;
                    var filterIndex = msg.IndexOf(this.SearchFilter, StringComparison.OrdinalIgnoreCase);
                    if (filterIndex == -1) continue;
                }
                var md5 = this.logReaderDic[index].md5;
                if (CDebugConfig.Collapse)
                {
                    if (this.md5CountDic.ContainsKey(md5))
                    {
                        this.md5CountDic[md5] += 1;
                        continue;
                    }
                    else
                    {
                        this.md5CountDic[md5] = 1;
                    }
                }
                var level = this.logReaderDic[index].level;

                var levelShow = this.logReaderDic[index].NeedShowLogLevel;
                var subSystemShow = this.logReaderDic[index].NeedShowSubSystem;
                var display = levelShow && subSystemShow;
                if (display)
                {
                    this.showingLogIndexList.Add(index);
                }

                if (subSystemShow)
                {
                    if (level == LogLevel.Log) this.logCount++;
                    else if (level == LogLevel.Warning) this.warningCount++;
                    else if (level == LogLevel.Error) this.errorCount++;
                    else if (level == LogLevel.Exception) this.exceptionCount++;
                }
            }
        }

        public int GetCollapseCount(string md5)
        {
            if (this.md5CountDic.ContainsKey(md5))
            {
                return this.md5CountDic[md5];
            }
            return 0;
        }

        public LogReader this[int index]
        {
            get
            {
                return this.targetLogFile[index];
            }
        }

        public int GetLogCount(int subSystemId)
        {
            if (this.subSystemLogCount.ContainsKey(subSystemId))
            {
                return this.subSystemLogCount[subSystemId];
            }
            return 0;
        }

        public IEnumerable<int> GetShowingSubSystem()
        {
            return this.subSystemLogCount.Keys;
        }

        public string SearchFilter { get; set; }
    }
}