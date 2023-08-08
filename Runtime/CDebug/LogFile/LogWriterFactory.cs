using System;
using System.Text;

namespace ClassifiedConsole.Runtime
{
    internal class LogWriterFactory
    {
        private static StringBuilder strackBuilder = new StringBuilder();
        public static LogWriter CreateLogWriter(LogWriter logWriter, LogLevel logLevel, int instanceId, params int[] subSystem)
        {
            logWriter.instanceId = instanceId;
            if (subSystem.Length == 0)
            {
                subSystem = new int[] { CDebugSubSystemEnumConfig.subSystemNullName };
            }
            logWriter.logSubSystem = subSystem;
            if (CDebugSettings.Instance.SplitLogFile)
            {
                logWriter.logFileName = CDebugSubSystemEnumConfig.GetSubSystemName(subSystem[0]);
            }
            else
            {
                var systemId = CDebugSubSystemEnumConfig.subSystemNullName;
                logWriter.logFileName = CDebugSubSystemEnumConfig.GetSubSystemName(systemId);
            }
            logWriter.level = logLevel;
            var nowTs = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0);
            logWriter.time = (long)(nowTs.TotalSeconds);

            // var writeLine = CDebugSettings.Instance.GetWriteLine(logLevel);
            // log.msg = BuildLogMsg(msg, writeLine, out int stackTrackStartIndex).ToString();
            // log.stackTrackStartIndex = msg.Length;
            return logWriter;
        }

        public static StringBuilder BuildLogMsg(string msg, int msgLineCount, int skipLine, out int stackTrackStartIndex)
        {
            strackBuilder.Clear();
            strackBuilder.AppendLine(msg);
            stackTrackStartIndex = strackBuilder.Length;
            // strackBuilder.AppendLine(UnityEngine.StackTraceUtility.ExtractStackTrace());
            // return strackBuilder;
            if (msgLineCount == 0)
            {
                return strackBuilder;
            }
            var stackSkinLine = skipLine;

            var needFileInfo = CDebugSettings.Instance.msgWithFileInfo;
            var stack = new System.Diagnostics.StackTrace(stackSkinLine, needFileInfo);

            var hasWriteAny = false;

            for (int i = 0; i < stack.FrameCount; i++)
            {
                if (msgLineCount == 0)
                {
                    break;
                }
                msgLineCount--;
                hasWriteAny = true;

                var stackFrame = stack.GetFrame(i);
                var methd = stackFrame.GetMethod();

                strackBuilder.Append(methd.DeclaringType.FullName);
                strackBuilder.Append(".");
                strackBuilder.Append(methd.Name);

                strackBuilder.Append('(');
                var needParamDetails = true;
                if (needParamDetails)
                {
                    var methodParam = methd.GetParameters();
                    if (methodParam.Length > 0)
                    {
                        foreach (var param in methodParam)
                        {
                            strackBuilder.Append(param.ParameterType);
                            strackBuilder.Append(",");
                        }
                        strackBuilder.Remove(strackBuilder.Length - 1, 1);
                    }
                }

                strackBuilder.Append(") ");

                if (needFileInfo)
                {
                    strackBuilder.Append("(at ");
                    var fileName = stackFrame.GetFileName();
                    fileName = TryRootToUnityDataPath(fileName);

                    strackBuilder.Append(fileName);
                    strackBuilder.Append(":");
                    // strackBuilder.Append(stackFrame.GetFileLineNumber().ToString());
                    // 解GC
                    {
                        var numberString = new NumberString(stackFrame.GetFileLineNumber());
                        for (int j = numberString.Length - 1; j >= 0; j--)
                        {
                            strackBuilder.Append(numberString[j]);
                        }
                    }
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

        public static void InitDataFullPath()
        {
            if (dataFullPath == null)
            {
                dataFullPath = System.IO.Path.GetFullPath(UnityEngine.Application.dataPath);
                dataFullPath = dataFullPath.Remove(dataFullPath.Length - "Assets".Length, "Assets".Length);
            }
        }

        private static string dataFullPath = null;
        private static string TryRootToUnityDataPath(string filePath)
        {
            InitDataFullPath();
            if (filePath != null && filePath.StartsWith(dataFullPath))
            {
                filePath = filePath.Replace(dataFullPath, "");
            }
            return filePath;
        }

        // public static LogWriter CreateLogWriterWithStack(string msg, LogLevel logLevel, string stackTrack, params int[] subSystem)
        // {
        //     var log = new LogWriter();
        //     log.uid = null;
        //     if (subSystem.Length == 0)
        //     {
        //         subSystem = new int[] { CDebugSubSystemEnumConfig.subSystemNullName };
        //     }
        //     log.logSubSystem = subSystem;
        //     if (CDebugSettings.Instance.SplitLogFile)
        //     {
        //         log.logFileName = CDebugSubSystemEnumConfig.GetSubSystemName(subSystem[0]);
        //     }
        //     else
        //     {
        //         var systemId = CDebugSubSystemEnumConfig.subSystemNullName;
        //         log.logFileName = CDebugSubSystemEnumConfig.GetSubSystemName(systemId);
        //     }
        //     log.level = logLevel;
        //     var nowTs = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0);
        //     log.time = (long)(nowTs.TotalSeconds);
        //     var writeLine = CDebugSettings.Instance.GetWriteLine(logLevel);
        //     log.msg = BuildLogMsgWithIn(msg, stackTrack, writeLine, out int stackTrackStartIndex).ToString();
        //     log.stackTrackStartIndex = stackTrackStartIndex;
        //     return log;
        // }

        public static StringBuilder BuildLogMsgWithIn(string msg, string stack, int msgLineCount, out int stackTrackStartIndex)
        {
            strackBuilder.Clear();
            strackBuilder.AppendLine(msg);
            stackTrackStartIndex = strackBuilder.Length;

            if (msgLineCount == -1)
            {
                strackBuilder.Append(stack);
                return strackBuilder;
            }

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