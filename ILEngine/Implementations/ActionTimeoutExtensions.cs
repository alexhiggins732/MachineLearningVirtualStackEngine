using System;
using System.Threading;
using System.Threading.Tasks;

namespace ILEngine
{
    public static class ActionTimeoutExtensions
    {
        static void ExecuteAction(object state)
        {
            var executionState = (ActionExecutionResult)state;
            executionState.Success = false;
            try
            {
                executionState.Action();
            }
            catch (Exception ex)
            {
                executionState.Exception = ex;
            }
        }
        public static ActionExecutionResult WithTimeout(this Action action, int timeoutSeconds)
        {
            var result = new ActionExecutionResult { Action = action };
            TimeSpan timeout = TimeSpan.FromSeconds(timeoutSeconds);
            using (var src = new CancellationTokenSource())
            {
                var startParams = new ParameterizedThreadStart(ExecuteAction);
                var t = new Thread(startParams);
                t.Start(result);
                var threadTask = Task.Run(() => t.Join());
                Task.WaitAny(threadTask, Task.Delay(timeout, src.Token));
                if (threadTask.IsCompleted)
                {
                    src.Cancel();
                    if (result.Exception != null)
                    {

                        result.TimedOut = result.Exception is TimeoutException;
                        result.Success = false;
                    }
                    else
                    {
                        result.Success = true;
                    }

                }
                else
                {
                    t.Abort();
                    result.Exception = new ActionTimeoutException(result, timeout);
                }
            }
            return result;
        }
    }
}
