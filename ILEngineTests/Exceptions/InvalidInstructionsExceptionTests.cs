using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.Tests
{
    [TestClass()]
    public class InvalidInstructionsExceptionTests
    {
        [TestMethod()]
        public void InvalidInstructionsExceptionTest()
        {
            var expected = "End of instructions reached without a Ret statement";
            var ex = new InvalidInstructionsException(expected);
            var actual = ex.Message;
            Assert.IsTrue(actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }
    }
}