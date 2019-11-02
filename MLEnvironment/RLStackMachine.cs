using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MLEnvironment
{
    /*
     
     Traditional MSIL requires arguments within the IL code to be set in order to execute.
        EG: Ldloc_S {(short)ArgIndex}
    Initial an agent who has learned nothing will need to learn to generate these arguments in order to be able
        to execute ld.loc.s 6;

    Do we give the full IL stack and allow agents to generate and work with the entire MSIL code base
        or do we give an argument free OpCode engine and require the argent to learn from that.

    Argument free would require rewriting the entire VM stack from OpCode to execution engine and will likely result in roadbloacks.
    

     */
    class RLStackMachine
    {
    }

    public class PolicyBuilder : IPolicyBuilder
    {

    }
    public interface IPolicyBuilder
    {

    }
    public interface IPolicy
    {
        Action Action { get; set; }
        void Execute();
    }
    public interface IExecutionPolicy : IPolicy
    {
        void Enter();
        bool Continue();
        void Exit();

    }
    public interface ITimespanExecutionPolicy : IExecutionPolicy
    {

    }
    public static class TaskExtensions
    {
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
        {

            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {

                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    return await task;  // Very important in order to propagate exceptions
                }
                else
                {
                    throw new TimeoutException("The operation has timed out.");
                }
            }
        }
        private static async Task TimeoutAfter(this Task task, TimeSpan timeout)
        {

            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {

                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    await task;  // Very important in order to propagate exceptions
                }
                else
                {
                    throw new TimeoutException("The operation has timed out.");
                }
            }
        }
        public static async Task TimeoutAfter(this Action action, TimeSpan timeout)
        {
            var threadStart = new ParameterizedThreadStart(ActionExecutor);
            var thread = new System.Threading.Thread(threadStart);
            thread.Start(action);
            var task = Task.Run(() => thread.Join());
            try
            {
                await task.TimeoutAfter(timeout);
            }
            catch (TimeoutException)
            {
                thread.Abort();
                throw;
            }
        }

        private static void ActionExecutor(object state)
        {
            ((Action)state)();
        }

        public static async Task TimeoutAfter(this Action action, TimespanExecutionPolicy policy)
        {
            await action.TimeoutAfter(policy.Timeout);


        }
    }


    public class TimespanExecutionPolicy
    {
        public TimeSpan Timeout { get; private set; }
        Stopwatch watch = null;
        public TimeSpan Elapsed => watch?.Elapsed ?? TimeSpan.MinValue;

        public bool TimedOut { get; private set; }
        public Exception Exception { get; private set; }

        public TimespanExecutionPolicy(TimeSpan timeout)
        {
            this.Timeout = timeout;
        }

        public bool Continue()
        {
            return Elapsed < Timeout;
        }

        public void Enter()
        {
            this.TimedOut = false;
            this.Exception = null;
            watch = Stopwatch.StartNew();
        }

        public void Exit()
        {
            watch.Stop();
        }

        public void Execute(Action action)
        {
            Enter();
            try
            {
                action.TimeoutAfter(Timeout).GetAwaiter().GetResult();
            }
            catch (TimeoutException timeoutException)
            {
                this.Exception = timeoutException;
                this.TimedOut = true;
            }
            catch (Exception ex)
            {
                this.Exception = ex;
            }
            Exit();
        }
    }


    public static class PolicyExtensions
    {
        public static IPolicy WithExceptionHandler(this Action executeAction, Action<Exception> handler)
        {
            HandleExceptionPolicy handleExceptionPolicy = null;
               Action executeWithHandler = () =>
            {
                try
                {
                    executeAction();
                }
                catch (Exception ex)
                {
                    handleExceptionPolicy.HandledException = ex;
                    handler(ex);
                }
            };
            handleExceptionPolicy = new HandleExceptionPolicy(executeWithHandler);
            return handleExceptionPolicy;
        }
        public static IPolicy WithIgnoreExceptionPolicy(this IPolicy policy, TimeSpan timeout)
        {
            return policy;
        }
        public static IPolicy WithTimeoutPolicy(this IPolicy policy, TimeSpan timeout)
        {
            return new TimeoutPolicy(policy, timeout);

        }
    }

    public class HandleExceptionPolicy : IPolicy
    {
        public Action Action { get; set; }

        public HandleExceptionPolicy(Action executeWithHandler)
        {
            this.Action = executeWithHandler;
        }
        public HandleExceptionPolicy(IPolicy policy)
        {
            this.Action = policy.Action;
        }

        public Exception HandledException { get; set; }
        public void Execute()
        {
            try
            {
                Action();
            }
            catch (Exception ex)
            {
                HandledException = ex;
            }

        }
    }

    public static class ExceptionExtensions
    {
        public static IPolicy WithIgnoreExceptionPolicy(this Action action)
        {
            return new IgnoreExceptionPolicy(action);

        }
    }

    public class TimeoutPolicy : IPolicy
    {
        public Action Action { get; set; }
        TimespanExecutionPolicy timespanExecutionPolicy;
        private TimeSpan timeout;

        public TimeoutPolicy(Action action, TimeSpan timeout)
        {
            this.Action = action;
            this.timespanExecutionPolicy = new TimespanExecutionPolicy(timeout);

        }

        public TimeoutPolicy(IPolicy policy, TimeSpan timeout) : this(policy.Execute, timeout) { }


        public void Execute()
        {
            timespanExecutionPolicy.Execute(Action);
        }
    }
    public class IgnoreExceptionPolicy : IPolicy
    {
        public Action Action { get; set; }

        public IgnoreExceptionPolicy(Action action)
        {
            this.Action = action;
        }
        public IgnoreExceptionPolicy(IPolicy policy)
        {
            this.Action = policy.Execute;
        }

        public Exception IgnoredException { get; private set; }
        public void Execute()
        {
            try
            {
                Action();
            }
            catch (Exception handledException)
            {
                this.IgnoredException = handledException;
            }
        }
    }
}
