using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace ILEngine.Builder.Tests
{
    [TestClass()]
    public class ILInstructionBuilderTests
    {
        [TestMethod()]
        public void WriteILInstructionTest()
        {
            var opCode = OpCodes.Nop;
            var instruction = ILInstruction.Create(opCode);
            var builder = new ILInstructionBuilder();
            builder.Write(instruction);
            Assert.IsTrue(builder.Instructions.Count == 1);
            Assert.IsTrue(builder.Instructions[0].OpCode == opCode);
            Assert.IsNull(builder.Instructions[0].Arg);
        }

        [TestMethod()]
        public void WriteILInstructionsTest()
        {
            var opCode1 = OpCodes.Nop;
            var opCode2 = OpCodes.Ret;
            var instruction1 = ILInstruction.Create(opCode1);
            var instruction2 = ILInstruction.Create(opCode2);
            var builder = new ILInstructionBuilder();
            builder.Write(instruction1, instruction2);
            Assert.IsTrue(builder.Instructions.Count == 2);
            Assert.IsTrue(builder.Instructions[0].OpCode == opCode1);
            Assert.IsNull(builder.Instructions[0].Arg);
            Assert.IsTrue(builder.Instructions[1].OpCode == opCode2);
            Assert.IsNull(builder.Instructions[1].Arg);
        }

        [TestMethod()]
        public void WriteOpCodeTest()
        {
            var opCode1 = OpCodes.Nop;
            var builder = new ILInstructionBuilder();
            builder.Write(opCode1);
            Assert.IsTrue(builder.Instructions.Count == 1);
            Assert.IsTrue(builder.Instructions[0].OpCode == opCode1);
            Assert.IsNull(builder.Instructions[0].Arg);

        }
        [TestMethod()]
        public void WriteOpCodesTest()
        {
            var opCode1 = OpCodes.Nop;
            var opCode2 = OpCodes.Ret;
            var builder = new ILInstructionBuilder();
            builder.Write(opCode1, opCode2);
            Assert.IsTrue(builder.Instructions.Count == 2);
            Assert.IsTrue(builder.Instructions[0].OpCode == opCode1);
            Assert.IsNull(builder.Instructions[0].Arg);
            Assert.IsTrue(builder.Instructions[1].OpCode == opCode2);
            Assert.IsNull(builder.Instructions[1].Arg);
        }

        [TestMethod()]
        public void WriteOpCodeWithArgTest()
        {
            var opCode1 = OpCodes.Ldarga_S;
            var opCode1Arg = 1;
            var builder = new ILInstructionBuilder();
            builder.Write(opCode1, opCode1Arg);
            Assert.IsTrue(builder.Instructions.Count == 1);
            Assert.IsTrue(builder.Instructions[0].OpCode == opCode1);
            Assert.IsNotNull(builder.Instructions[0].Arg);
            Assert.IsTrue((int)builder.Instructions[0].Arg == opCode1Arg);
        }

        [TestMethod()]
        public void WriteSingleILOpCodeValues()
        {
            var opCode1 = OpCodes.Ret;
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ret);
            Assert.IsTrue(builder.Instructions.Count == 1);
            Assert.IsTrue(builder.Instructions[0].OpCode == opCode1);
            Assert.IsNull(builder.Instructions[0].Arg);
        }

        [TestMethod()]
        public void WriteILOpCodeValues()
        {

            var opCode1 = OpCodes.Nop;
            var opCode2 = OpCodes.Ret;
            var builder = new ILInstructionBuilder();
            builder.Write(opCode1, opCode2);
            Assert.IsTrue(builder.Instructions.Count == 2);
            Assert.IsTrue(builder.Instructions[0].OpCode == opCode1);
            Assert.IsNull(builder.Instructions[0].Arg);
            Assert.IsTrue(builder.Instructions[1].OpCode == opCode2);
            Assert.IsNull(builder.Instructions[1].Arg);
        }

        [TestMethod()]
        public void WriteSingleILOpCodeValuesWithArg()
        {
            var opCode1 = OpCodes.Ldarga_S;
            var opCode1Arg = 1;
            var builder = new ILInstructionBuilder();
            builder.Write(opCode1, opCode1Arg);
            Assert.IsTrue(builder.Instructions.Count == 1);
            Assert.IsTrue(builder.Instructions[0].OpCode == opCode1);
            Assert.IsNotNull(builder.Instructions[0].Arg);
            Assert.IsTrue((int)builder.Instructions[0].Arg == opCode1Arg);
        }

        [TestMethod()]
        public void ClearTest()
        {
            var builder = new ILInstructionBuilder();
            Assert.IsTrue(builder.Instructions.Count == 0);
            builder.Write(OpCodes.Ret);
            Assert.IsTrue(builder.Instructions.Count == 1);
            Assert.IsTrue(builder.Instructions[0].OpCode == OpCodes.Ret);
            builder.Clear();
            Assert.IsTrue(builder.Instructions.Count == 0);

        }
    }
}