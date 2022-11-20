using System;

namespace ClassifiedConsole
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class CDebugSubSystemAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Field)]
    public class CDebugSubSystemLabelAttribute : Attribute
    {
        public string label;

        public CDebugSubSystemLabelAttribute(string label)
        {
            this.label = label;
        }
    }
}