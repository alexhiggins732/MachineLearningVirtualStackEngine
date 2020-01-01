using System;

namespace ILEngine
{
    public class ActionExecutionResult
    {
        public Action Action;
        public Exception Exception;
        public bool Success;
        public bool TimedOut;
    }
}
