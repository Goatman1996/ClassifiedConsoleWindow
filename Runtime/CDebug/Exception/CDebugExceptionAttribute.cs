using System;

namespace ClassifiedConsole
{
    /// <summary>
    /// 需要如下形式的存储 Exception 信息 base($"{subSystemId}:{msg}")
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CDebugExceptionAttribute : Attribute
    {

    }
}