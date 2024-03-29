using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ClassifiedConsole.Runtime;
using UnityEngine;
using UnityEngine.Profiling;

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

        private List<EditorLogReader> logReaderList = new List<EditorLogReader>();

        // private List<int> logReaderIndexList = new List<int>();
        // private Dictionary<int, EditorLogReader> logReaderDic = new Dictionary<int, EditorLogReader>();

        public List<int> showingLogIndexList = new List<int>();
        // public Dictionary<int, int> subSystemLogCount = new Dictionary<int, int>();
        public Dictionary<string, int> md5CountDic = new Dictionary<string, int>();

        public int logCount;
        public int warningCount;
        public int errorCount;
        public int exceptionCount;

        public RemoteRequestor remoteLogRequestor { get; private set; }
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

        public bool isRemote { get => this.remoteLogRequestor != null; }

        public event Action OnTargetLogFileChanged;
        public void InitFromLocal(LogFile logFile)
        {
            this.UnRegistsLogFile();
            this.remoteLogRequestor = null;

            this.localLogFile = logFile;
            this.remoteLogFile?.ReleaseIO();
            this.remoteLogFile = null;

            this.ClearEditorLogFileState();
            this.PrepareEditorLogData();

            this.RegistsLogFile();
            this.OnTargetLogFileChanged?.Invoke();
        }

        public void InitFromRemote(RemoteRequestor remoteRequestor, int? targetLogFileId = null)
        {
            this.UnRegistsLogFile();
            this.remoteLogRequestor = remoteRequestor;
            this.remoteLogFile?.ReleaseIO();
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

            // // 拉取SubSystem信息
            // if (this.isRemote)
            // {
            //     var requestParam = new LogFileNetRequestParam();
            //     requestParam.IsGetSubSystem = true;
            //     var requestJson = requestParam.ToJson();
            //     LogFileNetResponseParam responseParam = null;
            //     try
            //     {
            //         var respneseJson = await this.remoteLogRequestor.PostAsync(requestJson, true);
            //         responseParam = LogFileNetResponseParam.FromJson(respneseJson);
            //     }
            //     catch { }
            //     if (responseParam == null)
            //     {
            //         this.receivingLoopTask.SetResult(false);
            //         return;
            //     }
            //     CDebugSubSystemEnumConfig.remote_SubSystemEnumDic = new Dictionary<int, string>();
            //     CDebugSubSystemEnumConfig.remote_SubSystemEnumLabelDic = new Dictionary<int, string>();
            //     for (int i = 0; i < responseParam.subSystemKey.Count; i++)
            //     {
            //         var key = responseParam.subSystemKey[i];
            //         var value = responseParam.subSystemName[i];
            //         var label = responseParam.subSystemLabel[i];
            //         CDebugSubSystemEnumConfig.remote_SubSystemEnumDic.Add(key, value);
            //         CDebugSubSystemEnumConfig.remote_SubSystemEnumLabelDic.Add(key, label);
            //     }
            // }

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

            var tempPath = Path.Combine(LogFilePathConfig.root, "RemoteLog");
            try
            {
                // 这里有可能报错（但是现在IO有Release了，应该不会了）
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }
            }
            catch { }

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
                    await Task.Delay(100);
                    if (this.NeedReceiving(receivingId) == false) return;
                }
                else
                {
                    // 当前没有剩余Log 等待时间长一点
                    await Task.Delay(100);
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

            // this.logReaderIndexList.Clear();
            // this.logReaderDic.Clear();
            this.logReaderList.Clear();

            this.showingLogIndexList.Clear();
            // this.subSystemLogCount.Clear();
            this.subSystem_Log_Count.Clear();
            this.subSystem_Warning_Count.Clear();
            this.subSystem_Error_Count.Clear();
            this.subSystem_Exception_Count.Clear();
            this.md5CountDic.Clear();
            this.SearchFilter = null;
        }

        private void PrepareEditorLogData()
        {
            var logCount = this.targetLogFile.logCount;

            this.subSystem_Dic.Clear();
            foreach (var subSystem in CDebugSubSystemEnumConfig.GetAllSubSystemList())
            {
                if (CDebugSubSystemEnumConfig.GetSubSystemName(subSystem) == subSystem.ToString())
                {
                    continue;
                }
                this.subSystem_Dic[subSystem] = subSystem;
            }

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
            var targetLogCount = this.targetLogFile.logCount;
            // var currentIndex = this.logReaderIndexList.Count;
            var currentIndex = this.logReaderList.Count;

            bool needPause = false;
            if (targetLogCount > currentIndex)
            {
                for (int i = currentIndex; i < targetLogCount; i++)
                {
                    needPause = needPause || this.OnAppendLogInternal(i);
                }

                this.OnEditorLogFileRefresh?.Invoke();

                if (needPause && UnityEngine.Application.isPlaying)
                {
                    ClassifiedConsoleWindow.windowRoot.MarkDirtyImmediatly();
                    UnityEditor.EditorApplication.isPaused = true;
                }
            }
        }

        public void RefreshWithOutNotify()
        {
            this.RefreshShowingList();
        }

        private bool OnAppendLogInternal(int index)
        {
            var logReader = this.targetLogFile[index];
            // this.logReaderIndexList.Add(index);
            var logIo = this.targetLogFile.GetLogIo(logReader.logFileName);
            UnityEngine.Profiling.Profiler.BeginSample("New EditorLogReader [EditorOnly]");
            var editorLogReader = new EditorLogReader(logReader, logIo);
            UnityEngine.Profiling.Profiler.EndSample();
            // this.logReaderDic.Add(index, editorLogReader);
            this.logReaderList.Add(editorLogReader);

            var subSystems = logReader.subSystem;
            var level = logReader.level;
            this.Collect_System_LogCount(level, subSystems);
            if (level == LogLevel.Error && CDebugWindowConfig.PauseOnError)
            {
                return true;
            }
            if (level == LogLevel.Exception && CDebugWindowConfig.PauseOnException)
            {
                return true;
            }
            return false;
        }

        private Dictionary<string, bool> md5_Search_Contains = new Dictionary<string, bool>();

        private void RefreshShowingList()
        {
            // this.subSystem_Log_Count.Clear();
            // this.subSystem_Warning_Count.Clear();
            // this.subSystem_Error_Count.Clear();
            // this.subSystem_Exception_Count.Clear();

            this.logCount = 0;
            this.warningCount = 0;
            this.errorCount = 0;
            this.exceptionCount = 0;
            this.showingLogIndexList.Clear();
            this.md5CountDic.Clear();
            this.md5_Search_Contains.Clear();
            var collapse = CDebugWindowConfig.Collapse;
            var searchContent = this.SearchFilter;
            var hasSearchContent = !string.IsNullOrEmpty(searchContent);

            // foreach (var index in this.logReaderIndexList)
            for (int index = 0; index < this.logReaderList.Count; index++)
            {
                // var logReader = this.logReaderDic[index];

                var logReader = this.logReaderList[index];
                var level = logReader.level;
                var levelShow = EditorLogReader.NeedShowLogLevel(level);
                var subSystemShow = logReader.NeedShowSubSystem;
                var display = levelShow && subSystemShow;
                var md5 = logReader.md5;

                if (subSystemShow)
                {
                    if (level == LogLevel.Log) this.logCount++;
                    else if (level == LogLevel.Warning) this.warningCount++;
                    else if (level == LogLevel.Error) this.errorCount++;
                    else if (level == LogLevel.Exception) this.exceptionCount++;
                }

                var subSystem = logReader.subSystem;
                // this.Collect_System_LogCount(level, subSystem);

                if (display == false)
                {
                    continue;
                }

                if (hasSearchContent && display)
                {
                    bool inSearch = false;
                    if (this.md5_Search_Contains.ContainsKey(md5))
                    {
                        inSearch = this.md5_Search_Contains[md5];
                    }
                    else
                    {
                        var msg = logReader.msg;

                        var filterIndex = msg.IndexOf(this.SearchFilter, StringComparison.OrdinalIgnoreCase);
                        inSearch = filterIndex != -1;
                        this.md5_Search_Contains.Add(md5, inSearch);
                    }

                    if (inSearch == false) continue;
                }
                if (collapse)
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

                if (display)
                {
                    this.showingLogIndexList.Add(index);
                }
            }
        }

        private void Collect_System_LogCount(LogLevel level, int subSystem)
        {
            if (level == LogLevel.Log)
            {
                if (!this.subSystem_Log_Count.ContainsKey(subSystem))
                {
                    this.subSystem_Log_Count.Add(subSystem, 0);
                }
                this.subSystem_Log_Count[subSystem]++;
            }
            else if (level == LogLevel.Warning)
            {
                if (!this.subSystem_Warning_Count.ContainsKey(subSystem))
                {
                    this.subSystem_Warning_Count.Add(subSystem, 0);
                }
                this.subSystem_Warning_Count[subSystem]++;
            }
            else if (level == LogLevel.Error)
            {
                if (!this.subSystem_Error_Count.ContainsKey(subSystem))
                {
                    this.subSystem_Error_Count.Add(subSystem, 0);
                }
                this.subSystem_Error_Count[subSystem]++;
            }
            else if (level == LogLevel.Exception)
            {
                if (!this.subSystem_Exception_Count.ContainsKey(subSystem))
                {
                    this.subSystem_Exception_Count.Add(subSystem, 0);
                }
                this.subSystem_Exception_Count[subSystem]++;
            }
            this.subSystem_Dic[subSystem] = subSystem;
        }

        public int GetCollapseCount(string md5)
        {
            if (this.md5CountDic.ContainsKey(md5))
            {
                return this.md5CountDic[md5];
            }
            return 0;
        }

        public EditorLogReader this[int index]
        {
            get
            {
                return this.logReaderList[index];
            }
        }

        public int GetLogCount(int subSystemId, LogLevel level)
        {
            if (level == LogLevel.Log)
            {
                this.subSystem_Log_Count.TryGetValue(subSystemId, out int value);
                return value;
            }
            if (level == LogLevel.Warning)
            {
                this.subSystem_Warning_Count.TryGetValue(subSystemId, out int value);
                return value;
            }
            if (level == LogLevel.Error)
            {
                this.subSystem_Error_Count.TryGetValue(subSystemId, out int value);
                return value;
            }
            if (level == LogLevel.Exception)
            {
                this.subSystem_Exception_Count.TryGetValue(subSystemId, out int value);
                return value;
            }
            return 0;
        }

        public string SearchFilter { get; set; }

        private Dictionary<int, int> subSystem_Log_Count = new Dictionary<int, int>();
        private Dictionary<int, int> subSystem_Warning_Count = new Dictionary<int, int>();
        private Dictionary<int, int> subSystem_Error_Count = new Dictionary<int, int>();
        private Dictionary<int, int> subSystem_Exception_Count = new Dictionary<int, int>();

        public Dictionary<int, int> subSystem_Dic = new Dictionary<int, int>();
        // public HashSet<int> subSystemList = new HashSet<int>();
    }
}