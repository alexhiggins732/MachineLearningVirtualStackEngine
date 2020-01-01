using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace ILEngine.Tests
{
    [TestClass()]
    public class InvalidOpCodeExceptionTests
    {
        [TestMethod()]
        public void InvalidOpCodeExceptionFromOpCodeValueTest()
        {
            var ex = new InvalidOpCodeException(OpCodes.Nop.Value);
            var actual = ex.Message;
            var expected = $"Invalid Opcode {OpCodes.Nop.Value}.";
            Assert.IsTrue(actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        [TestMethod()]
        public void InvalidOpCodeExceptionFromOpCodeTest()
        {
            var ex = new InvalidOpCodeException(OpCodes.Nop);
            var actual = ex.Message;
            var expected = $"Invalid Opcode {OpCodes.Nop}.";
            Assert.IsTrue(actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        [TestMethod()]
        public void InvalidOpCodeExceptionFromIlOpCodeValuesTest()
        {
            var ex = new InvalidOpCodeException(ILOpCodeValues.Nop);
            var actual = ex.Message;
            var expected = $"Invalid Opcode {ILOpCodeValues.Nop}.";
            Assert.IsTrue(actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }
    }
}