using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ClassifiedConsole
{
    internal class CDebugAttributeHelper
    {
        private static void InitHelper()
        {
            var assemblyString = CDebugSettings.Instance.subSystemDefinedAssembly;
            var assemblyNameArray = assemblyString.Split(',');
            foreach (var assemblyName in assemblyNameArray)
            {
                Assembly assembly;
                try
                {
                    assembly = Assembly.Load(assemblyName);
                    if (assembly != null)
                    {
                        CollectByAssembly(assembly);
                    }
                }
                catch (FileNotFoundException)
                {
                    UnityEngine.Debug.LogError($"未找到Assembly {assemblyName} ");
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }

            var packageRuntimeAssembly = Assembly.Load("GM.ClassifiedConsole.Runtime");
            CollectByAssembly(packageRuntimeAssembly);
        }

        private static void CollectByAssembly(Assembly assembly)
        {

            var allType = assembly.GetTypes();
            foreach (var type in allType)
            {
                if (!type.IsClass)
                {
                    continue;
                }
                foreach (var attr in type.GetCustomAttributes(false))
                {
                    if (attr is CDebugExceptionAttribute)
                    {
                        CollectExceptionType(type);
                    }
                }
            }
        }

        private static void CollectExceptionType(Type exceptionType)
        {
            _exceptionType.Add(exceptionType.Name);
        }

        private static List<string> _exceptionType;
        private static List<string> managedExceptionType
        {
            get
            {
                if (_exceptionType == null)
                {
                    _exceptionType = new List<string>();
                    InitHelper();
                }
                return _exceptionType;
            }
        }

        public static bool IsClassifiedException(string msg, out int subSystem, out string realMsg)
        {
            realMsg = msg;
            subSystem = (int)CDebugBosster.UnityNativeSubSystem.Unity_Native_Log;
            var exceptionList = managedExceptionType;
            foreach (var exceptionName in exceptionList)
            {
                if (msg.StartsWith(exceptionName))
                {
                    var spliter = msg.Split(':');
                    if (spliter.Length >= 2 && int.TryParse(spliter[1], out int targetSubSystem))
                    {
                        subSystem = targetSubSystem;
                        realMsg = msg.Replace(spliter[1], "");
                        return true;
                    }
                }
            }

            return false;
        }
    }
}