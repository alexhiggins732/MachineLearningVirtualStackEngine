using System.Diagnostics.CodeAnalysis;

namespace ILEngine
{
    [ExcludeFromCodeCoverage]
    public class NullLogger : ILogger
    {
        public void Log(string message)
        {
        }

        public void Log(string format, params object[] args)
        {
        }
    }
}
