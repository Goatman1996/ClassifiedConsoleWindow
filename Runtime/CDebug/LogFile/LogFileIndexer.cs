using System.Collections.Generic;
using System.IO;
using System.Text;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole
{
    internal class LogFileIndexer
    {
        private FileStream indexerFileStream;
        private StreamWriter indexerWriter;
        private StreamReader indexerReader;

        private string indexerFileName;

        public LogFileIndexer(string indexerName)
        {
            this.indexerFileName = indexerName;

            this.indexerFileStream = new FileStream(indexerFileName, FileMode.OpenOrCreate);

            this.indexerWriter = new StreamWriter(this.indexerFileStream, Encoding.UTF8);
            this.indexerReader = new StreamReader(this.indexerFileStream, Encoding.UTF8);
        }

        private bool hasInit = false;
        private void Initialize()
        {
            this.hasInit = true;

            this.indexerReader.BaseStream.Position = 0;
            this.indexerReader.DiscardBufferedData();


            this.managedReader = new List<LogReader>();
            // var allLine = File.ReadLines(this.indexerFileName, System.Text.Encoding.UTF8);
            while (true)
            {
                var line = this.indexerReader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }
                var reader = LogReader.Parse(line);
                this.managedReader.Add(reader);
            }
        }

        private List<LogReader> managedReader;

        public int logCount
        {
            get
            {
                if (this.hasInit == false)
                {
                    this.Initialize();
                }
                return this.managedReader.Count;
            }
        }

        public LogReader this[int index]
        {
            get
            {
                if (this.hasInit == false)
                {
                    this.Initialize();
                }
                return this.managedReader[index];
            }
        }


        private string[] lineAppender = new string[1];
        public void AppendReader(LogReader logReader)
        {
            this.indexerWriter.BaseStream.Position = this.indexerWriter.BaseStream.Length;

            logReader.Write(this.indexerWriter);
            this.indexerWriter.Flush();
            if (this.hasInit)
            {
                this.managedReader.Add(logReader);
            }
        }
    }
}