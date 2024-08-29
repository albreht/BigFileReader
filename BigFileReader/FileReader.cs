using System.Collections.Concurrent;
using System.IO;
using System.Text;
using BigFileReader.Model;

namespace BigFileReader
{
    internal class FileReader
    {
      
        private FileSystemWatcher _fileSystemWatcher;
        private string _fileName;
        private long _chunkSize;
        private long _fileSize;
        public FileReader(string fileLocation)
        {
            _fileName = fileLocation;
            _fileSize = new FileInfo(fileLocation).Length;

        }
        public void SetChunkSize(long chunkSize) { 
        _chunkSize = chunkSize;
        }
        private void _fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            var d = e;
        }

        public event Action<List<long>> NewLineBreak;

        public long FileLength { get => _fileSize; }

        public async Task<string> FileContentsAsync(LineBreak startPosi, int lineCount)
        {
            using var fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);


            fs.Position = startPosi.LineNumber;

            if (startPosi.IsEstimated)
                await sr.ReadLineAsync();



            var stringBuilder = new StringBuilder();
            for (int i = 0; i < lineCount; i++)
            {
                stringBuilder.AppendLine(await sr.ReadLineAsync());
            }


            return stringBuilder.ToString();
        }



        public async Task<List<SearchResult>> SearchWord(string word)
        {
            int threadCount = Environment.ProcessorCount;

            var lineBreakPositions = new ConcurrentBag<SearchResult>();

            var chunkLen = _fileSize / threadCount;
            var tasks = new List<Task>();

            tasks.AddRange(
            Enumerable.Range(0, threadCount)
                .Select(i =>
            Task.Run(() =>
            {
                var endPosition = (i + 1) * chunkLen;

                using var fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var sr = new StreamReader(fs);

                var position = i * chunkLen;
                fs.Position = position;

                string line;

                if (position != 0)
                {
                    sr.ReadLine();
                }

                while (true)
                {
                    position = sr.GetPosition();
        
                    line = sr.ReadLine();
                    if (line == null)
                        break;



                    if (line.Contains(word))
                    {
                        lineBreakPositions.Add(new SearchResult() { LineNumber = position, Text = line });
                    }

                    if (position > endPosition)
                    {
                        break;
                    }
                }

            })));

            await Task.WhenAll(tasks.ToArray());
            return lineBreakPositions.OrderBy(p => p.LineNumber).ToList();
        }


    }
}