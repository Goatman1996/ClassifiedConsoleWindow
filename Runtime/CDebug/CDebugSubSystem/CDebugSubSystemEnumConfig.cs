using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;

namespace ClassifiedConsole.Runtime
{
    public class CDebugSubSystemEnumConfig
    {
        public readonly static int subSystemNullName = (int)UnDefinedSubSystem.Not_Classified;
        private static Dictionary<int, string> _subSystemEnumLabelDic;
        private static Dictionary<int, string> _subSystemEnumDic;
        private static Dictionary<int, string> subSystemEnumDic
        {
            get
            {
                if (_subSystemEnumDic == null)
                {
                    _subSystemEnumLabelDic = new Dictionary<int, string>();
                    _subSystemEnumDic = new Dictionary<int, string>();
                    InitSubSystemEnum();
                }
                return _subSystemEnumDic;
            }
        }

        private static void InitSubSystemEnum()
        {
            // var packageRuntimeAssembly = Assembly.Load("GM.ClassifiedConsole.Runtime");
            // CollectByAssembly(packageRuntimeAssembly);

            CollectEnumType(typeof(CDebugBosster.UnityNativeSubSystem));

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

            CollectEnumType(typeof(UnDefinedSubSystem));
        }

        private static void CollectByAssembly(Assembly assembly)
        {
            var allType = assembly.GetTypes();
            foreach (var type in allType)
            {
                if (!type.IsEnum)
                {
                    continue;
                }
                foreach (var attr in type.GetCustomAttributes(false))
                {
                    if (attr is CDebugSubSystemAttribute)
                    {
                        CollectEnumType(type);
                    }
                }
            }
        }

        private static void CollectEnumType(Type enumType)
        {
            foreach (var key in enumType.GetEnumValues())
            {
                var value = enumType.GetEnumName(key);
                _subSystemEnumDic[(int)key] = value;

                var field = key.GetType().GetField(value);
                var attr = field.GetCustomAttributes(typeof(CDebugSubSystemLabelAttribute), false);
                if (attr.Length > 0)
                {
                    var label = (CDebugSubSystemLabelAttribute)attr[0];
                    _subSystemEnumLabelDic.Add((int)key, label.label);
                }
                else
                {
                    _subSystemEnumLabelDic.Add((int)key, value);
                }
            }
        }

        public static string GetSubSystemLabel(int key)
        {
            if (remote_SubSystemEnumLabelDic != null)
            {
                if (!remote_SubSystemEnumLabelDic.ContainsKey(key))
                {
                    remote_SubSystemEnumLabelDic[key] = key.ToString();
                }
                return remote_SubSystemEnumLabelDic[key];
            }
            else
            {
                if (!_subSystemEnumLabelDic.ContainsKey(key))
                {
                    _subSystemEnumLabelDic[key] = key.ToString();
                }
                return _subSystemEnumLabelDic[key];
            }
        }

        public static Dictionary<int, string> remote_SubSystemEnumDic;
        public static Dictionary<int, string> remote_SubSystemEnumLabelDic;

        public static string GetSubSystemName(int key)
        {
            if (remote_SubSystemEnumDic != null)
            {
                if (!remote_SubSystemEnumDic.ContainsKey(key))
                {
                    remote_SubSystemEnumDic[key] = key.ToString();
                }
                return remote_SubSystemEnumDic[key];
            }
            else
            {
                if (!subSystemEnumDic.ContainsKey(key))
                {
                    subSystemEnumDic[key] = key.ToString();
                }
                return subSystemEnumDic[key];
            }

        }

        public static IEnumerable<int> GetAllSubSystemList()
        {
            if (remote_SubSystemEnumDic != null)
            {
                return remote_SubSystemEnumDic.Keys;
            }
            return subSystemEnumDic.Keys;
        }


        #region Prefs
        private static Dictionary<int, bool> _prefsDic;
        public static Dictionary<int, bool> prefsDic
        {
            get
            {
                if (_prefsDic == null)
                {
                    _prefsDic = new Dictionary<int, bool>();
                }
                return _prefsDic;
            }
        }
        public static bool GetSubSystemPrefs(int key)
        {
            if (!prefsDic.ContainsKey(key))
            {
                var intValue = PlayerPrefs.GetInt(key.ToString(), 1);
                prefsDic[key] = intValue == 1;
            }
            return prefsDic[key];
        }

        public static void SetSubSystemPrefs(int key, bool value)
        {
            var intValue = value ? 1 : 0;
            PlayerPrefs.SetInt(key.ToString(), intValue);
            prefsDic[key] = value;
            SubSystemSettingsVersion++;
        }

        public static void SetAllState(bool isOn)
        {
            foreach (var key in subSystemEnumDic.Keys)
            {
                SetSubSystemPrefs(key, isOn);
            }
        }
        #endregion

        public static bool IsSubSystemOn(int key)
        {
            return GetSubSystemPrefs(key);
        }

        public static bool IsSubSystemOn(params int[] keyS)
        {
            foreach (var key in keyS)
            {
                var isOn = IsSubSystemOn(key);
                if (isOn)
                {
                    return true;
                }
            }
            return false;
        }

        public static int SubSystemSettingsVersion = 0;
    }

    [ClassifiedConsole.CDebugSubSystem]
    internal enum UnDefinedSubSystem
    {
        Not_Classified = int.MinValue,
    }
}