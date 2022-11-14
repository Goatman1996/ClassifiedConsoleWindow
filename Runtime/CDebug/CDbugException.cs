using System;

namespace ClassifiedConsole
{
    internal class ClassifiedException : Exception
    {
        public int subSystem;

        public ClassifiedException(int subSystem, string msg = "") : base($"{subSystem}:{msg}")
        {
            this.subSystem = subSystem;
        }
    }

    internal class CDbugExceptionUtil
    {
        public static string SplitCDebugException(string content, out bool isClassifiedException, out int subSystem)
        {
            isClassifiedException = false;
            subSystem = 0;

            var splited = content.Split(':');
            var typeName = splited[0];
            if (typeName == nameof(ClassifiedException))
            {
                isClassifiedException = true;
                subSystem = int.Parse(splited[1]);
                var msg = splited[2];
                return msg;
            }
            else
            {
                return content;
            }
        }
    }
}