using ClassifiedConsole.Runtime;
using UnityEditor;
using UnityEngine.UIElements;

namespace ClassifiedConsole.Editor
{
    internal class ClassifiedConsoleWindow : EditorWindow
    {
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void FocusConsoleIfOpen()
        {
            EditorWindow.FocusWindowIfItsOpen<ClassifiedConsoleWindow>();
        }

        public static ClassifiedConsoleWindow windowRoot;

        private const int WindowFps = 30;
        private bool isRemote = false;
        public RemoteRequestor remoteRequestor;
        private VisualElement rootView;
        private ConsoleLeftRightView mainConsole;
        private CmdView cmdView;
        private ConsoleTopBar topBar;
        public EditorLogFile editorLogFile;

        private bool hasInit = false;
        private void OnEnable()
        {
            this.remoteRequestor = new RemoteRequestor();
            this.remoteRequestor.OnStateChanged += this.OnRemoteStateChanged;

            this.editorLogFile = new EditorLogFile();
            this.editorLogFile.OnEditorLogFileRefresh += this.MarkDirty;
            this.editorLogFile.OnTargetLogFileChanged += this.OnTargetLogFileChanged;
            ClassifiedConsoleWindow.windowRoot = this;

            this.hasInit = false;

            this.BuildWindow();
            this.InitLocalCurrentLogFile();

            this.hasInit = true;
        }

        private void OnRemoteStateChanged(RemoteRequestor.ConnectionState state)
        {
            if (isRemote == false)
            {
                if (state == RemoteRequestor.ConnectionState.ConnectedToRemote)
                {
                    this.editorLogFile.InitFromRemote(this.remoteRequestor);
                    this.MarkDirty();
                    this.isRemote = true;
                }
            }
            else if (isRemote == true)
            {
                if (state != RemoteRequestor.ConnectionState.ConnectedToRemote)
                {
                    this.InitLocalCurrentLogFile();
                    this.isRemote = false;
                }
            }
        }

        private bool isDirty = false;
        private void MarkDirty()
        {
            this.isDirty = true;
        }

        public void MarkDirtyImmediatly()
        {
            this.MarkDirty();
            this.dirtyDelay = 0;
        }

        private int dirtyDelay = 0;
        private void Update()
        {
            if (this.isDirty)
            {
                if (dirtyDelay == WindowFps)
                {
                    this.AfterUpdate();
                }
                dirtyDelay--;
                if (dirtyDelay < 0)
                {
                    this.RenderConsole();
                    this.isDirty = false;
                    dirtyDelay = WindowFps;
                }
            }
        }

        private void AfterUpdate()
        {
            this.mainConsole.logLayout.TryBackToBottom();
        }

        private void OnDisable()
        {
            this.editorLogFile.OnEditorLogFileRefresh -= this.MarkDirty;
            this.editorLogFile.OnTargetLogFileChanged -= this.OnTargetLogFileChanged;
            this.editorLogFile.Dispose();
            this.editorLogFile = null;
        }

        private void BuildWindow()
        {
            this.rootView = rootVisualElement;

            this.topBar = new ConsoleTopBar();
            this.rootView.Add(this.topBar);

            this.mainConsole = new ConsoleLeftRightView();
            this.rootView.Add(this.mainConsole);

            this.cmdView = new CmdView();
            this.rootView.Add(this.cmdView);
        }

        private void OnTargetLogFileChanged()
        {
            this.mainConsole.logLayout.lastClick = null;
            this.mainConsole.logDetail.ClearMsg();
            this.topBar.RefreshHistoryMenuItem();
            this.MarkDirtyImmediatly();
        }

        public void TryLoadHistoryLogFile(int logFileId)
        {
            if (this.isRemote)
            {
                this.editorLogFile.InitFromRemote(this.remoteRequestor, logFileId);
            }
            else
            {
                var logFile = ManagedLogFile.GetLogFile(logFileId);
                this.editorLogFile.InitFromLocal(logFile);
            }
        }

        private void InitLocalCurrentLogFile()
        {
            var logFile = ManagedLogFile.Current;
            this.editorLogFile.InitFromLocal(logFile);
        }

        private void RefreshLogLayout()
        {
            this.mainConsole.logLayout.Refresh(this.editorLogFile.showingLogIndexList);
        }

        public void FilterTryNotify()
        {
            if (this.hasInit)
            {
                this.MarkDirty();
            }
        }

        private void RenderConsole()
        {
            this.editorLogFile.RefreshWithOutNotify();
            this.topBar.logCount = this.editorLogFile.logCount;
            this.topBar.warningCount = this.editorLogFile.warningCount;
            this.topBar.errorCount = this.editorLogFile.errorCount;
            this.topBar.exceptionCount = this.editorLogFile.exceptionCount;
            this.mainConsole.subSystemFilter.RefreeshSubSystem();
            this.mainConsole.logLayout.Refresh(this.editorLogFile.showingLogIndexList);
        }
    }
}