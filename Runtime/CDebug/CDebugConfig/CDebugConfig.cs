using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClassifiedConsole.Runtime
{
    internal class CDebugConfig
    {
        public const string version = "1.0.2";

        public static bool ArchiveOnPlay
        {
            get
            {
                var value = PlayerPrefs.GetInt("CDebugConfig.ArchiveOnPlay", 1);
                return value == 1;
            }
            set
            {
                var intValue = value ? 1 : 0;
                PlayerPrefs.SetInt("CDebugConfig.ArchiveOnPlay", intValue);
            }
        }
    }
}