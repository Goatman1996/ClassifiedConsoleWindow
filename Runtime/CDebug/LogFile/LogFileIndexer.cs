using System.Collections.Generic;
using System.IO;
using ClassifiedConsole.Runtime;

namespace ClassifiedConsole
{
    internal class LogFileIndexer
    {
        private string indexerFileName;

        public LogFileIndexer(string indexerName)
        {
            this.indexerFileName = indexerName;
        }

        private bool hasInit = false;
        private void Initialize()
        {
            this.hasInit = true;
            if (!File.Exists(indexerFileName))
            {
                var createStream = File.Create(indexerFileName);
                createStream.Close();
                createStream.Dispose();
            }

            this.managedReader = new List<LogReader>();
            var allLine = File.ReadLines(this.indexerFileName, System.Text.Encoding.UTF8);
            foreach (var line in allLine)
            {
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
            var line = logReader.ToString();
            this.lineAppender[0] = line;
            File.AppendAllLines(this.indexerFileName, this.lineAppender);
            if (this.hasInit)
            {
                this.managedReader.Add(logReader);
            }
        }
    }
}