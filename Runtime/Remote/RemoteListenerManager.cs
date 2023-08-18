using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClassifiedConsole.Runtime
{
    internal class RemoteListenerManager
    {
        private static RemoteListenerManager _Instance;
        public static RemoteListenerManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new RemoteListenerManager();
                }
                return _Instance;
            }
        }

        private RemoteListenerManager()
        {
            this.listenerDic = new Dictionary<int, HttpListener>();
            var url = GetLocalIPAddress();
            this.myIp = $"http://{url}";

            var go = new UnityEngine.GameObject();
            go.name = nameof(ForRemoteListenerOnGameExit);
            var exitEvt = go.AddComponent<ForRemoteListenerOnGameExit>();
            exitEvt.OnDestroyEvt += this.OnDestroy;
            UnityEngine.GameObject.DontDestroyOnLoad(go);
        }

        private void OnDestroy()
        {
            foreach (var kv in this.listenerDic)
            {
                var listener = kv.Value;
                listener.Stop();
            }
        }

        private string GetLocalIPAddress()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }

        public string myIp { get; private set; }
        private Dictionary<int, HttpListener> listenerDic;

        private bool IsListened(int port)
        {
            return this.listenerDic.ContainsKey(port);
        }

        public string StartListener(IRemoteListener remoteListener)
        {
            var port = remoteListener.port;
            var isListened = this.IsListened(port);
            if (isListened)
            {
                throw new System.Exception($"[HttpRemoteConnector] {port} is Listening already");
            }
            var url = $"{this.myIp}:{port}/";
            UnityEngine.Debug.Log($"[{remoteListener.GetType().FullName}] Listen URL {url}");
            var httpListener = new HttpListener();
            try
            {
                httpListener.Prefixes.Add(url);
                httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                httpListener.Start();
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return "";
            }

            this.listenerDic.Add(port, httpListener);
            this.Loop(httpListener, remoteListener);

            return url;
        }

        private async void Loop(HttpListener httpListener, IRemoteListener listener)
        {
            while (true)
            {
                HttpListenerRequest request = null;
                HttpListenerContext context = null;

                try
                {
                    context = await httpListener.GetContextAsync();
                    request = context.Request;
                }
                catch { return; }


                var response = context.Response;
                response.ContentEncoding = Encoding.UTF8;
                response.ContentType = "application/json; charset = utf-8";

                var responseMsg = "";
                if (request.HttpMethod == "GET")
                {
                    var getParam = this.ConvertGetParam(request);
                    if (getParam.Length == 1 && string.IsNullOrEmpty(getParam[0]))
                    {
                        responseMsg = listener.GetType().FullName;
                    }
                    else
                    {
                        responseMsg = listener.GetHandler(getParam);
                    }
                }
                else if (request.HttpMethod == "POST")
                {
                    var postParam = this.ConvertPostParam(request);
                    responseMsg = listener.PostHandler(postParam);
                }
                else { }

                byte[] msg = Encoding.UTF8.GetBytes(responseMsg);
                var opStream = response.OutputStream;
                opStream.Write(msg, 0, msg.Length);
                opStream.Flush();
                opStream.Close();
            }
        }

        private string[] ConvertGetParam(HttpListenerRequest request)
        {
            var rawUrl = request.RawUrl;
            // 删掉第一个 '/'
            rawUrl = rawUrl.Remove(0, 1);
            return rawUrl.Split('/');
        }

        private string ConvertPostParam(HttpListenerRequest request)
        {
            var inputStream = request.InputStream;
            var encoding = Encoding.UTF8;
            var streamReader = new StreamReader(inputStream, encoding);
            var requestMsg = streamReader.ReadToEnd();
            return requestMsg;
        }
    }
}