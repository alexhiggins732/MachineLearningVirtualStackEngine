using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace ILEngine
{
    [ExcludeFromCodeCoverage]
    public class FileLogger : ILogger, IDisposable
    {
        FileStream stream;
        StreamWriter writer;
        public FileLogger(string filePath)
        {
            var fi = new FileInfo(filePath);
            fi.Directory.Create();
            stream = fi.Open(FileMode.OpenOrCreate);
            stream.Position = stream.Length;
            initWriter(stream);
        }
        public FileLogger(FileStream fileStream)
        {
            stream = fileStream;
            stream.Position = stream.Length;
            initWriter(stream);
        }
        void initWriter(Stream stream)
        {
            this.writer = new StreamWriter(stream);
        }
        public void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Log(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }

        public void Dispose()
        {
            if (this.writer != null)
            {
                writer.Close();
                writer.Dispose();
                writer = null;
            }
            if (this.stream != null)
            {
                this.stream.Close();
                this.stream.Dispose();
                this.stream = null;
            }
        }
    }
}
