using System;
using System.Diagnostics.CodeAnalysis;

namespace ILEngine
{
    [ExcludeFromCodeCoverage]
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Log(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }
    }
}
