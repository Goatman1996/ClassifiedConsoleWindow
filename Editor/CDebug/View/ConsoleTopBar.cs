using ClassifiedConsole.Runtime;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleTopBar : Toolbar
    {
        private ConsoleLogLeveTogglel logToggle;
        public int logCount { set => this.logToggle.count = value; }
        private ConsoleLogLeveTogglel warningToggle;
        public int warningCount { set => this.warningToggle.count = value; }
        private ConsoleLogLeveTogglel errorToggle;
        public int errorCount { set => this.errorToggle.count = value; }
        private ConsoleLogLeveTogglel exceptionToggle;
        public int exceptionCount { set => this.exceptionToggle.count = value; }

        private VisualElement left;
        public ConsoleTopBar()
        {
            this.style.minWidth = 1f;
            this.style.justifyContent = Justify.SpaceBetween;

            this.left = new VisualElement();
            left.style.flexDirection = FlexDirection.Row;
            left.style.flexShrink = 0;
            this.Add(left);

            var archiveMenu = this.CreateToolbarMenu();
            left.Add(archiveMenu);
            var collapseToggle = this.CreateCollapseToggle();
            left.Add(collapseToggle);
            var pauseMenu = this.CreatePauseOnMenu();
            left.Add(pauseMenu);
            this.archivedLogFileMenu = this.CreateArchivedLogFileMenu();
            left.Add(this.archivedLogFileMenu);
            var remoteMenu = new ConsoleRemoteMenu();
            this.left.Add(remoteMenu);

            var right = new Toolbar();
            right.style.flexDirection = FlexDirection.Row;
            this.Add(right);
            var searchBar = this.CreateSearchBar();
            right.Add(searchBar);
            this.logToggle = new ConsoleLogLeveTogglel(LogLevel.Log);
            right.Add(this.logToggle);
            this.warningToggle = new ConsoleLogLeveTogglel(LogLevel.Warning);
            right.Add(this.warningToggle);
            this.errorToggle = new ConsoleLogLeveTogglel(LogLevel.Error);
            right.Add(this.errorToggle);
            this.exceptionToggle = new ConsoleLogLeveTogglel(LogLevel.Exception);
            right.Add(this.exceptionToggle);
        }

        private VisualElement CreateToolbarMenu()
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexShrink = 0;

            var archiveBtn = new Button();
            archiveBtn.style.borderRightWidth = 1;
            archiveBtn.style.borderRightColor = UnityEngine.Color.black;
            archiveBtn.text = "Archive";
            archiveBtn.clicked += ManagedLogFile.Archive;
            container.Add(archiveBtn);

            var archiveMenu = new ToolbarMenu();
            archiveMenu.menu.AppendAction("Archive on Play", (act) => { CDebugConfig.ArchiveOnPlay = !CDebugConfig.ArchiveOnPlay; }, (act) =>
            {
                if (CDebugConfig.ArchiveOnPlay)
                {
                    return DropdownMenuAction.Status.Checked;
                }
                else
                {
                    return DropdownMenuAction.Status.Normal;
                }
            });

            archiveMenu.menu.AppendAction("Clean Archive",
            (act) => { ClassifiedConsole.Runtime.ManagedLogFile.CleanUpManagedLogFile(); },
            (act) => { return DropdownMenuAction.Status.Normal; });
            container.Add(archiveMenu);
            return container;
        }

        private ToolbarToggle CreateCollapseToggle()
        {
            var collapseToggle = new ToolbarToggle();
            collapseToggle.RegisterValueChangedCallback((evt) =>
            {
                CDebugConfig.Collapse = evt.newValue;
                ClassifiedConsoleWindow.windowRoot.FilterTryNotify();
            });
            collapseToggle.text = "Collapse";

            collapseToggle.SetValueWithoutNotify(CDebugConfig.Collapse);
            return collapseToggle;
        }

        private ToolbarMenu CreatePauseOnMenu()
        {
            var pauseOnMenu = new ToolbarMenu();
            pauseOnMenu.style.flexShrink = 0;
            pauseOnMenu.text = "PauseOn...";
            pauseOnMenu.menu.AppendAction(nameof(CDebugConfig.PauseOnError),
            (act) => CDebugConfig.PauseOnError = !CDebugConfig.PauseOnError,
            (act =>
            {
                var isOn = CDebugConfig.PauseOnError;
                if (isOn) return DropdownMenuAction.Status.Checked;
                else return DropdownMenuAction.Status.Normal;
            }));
            pauseOnMenu.menu.AppendAction(nameof(CDebugConfig.PauseOnException),
            (act) => CDebugConfig.PauseOnException = !CDebugConfig.PauseOnException,
            (act =>
            {
                var isOn = CDebugConfig.PauseOnException;
                if (isOn) return DropdownMenuAction.Status.Checked;
                else return DropdownMenuAction.Status.Normal;
            }));

            return pauseOnMenu;
        }

        private ToolbarSearchField CreateSearchBar()
        {
            var searchBar = new ToolbarSearchField();
            searchBar.RegisterValueChangedCallback((evt) =>
            {
                ClassifiedConsoleWindow.windowRoot.editorLogFile.SearchFilter = evt.newValue;
                ClassifiedConsoleWindow.windowRoot.FilterTryNotify();
            });

            searchBar.style.marginRight = 5;
            searchBar.style.flexShrink = 1;

            return searchBar;
        }

        private ToolbarMenu archivedLogFileMenu;
        private ToolbarMenu CreateArchivedLogFileMenu()
        {
            this.archivedLogFileMenu = new ToolbarMenu();
            this.archivedLogFileMenu.style.flexShrink = 0;
            this.RefreshHistoryMenuItem();
            return this.archivedLogFileMenu;
        }

        public void RefreshHistoryMenuItem()
        {
            var editorLogFile = ClassifiedConsoleWindow.windowRoot.editorLogFile;
            var IsShowingCurrentLogFile = editorLogFile.IsShowingCurrentLogFile;
            if (IsShowingCurrentLogFile)
            {
                this.archivedLogFileMenu.text = $"Current_{editorLogFile.TargetLogFileID}";
            }
            else
            {
                this.archivedLogFileMenu.text = $"History_{editorLogFile.TargetLogFileID}";
            }
            this.ClearHistoryDeleteMenuItems();
            var logFileList = editorLogFile.ArchivedLogFileIdList;
            foreach (var logFileId in logFileList)
            {
                this.archivedLogFileMenu.menu.AppendAction(logFileId.ToString(),
                (act) =>
                {
                    ClassifiedConsoleWindow.windowRoot.TryLoadHistoryLogFile(logFileId);
                    this.RefreshHistoryMenuItem();
                },
                (act) =>
                {
                    var showingId = editorLogFile.TargetLogFileID;
                    return showingId == logFileId ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal;
                });
            }
            {
                this.archivedLogFileMenu.menu.AppendAction("Open Log Save Path",
                (act) =>
                {
                    var path = ClassifiedConsole.Runtime.LogFilePathConfig.versionRoot;
                    UnityEditor.EditorUtility.OpenWithDefaultApp(path);
                },
                (act) =>
                {
                    return DropdownMenuAction.Status.Normal;
                });
            }
        }

        private void ClearHistoryDeleteMenuItems()
        {
            var items = this.archivedLogFileMenu.menu.MenuItems();
            var itemsCount = items.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                this.archivedLogFileMenu.menu.RemoveItemAt(0);
            }
        }
    }
}