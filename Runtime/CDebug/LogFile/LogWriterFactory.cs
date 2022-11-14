using System;
using System.Text;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole
{
    internal class LogWriterFactory
    {
        private static StringBuilder strackBuilder = new StringBuilder();
        public static LogWriter CreateLogWriter(string msg, LogLevel logLevel, string uid, int instanceId, params int[] subSystem)
        {
            var log = new LogWriter();
            log.uid = uid;
            log.instanceId = instanceId;
            if (subSystem.Length == 0)
            {
                subSystem = new int[] { CDebugSubSystemEnumConfig.subSystemNullName };
            }
            log.logSubSystem = subSystem;
            if (CDebugSettings.Instance.SplitLogFile)
            {
                log.logFileName = CDebugSubSystemEnumConfig.GetSubSystemName(subSystem[0]);
            }
            else
            {
                var systemId = CDebugSubSystemEnumConfig.subSystemNullName;
                log.logFileName = CDebugSubSystemEnumConfig.GetSubSystemName(systemId);
            }
            log.level = logLevel;
            var nowTs = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0);
            log.time = (long)(nowTs.TotalSeconds);
            var writeLine = CDebugSettings.Instance.GetWriteLine(logLevel);
            log.msg = BuildLogMsg(msg, writeLine, out int stackTrackStartIndex).ToString();
            log.stackTrackStartIndex = stackTrackStartIndex;
            return log;
        }

        private static StringBuilder BuildLogMsg(string msg, int msgLineCount, out int stackTrackStartIndex)
        {
            strackBuilder.Clear();
            strackBuilder.AppendLine(msg);
            stackTrackStartIndex = strackBuilder.Length;

            var needFileInfo = CDebugSettings.Instance.msgWithFileInfo;
            var stack = new System.Diagnostics.StackTrace(needFileInfo);

            var hasWriteAny = false;

            var stackSkinLine = CDebugSettings.Instance.stackSkipLine;
            for (int i = stackSkinLine; i < stack.FrameCount; i++)
            {
                if (msgLineCount == 0)
                {
                    break;
                }
                msgLineCount--;
                hasWriteAny = true;

                var stackFrame = stack.GetFrame(i);
                var methd = stackFrame.GetMethod();

                strackBuilder.Append($"{methd.DeclaringType.Name}.");
                strackBuilder.Append($"{methd.Name}");

                strackBuilder.Append('(');
                var methodParam = methd.GetParameters();
                if (methodParam.Length > 0)
                {
                    foreach (var param in methodParam)
                    {
                        strackBuilder.Append($"{param.ParameterType},");
                    }
                    strackBuilder.Remove(strackBuilder.Length - 1, 1);
                }
                strackBuilder.Append(") ");

                if (needFileInfo)
                {
                    strackBuilder.Append("(at ");
                    var fileName = stackFrame.GetFileName();
                    fileName = TryRootToUnityDataPath(fileName);

                    strackBuilder.Append($"{fileName}:");
                    strackBuilder.Append($"{stackFrame.GetFileLineNumber()}");
                    strackBuilder.Append(")");
                }
                strackBuilder.AppendLine("");
            }

            if (hasWriteAny)
            {
                // 删除最后一个换行
                strackBuilder.Remove(strackBuilder.Length - 1, 1);
            }
            return strackBuilder;
        }

        private static string TryRootToUnityDataPath(string filePath)
        {
            if (filePath != null)
            {
                var assetPath = System.IO.Path.GetFullPath(UnityEngine.Application.dataPath);
                assetPath = assetPath.Remove(assetPath.Length - "Assets".Length, "Assets".Length);
                filePath = filePath.Replace(assetPath, "");
            }
            return filePath;
        }

        public static LogWriter CreateLogWriterWithStack(string msg, LogLevel logLevel, string stackTrack, params int[] subSystem)
        {
            var log = new LogWriter();
            log.uid = null;
            if (subSystem.Length == 0)
            {
                subSystem = new int[] { CDebugSubSystemEnumConfig.subSystemNullName };
            }
            log.logSubSystem = subSystem;
            if (CDebugSettings.Instance.SplitLogFile)
            {
                log.logFileName = CDebugSubSystemEnumConfig.GetSubSystemName(subSystem[0]);
            }
            else
            {
                var systemId = CDebugSubSystemEnumConfig.subSystemNullName;
                log.logFileName = CDebugSubSystemEnumConfig.GetSubSystemName(systemId);
            }
            log.level = logLevel;
            var nowTs = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0);
            log.time = (long)(nowTs.TotalSeconds);
            var writeLine = CDebugSettings.Instance.GetWriteLine(logLevel);
            if (writeLine != -1)
            {
                // 从Unity Native 里来的Log，如果不是不限制栈行数的话，就多加一行，才能看到第一个栈信息
                writeLine++;
            }
            log.msg = BuildLogMsgWithIn(msg, stackTrack, writeLine, out int stackTrackStartIndex).ToString();
            log.stackTrackStartIndex = stackTrackStartIndex;
            return log;
        }

        private static StringBuilder BuildLogMsgWithIn(string msg, string stack, int msgLineCount, out int stackTrackStartIndex)
        {
            strackBuilder.Clear();
            strackBuilder.AppendLine(msg);
            stackTrackStartIndex = strackBuilder.Length;

            var lines = stack.Split('\n');

            var hasWriteAny = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (msgLineCount == 0)
                {
                    break;
                }
                msgLineCount--;
                hasWriteAny = true;

                strackBuilder.AppendLine(lines[i]);
            }

            if (hasWriteAny)
            {
                // 删除最后一个换行
                strackBuilder.Remove(strackBuilder.Length - 1, 1);
            }
            return strackBuilder;
        }
    }
}