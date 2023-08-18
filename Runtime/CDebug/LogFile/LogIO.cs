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

        public void ReleaseIO()
        {
            this.streamWriter?.Close();
            this.writeBaseStream?.Close();
            this.streamReader?.Close();
            this.readBaseStream?.Close();
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

        public LogReader? WriteLog(LogWriter log)
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

        public string ReadLog(long startIndex, string endToken = LogReader.END)
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

        private char[] readBuffer = new char[1024];
        public string ReadLog(long startIndex, int length)
        {
            this.readerSB.Clear();
            this.streamReader.BaseStream.Position = startIndex;
            this.streamReader.DiscardBufferedData();
            // var readLoop = 0;
            // bool hasContent = false;



            for (int i = 0; i < length;)
            {
                int readedLength = 0;
                if (length - i >= 1024)
                {
                    readedLength = this.streamReader.Read(readBuffer, 0, 1024);
                }
                else
                {
                    readedLength = this.streamReader.Read(readBuffer, 0, length - i);
                }
                this.readerSB.Append(this.readBuffer, 0, readedLength);

                i += readedLength;
            }

            // while (true)
            // {
            //     var line = this.streamReader.ReadLine();
            //     if (line == endToken)
            //     {
            //         if (hasContent)
            //         {
            //             // 去掉最后一个换行
            //             this.readerSB.Remove(this.readerSB.Length - 1, 1);
            //         }
            //         break;
            //     }
            //     else
            //     {
            //         hasContent = true;
            //         this.readerSB.AppendLine(line);
            //     }

            //     if (readLoop++ > 1000)
            //     {
            //         break;
            //     }
            // }
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

        /* 另外一个尝试，并没有什么用 
        private StringBuilder readerSB = new StringBuilder();
        private char[] readerBuffer = new char[1024];

        public string ReadLog(long startIndex)
        {
            string endToken = LogReader.END;

            this.readerSB.Clear();
            // this.streamReader.DiscardBufferedData();
            // this.streamReader.BaseStream.Seek(startIndex, SeekOrigin.Begin);
            this.streamReader.BaseStream.Position = startIndex;
            this.streamReader.DiscardBufferedData();
            var readLoop = 0;

            bool isEnd = false;

            while (true)
            {
                var bufferLength = this.streamReader.ReadBlock(this.readerBuffer, 0, this.readerBuffer.Length);

                for (int i = 0; i < bufferLength; i++)
                {
                    var content = this.readerBuffer[i];
                    this.readerSB.Append(content);
                    if (content == '\n' || content == '\r')
                    {
                        var sbLength = this.readerSB.Length;

                        if (sbLength >= 5 &&
                            this.readerSB[sbLength - 2] == 'd' &&
                            this.readerSB[sbLength - 3] == 'n' &&
                            this.readerSB[sbLength - 4] == 'E' &&
                            this.readerSB[sbLength - 5] == '#')
                        {
                            isEnd = true;
                            break;
                        }
                    }

                }

                if (isEnd)
                {
                    // 去掉最后一个换行
                    this.readerSB.Remove(this.readerSB.Length - 5, 5);
                    break;
                }

                if (readLoop++ > 1000)
                {
                    break;
                }
            }
            return this.readerSB.ToString();
        }
        */
        #endregion
    }
}