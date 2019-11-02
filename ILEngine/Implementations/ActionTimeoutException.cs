using System;

namespace ILEngine.Implementations
{
    public class ActionTimeoutException : Exception
    {
        private ActionExecutionResult result;
        private TimeSpan timeout;

        public ActionTimeoutException(ActionExecutionResult result, TimeSpan timeout)
        {
            this.result = result;
            this.timeout = timeout;
        }
    }
}
