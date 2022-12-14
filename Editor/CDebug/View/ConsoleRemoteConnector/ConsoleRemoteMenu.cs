using System;
using ClassifiedConsole.Runtime;
using HttpRemoteConnector;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ClassifiedConsole.Editor
{
    internal class ConsoleRemoteMenu : ToolbarMenu
    {
        public RemoteRequestor remoteRequestor { get => ClassifiedConsoleWindow.windowRoot.remoteRequestor; }

        public ConsoleRemoteMenu()
        {
            this.style.flexShrink = 0;
            this.RefreshMenu();
            this.remoteRequestor.OnStateChanged += this.OnStateChanged;
            this.OnStateChanged(this.remoteRequestor.state);
        }

        private void OnStateChanged(RemoteRequestor.ConnectionState state)
        {
            switch (state)
            {
                case RemoteRequestor.ConnectionState.NoConnection:
                case RemoteRequestor.ConnectionState.Connecting:
                    this.text = "Editor";
                    break;
                case RemoteRequestor.ConnectionState.ConnectedToRemote:
                    var ip = this.remoteRequestor.remoteUrl;
                    ip = ip.Replace("http://", "");
                    this.text = ip;
                    break;
            }
        }

        private void RefreshMenu()
        {
            var itemsCount = this.menu.MenuItems().Count;
            for (int i = 0; i < itemsCount; i++)
            {
                this.menu.RemoveItemAt(0);
            }
            this.menu.AppendAction("Editor",
            (act) =>
            {
                remoteRequestor.state = RemoteRequestor.ConnectionState.NoConnection;
            },
            (act) =>
            {
                if (remoteRequestor.state == RemoteRequestor.ConnectionState.ConnectedToRemote)
                {
                    return DropdownMenuAction.Status.Normal;
                }
                else
                {
                    return DropdownMenuAction.Status.Checked;
                }
            });

            var ipPrefs = CDebugConfig.IpPrefs;
            if (!string.IsNullOrEmpty(ipPrefs))
            {
                this.menu.AppendAction(ipPrefs,
                async (act) =>
                {
                    if (this.remoteRequestor.state == RemoteRequestor.ConnectionState.NoConnection)
                    {
                        await this.remoteRequestor.TryConnect(ipPrefs, typeof(LogFileRemoteListener));
                    }
                },
                (act) =>
                {
                    var remoteUrl = remoteRequestor.remoteUrl;
                    if (string.IsNullOrEmpty(remoteUrl))
                    {
                        return DropdownMenuAction.Status.Normal;
                    }

                    var state = this.remoteRequestor.state;
                    var isConnected = state == RemoteRequestor.ConnectionState.ConnectedToRemote;
                    if (remoteRequestor.remoteUrl.Contains(ipPrefs) && isConnected)
                    {
                        return DropdownMenuAction.Status.Checked;
                    }
                    else
                    {
                        return DropdownMenuAction.Status.Normal;
                    }
                });
            }

            this.menu.AppendSeparator();
            this.menu.AppendAction("Enter Ip",
            (act) =>
            {
                ConsoleRemoteIpInputer.Show(this.contentRect, this.OnIpInputFinish);
            });
        }

        private async void OnIpInputFinish(string ip)
        {
            if (!ip.Contains(":"))
            {
                ip = ip + ":" + CDebugSettings.Instance.port;
            }
            var success = await this.remoteRequestor.TryConnect(ip, typeof(LogFileRemoteListener));
            if (success)
            {
                CDebugConfig.IpPrefs = ip;
                this.RefreshMenu();
            }
            else
            {
                UnityEngine.Debug.LogError($"Can Not Touch {ip}");
            }
        }


    }
}