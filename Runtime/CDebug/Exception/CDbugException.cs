using System;

namespace ClassifiedConsole
{
    [CDebugException]
    internal class ClassifiedException : Exception
    {
        public int subSystem;
        public ClassifiedException(int subSystem, string msg = "") : base($"{subSystem}:{msg}")
        {
            this.subSystem = subSystem;
        }
    }
}