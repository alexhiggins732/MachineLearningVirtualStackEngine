using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLEnvironment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLEnvironmentTests
{
    [TestClass]
    public class TimeoutPolicyTests
    {
        [TestMethod]
        public void Policy_WhenGivenAmpleTime_Succeed()
        {
            var timeoutPolicy = new TimespanExecutionPolicy(TimeSpan.FromSeconds(5));
            int sleepCount = 3;
            int executionCount = 0;
            timeoutPolicy.Execute(() =>
            {
                for (var i = 0; i < sleepCount; i++)
                {
                    executionCount++;
                    System.Diagnostics.Debug.WriteLine($"Iteration {i}");
                    System.Threading.Thread.Sleep(1000);
                }

            });
            Exception ex = timeoutPolicy.Exception;
            Assert.IsNull(ex, $"Policy caught an {ex?.GetType()}: {ex?.Message}");
            Assert.IsFalse(timeoutPolicy.TimedOut, "Policy threw a timeout");
            Assert.AreEqual(executionCount, sleepCount);
        }


        [TestMethod]
        public void Policy_WhenNotGivenAmpleTime_Fails()
        {
            var timeoutPolicy = new TimespanExecutionPolicy(TimeSpan.FromSeconds(5));
            int sleepCount = 10;
            int executionCount = 0;
            int maxExpectedExecutions = 6;
            timeoutPolicy.Execute(() =>
            {
                for (var i = 0; i < sleepCount; i++)
                {
                    executionCount++;
                    System.Diagnostics.Debug.WriteLine($"Iteration {i}");
                    System.Threading.Thread.Sleep(1000);
                }

            });
            Exception ex = timeoutPolicy.Exception;
            Assert.IsNotNull(ex, $"Policy failed to handle an exception");
            Assert.IsTrue(timeoutPolicy.TimedOut, "Policy failed to throw a timeout");
            System.Threading.Thread.Sleep(sleepCount * 1000);
            Assert.IsTrue(executionCount < sleepCount, "Policy failed to cancel execution");
            Assert.IsTrue(executionCount <= maxExpectedExecutions, $"Policy failed to cancel execution within {maxExpectedExecutions}. Executions: {executionCount}");
        }
    }
}
