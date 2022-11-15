using UnityEngine;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedConsole.Runtime
{
    public class LogIO
    {
        private FileStream writeBaseStream;
        private FileStream readBaseStream;

        public string logFileName { get; private set; }

        public LogIO(string logFilePath)
        {
            this.logFileName = logFilePath;
            string logDirPath = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logDirPath))
            {
                Directory.CreateDirectory(logDirPath);
            }
            this.writeBaseStream = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            this.readBaseStream = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Write);
        }


        #region Writer
        private StreamWriter _streamWriter;
        private StreamWriter streamWriter
        {
            get
            {
                if (this._streamWriter == null)
                {
                    this._streamWriter = new StreamWriter(this.writeBaseStream, Encoding.UTF8);
                }
                return this._streamWriter;
            }
        }

        public LogReader WriteLog(LogWriter log)
        {
            return log.Write(this.streamWriter);
        }
        #endregion

        #region Reader
        private StreamReader _streamReader;
        private StreamReader streamReader
        {
            get
            {
                if (this._streamReader == null)
                {
                    this._streamReader = new StreamReader(this.readBaseStream, Encoding.UTF8);
                }
                return this._streamReader;
            }
        }

        private StringBuilder readerSB = new StringBuilder();

        public string ReadLog(long startIndex, string endToken)
        {
            this.readerSB.Clear();
            // this.streamReader.DiscardBufferedData();
            // this.streamReader.BaseStream.Seek(startIndex, SeekOrigin.Begin);
            this.streamReader.BaseStream.Position = startIndex;
            this.streamReader.DiscardBufferedData();
            var readLoop = 0;
            bool hasContent = false;
            while (true)
            {
                var line = this.streamReader.ReadLine();
                if (line == endToken)
                {
                    if (hasContent)
                    {
                        // 去掉最后一个换行
                        this.readerSB.Remove(this.readerSB.Length - 1, 1);
                    }
                    break;
                }
                else
                {
                    hasContent = true;
                    this.readerSB.AppendLine(line);
                }

                if (readLoop++ > 1000)
                {
                    break;
                }
            }
            return this.readerSB.ToString();
        }

        public string ReadLines(long startIndex, int lineCount)
        {
            this.readerSB.Clear();
            // this.streamReader.DiscardBufferedData();
            // this.streamReader.BaseStream.Seek(startIndex, SeekOrigin.Begin);
            this.streamReader.BaseStream.Position = startIndex;
            this.streamReader.DiscardBufferedData();
            var readLoop = 1;
            bool hasContent = false;
            while (true)
            {
                var line = this.streamReader.ReadLine();

                if (readLoop++ > lineCount)
                {
                    if (hasContent)
                    {
                        // 去掉最后一个换行
                        this.readerSB.Remove(this.readerSB.Length - 1, 1);
                    }
                    break;
                }
                else
                {
                    hasContent = true;
                    this.readerSB.AppendLine(line);
                }
            }
            return this.readerSB.ToString();
        }
        #endregion
    }
}