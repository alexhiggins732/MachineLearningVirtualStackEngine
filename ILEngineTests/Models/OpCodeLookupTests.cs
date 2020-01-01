using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILEngine.Models;
using System.Reflection.Emit;

namespace ILEngine.Tests
{
    [TestClass()]
    public class OpCodeLookupTests
    {
        [TestMethod()]
        public void GetOpCodeArgByteSizeTest()
        {
            int actual = OpCodeLookup.GetOpCodeArgByteSize(ILOpCodeValues.Nop);
            int expected = 0;
            Assert.IsTrue(actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");

            var metas = OpCodeMetaModel.OpCodeMetas;
            foreach (var meta in metas)
            {
                expected = meta.OperandTypeByteSize;
                try
                {
                    actual = OpCodeLookup.GetOpCodeArgByteSize((ILOpCodeValues)unchecked((ushort)meta.OpCodeValue));
                  
                    Assert.IsTrue(actual == expected, $"Argsize lookup failed for {meta.ClrName}\r\nActual:{actual}\r\nExpected:{expected}\r\n");
                }
                catch (NotImplementedException)
                {
                    Assert.Fail($"Argsize not implemented for failed for {meta.ClrName}: Expected: {expected}");

                }
            }
        }

        [TestMethod()]
        public void GetILOpcodePositiveTest()
        {
            var actual = OpCodeLookup.GetILOpcode(OpCodes.Nop.Value);
            var expected = OpCodes.Nop;
            Assert.IsTrue(actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        [ExpectedException(typeof(KeyNotFoundException))]
        [TestMethod()]
        public void GetILOpcodeNegativeTest()
        {
            var actual = OpCodeLookup.GetILOpcode(300);
        }

        [TestMethod()]
        public void GetILOpcodeDebugPositiveTest()
        {
            var actual = OpCodeLookup.GetILOpcodeDebug(OpCodes.Nop.Value);
            var expected = OpCodes.Nop;
            Assert.IsTrue(actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        [ExpectedException(typeof(KeyNotFoundException))]
        [TestMethod()]
        public void GetILOpcodeDebugNegativeTest()
        {
            var actual = OpCodeLookup.GetILOpcode(300);
        }

    }
}