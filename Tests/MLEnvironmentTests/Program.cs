using MLEnvironment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLEnvironmentTests
{
    class Program
    {
        public static void Main(string[] args)
        {
            TestPolicyBuilder();
        }

        private static void TestPolicyBuilder()
        {
            Action throwExceptionAction = () =>
            {
                throw new Exception(nameof(throwExceptionAction));
            };
            throwExceptionAction.WithIgnoreExceptionPolicy().WithTimeoutPolicy(TimeSpan.FromSeconds(4)).Execute();

        }

        static void TestTimeoutPolicy()
        {
            var timeoutPolicy = new TimespanExecutionPolicy(TimeSpan.FromSeconds(5));
            int sleepCount = 10;
            timeoutPolicy.Execute(() =>
            {
                for (var i = 0; i < sleepCount; i++)
                {
                    System.Diagnostics.Debug.WriteLine($"Iteration {i}");
                    System.Threading.Thread.Sleep(1000);
                }

            });
            System.Diagnostics.Debug.WriteLine($"Executed in {timeoutPolicy.Elapsed}, TimedOut: {timeoutPolicy.TimedOut}");
            System.Threading.Thread.Sleep(10000);
        }
    }
}
