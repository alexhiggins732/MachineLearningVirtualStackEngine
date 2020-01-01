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
    public class OpCodeNotImplementedExceptionTests
    {
        [TestMethod()]
        public void OpCodeNotImplementedExceptionFromShortTest()
        {
            var ex = new OpCodeNotImplementedException(OpCodes.Nop.Value);
            var actual = ex.Message;
            var expected = $"Opcode {OpCodes.Nop.Value} is not implemented";
            Assert.IsTrue(actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");

        }

        [TestMethod()]
        public void OpCodeNotImplementedExceptionFromOpcodeTest()
        {
            var ex = new OpCodeNotImplementedException(OpCodes.Nop);
            var actual = ex.Message;
            var expected = $"Opcode {OpCodes.Nop} is not implemented";
            Assert.IsTrue(actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        [TestMethod()]
        public void OpCodeNotImplementedExceptionFromILOpCodeValuesTest()
        {
            var ex = new OpCodeNotImplementedException(ILOpCodeValues.Nop);
            var actual = ex.Message;
            var expected = $"Opcode {ILOpCodeValues.Nop} is not implemented";
            Assert.IsTrue(actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }
    }
}