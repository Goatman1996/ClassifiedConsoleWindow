using System;

namespace ClassifiedConsole.Runtime
{
    public class ThreadTask
    {
        public Func<object> Task;
        public object result;
        public Action<ThreadTask> callBack;
    }
}
