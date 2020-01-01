using System.Diagnostics.CodeAnalysis;

namespace ILEngine
{
    [ExcludeFromCodeCoverage]
    public class DebugLogger : ILogger
    {
        public void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Log(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }
    }
}
