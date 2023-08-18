namespace ClassifiedConsole.Runtime
{
    internal interface IRemoteListener
    {
        public int port { get; }
        /// <summary>
        /// paramArray 起码会有一个
        /// </summary>
        public string GetHandler(string[] paramArray);
        public string PostHandler(string param);
    }
}