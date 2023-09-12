using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ClassifiedConsole.Runtime
{
    internal class RemoteRequestor
    {
        public enum ConnectionState
        {
            NoConnection,
            Connecting,
            ConnectedToRemote
        }

        private string _remoteUrl;
        public string remoteUrl
        {
            get => this._remoteUrl;
            private set => this._remoteUrl = value;
        }
        private ConnectionState _state = ConnectionState.NoConnection;
        public ConnectionState state
        {
            get => this._state;
            set
            {
                if (this._state == value)
                {
                    return;
                }
                this._state = value;
                if (this._state == ConnectionState.NoConnection)
                {
                    this.remoteUrl = "";
                }
                this.OnStateChanged?.Invoke(this._state);
            }
        }
        public event Action<ConnectionState> OnStateChanged;

        /// <summary>
        /// http://xxx.xxx.xxx.xxx:xxx
        /// </summary>
        public async Task<bool> TryConnect(string url, Type targetRemoteListenerType)
        {
            if (!url.StartsWith("http://"))
            {
                url = "http://" + url;
            }
            this.remoteUrl = url;
            if (state == ConnectionState.Connecting)
            {
                throw new System.Exception($"[RemoteRequestor] ConnectionState.Connecting");
            }
            else if (state == ConnectionState.ConnectedToRemote)
            {
                throw new System.Exception($"[RemoteRequestor] ConnectionState.ConnectedToRemote");
            }

            UnityEngine.Debug.Log($"TryConnect {this.remoteUrl}");
            this.state = ConnectionState.Connecting;
            var remoteType = await this.GetAsync(false);
            var success = remoteType == targetRemoteListenerType.FullName;
            if (success)
            {
                UnityEngine.Debug.Log($"[RemoteRequestor] Connect To {this.remoteUrl} Successfully");
                this.state = ConnectionState.ConnectedToRemote;
            }
            else
            {
                UnityEngine.Debug.LogError($"[RemoteRequestor] Failed To Connect {this.remoteUrl}");
                this.state = ConnectionState.NoConnection;
            }
            return success;
        }

        /// <summary>
        /// 参数内不能包涵 '/'
        /// </summary>
        public Task<string> GetAsync(bool throwWhenError, params string[] paramArray)
        {
            if (paramArray.Length != 0)
            {
                if (this.state != ConnectionState.ConnectedToRemote)
                {
                    throw new Exception($"[RemoteRequestor] No Connection.");
                }
            }

            var tcs = new TaskCompletionSource<string>();

            var param = this.ConcatParam(paramArray);
            var url = $"{this.remoteUrl}/{param}";
            var getRequest = UnityWebRequest.Get(url);
            getRequest.SetRequestHeader("Content-Type", "application/json");
            getRequest.timeout = 10;
            getRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            var operation = getRequest.SendWebRequest();
            operation.completed += (a) =>
            {
                if (getRequest.responseCode != 200)
                {
                    if (throwWhenError)
                    {
                        var e = new Exception($"[RemoteRequestor] Get Error Code : {getRequest.responseCode}");
                        tcs.SetException(e);
                        this.state = ConnectionState.NoConnection;
                        return;
                    }
                    else
                    {
                        var e = new Exception($"[RemoteRequestor] Get Error Code : {getRequest.responseCode}");
                        UnityEngine.Debug.LogException(e);
                        tcs.SetResult($"Error Code : {getRequest.responseCode}");
                        this.state = ConnectionState.NoConnection;
                        return;
                    }
                }
                var response = getRequest.downloadHandler.text;
                tcs.SetResult(response);
            };

            return tcs.Task;
        }

        private string ConcatParam(params string[] paramArray)
        {
            var ret = "";
            foreach (var p in paramArray)
            {
                if (p.Contains("/"))
                {
                    throw new System.Exception($"[RemoteRequestor] Get Param Can not Contains '/'");
                }
                ret += p;
            }
            return ret;
        }

        public Task<string> PostAsync(string param, bool throwWhenError)
        {
            if (this.state != ConnectionState.ConnectedToRemote)
            {
                throw new Exception($"[RemoteRequestor] No Connection.");
            }

            var tcs = new TaskCompletionSource<string>();

            var url = $"{this.remoteUrl}";
            var postRequest = new UnityWebRequest(url, "POST");
            var bytes = Encoding.UTF8.GetBytes(param);
            postRequest.uploadHandler = new UploadHandlerRaw(bytes);
            postRequest.SetRequestHeader("Content-Type", "application/json");
            postRequest.timeout = 10;
            postRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            var operation = postRequest.SendWebRequest();
            operation.completed += (a) =>
            {
                if (postRequest.responseCode != 200)
                {
                    if (throwWhenError)
                    {
                        var e = new Exception($"[RemoteRequestor] Post Error Code : {postRequest.responseCode}");
                        tcs.SetException(e);
                        this.state = ConnectionState.NoConnection;
                        return;
                    }
                    else
                    {
                        var e = new Exception($"[RemoteRequestor] Post Error Code : {postRequest.responseCode}");
                        UnityEngine.Debug.LogException(e);
                        tcs.SetResult($"Error Code : {postRequest.responseCode}");
                        this.state = ConnectionState.NoConnection;
                        return;
                    }

                }
                var downLoad = postRequest.downloadHandler;
                var response = downLoad.text;
                tcs.SetResult(response);
            };

            return tcs.Task;
        }
    }
}