using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ClassifiedConsole.Runtime
{
    internal class Cmd
    {
        private string _cmdContent;
        public string cmdContent
        {
            get => this._cmdContent;
            set
            {
                this._cmdContent = value;
                this._cmdContent = this._cmdContent.Replace("\n", "");
                this._cmdContent = this._cmdContent.Replace("\r", "");
            }
        }

        private Assembly asm;
        public Cmd()
        {
            var assmblyName = CDebugSettings.Instance.SubSystemDefinedAssembly[0];
            try
            {
                this.asm = Assembly.Load(assmblyName);
            }
            catch (FileNotFoundException)
            {
                UnityEngine.Debug.LogWarning($"未找到Assembly {assmblyName} ");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        public bool cmdValid
        {
            get => this.CmdMethod != null;
        }

        private MethodInfo CmdMethod
        {
            get
            {
                string content = this.cmdContent;

                var spaceIndex = this.cmdContent.IndexOf(' ');
                if (spaceIndex != -1)
                {
                    content = content.Substring(0, spaceIndex);
                }

                var dotSplit = content.Split('.');
                if (dotSplit.Length <= 1) return null;

                var lastDot = content.LastIndexOf('.');
                var typeString = content.Substring(0, lastDot);
                var methodString = content.Substring(lastDot + 1);

                if (string.IsNullOrEmpty(methodString)) return null;

                if (this.asm == null) return null;

                var type = this.asm.GetType(typeString);
                if (type == null) return null;

                var method = type.GetMethod(methodString, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (method == null) return null;

                return method;
            }
        }

        private string[] paramArray
        {
            get
            {
                string content = this.cmdContent;

                var spaceIndex = this.cmdContent.IndexOf(' ');
                if (spaceIndex != -1)
                {
                    content = content.Substring(spaceIndex + 1);
                    return content.Split(' ');
                }
                else
                {
                    return new string[0];
                }
            }
        }

        public bool ExecuteCmd()
        {
            if (this.cmdValid == false)
            {
                var logType = ClassifiedConsole.CDebugBosster.UnityNativeSubSystem.Unity_Native_Log;
                ClassifiedConsole.CDebug.LogError($"Not Found Cmd {cmdContent}", (int)logType);
                return false;
            }
            try
            {
                var parameters = this.CmdMethod.GetParameters();
                var paramLength = parameters.Length;
                if (paramArray.Length != paramLength)
                {
                    throw new Exception($"Parameters not match to {this.cmdContent}");
                }
                var inParam = new object[paramArray.Length];
                for (int i = 0; i < paramLength; i++)
                {
                    var paramInfo = parameters[i];
                    var paramContent = this.paramArray[i];
                    var p = this.Converter(paramInfo.ParameterType, paramContent);
                    inParam[i] = p;
                }
                this.CmdMethod.Invoke(null, inParam);
            }
            catch (Exception e)
            {
                var logType = ClassifiedConsole.CDebugBosster.UnityNativeSubSystem.Unity_Native_Log;
                ClassifiedConsole.CDebug.LogError($"Execute Cmd {cmdContent} With Exception:\n{e}", (int)logType);
                return false;
            }
            return true;
        }

        private object Converter(Type type, string content)
        {
            return System.Convert.ChangeType(content, type);
        }
    }
}