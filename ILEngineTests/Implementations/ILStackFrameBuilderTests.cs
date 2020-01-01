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
    public class ILStackFrameBuilderGenericTests
     : ILStackFrameBuilderGenericTests<ILStackFrameBuilder<ILStackFrameWithDiagnostics>, ILStackFrameWithDiagnostics>
    {

    }

    [TestClass()]
    public class ILStackFrameBuilderTests
    {
        [TestMethod()]
        public void BuildOpCodesTest()
        {
            var opCode1 = OpCodes.Ret;
            var opCodes = (new[] { opCode1 }).ToList();
            var frame = ILStackFrameBuilder.Build(opCodes);
            Assert.IsTrue(frame.Stream.Count == 1);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
        }

        [TestMethod()]
        public void BuildILInstructionsTest()
        {
            var opCode1 = OpCodes.Ret;
            var instruction = ILInstruction.Create(opCode1);
            var instructions = (new[] { instruction }).ToList();
            var frame = ILStackFrameBuilder.Build(instructions);
            Assert.IsTrue(frame.Stream.Count == 1);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
        }

        [TestMethod()]
        public void BuildAndExecuteOpCodesTest()
        {
            var opCode1 = OpCodes.Ldc_I4_1;
            var opCode2 = OpCodes.Ret;
            var opCodes = (new[] { opCode1, opCode2 }).ToList();
            var frame = ILStackFrameBuilder.BuildAndExecute(opCodes);
            Assert.IsTrue(frame.Stream.Count == 2);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
            Assert.IsTrue(frame.Stream[1].OpCode == opCode2);
            var actual = frame.ReturnResult;
            Assert.IsNotNull(actual);
            var expected = 1;
            Assert.IsTrue((int)actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        [TestMethod()]
        public void BuildAndExecuteOpCodesWithTimeoutTest()
        {
            var opCode1 = OpCodes.Ldc_I4_1;
            var opCode2 = OpCodes.Ret;
            var opCodes = (new[] { opCode1, opCode2 }).ToList();
            var frame = ILStackFrameBuilder.BuildAndExecute(opCodes, 1);
            Assert.IsTrue(frame.Stream.Count == 2);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
            Assert.IsTrue(frame.Stream[1].OpCode == opCode2);
            var actual = frame.ReturnResult;
            Assert.IsNotNull(actual);
            var expected = 1;
            Assert.IsTrue((int)actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        [TestMethod()]
        public void BuildAndExecuteILInstructionsTest()
        {
            var opCode1 = OpCodes.Ldc_I4_1;
            var opCode2 = OpCodes.Ret;
            var instruction1 = ILInstruction.Create(opCode1);
            var instruction2 = ILInstruction.Create(opCode2);
            var instructions = (new[] { instruction1, instruction2 }).ToList();
            var frame = ILStackFrameBuilder.BuildAndExecute(instructions);
            Assert.IsTrue(frame.Stream.Count == 2);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
            Assert.IsTrue(frame.Stream[1].OpCode == opCode2);
            var actual = frame.ReturnResult;
            Assert.IsNotNull(actual);
            var expected = 1;
            Assert.IsTrue((int)actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        [TestMethod()]
        public void BuildAndExecuteILInstructionsWithTimeoutTest()
        {
            var opCode1 = OpCodes.Ldc_I4_1;
            var opCode2 = OpCodes.Ret;
            var instruction1 = ILInstruction.Create(opCode1);
            var instruction2 = ILInstruction.Create(opCode2);
            var instructions = (new[] { instruction1, instruction2 }).ToList();
            var frame = ILStackFrameBuilder.BuildAndExecute(instructions, 1);
            Assert.IsTrue(frame.Stream.Count == 2);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
            Assert.IsTrue(frame.Stream[1].OpCode == opCode2);
            var actual = frame.ReturnResult;
            Assert.IsNotNull(actual);
            var expected = 1;
            Assert.IsTrue((int)actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            TestExecuteOpCodeFrame();
            TestExecuteInstructionFrame();
        }

        private void TestExecuteInstructionFrame()
        {
            var opCode1 = OpCodes.Ldc_I4_1;
            var opCode2 = OpCodes.Ret;
            var instruction1 = ILInstruction.Create(opCode1);
            var instruction2 = ILInstruction.Create(opCode2);
            var instructions = (new[] { instruction1, instruction2 }).ToList();
            var frame = ILStackFrameBuilder.Build(instructions);
            ILStackFrameBuilder.Execute(frame, 1);
            Assert.IsTrue(frame.Stream.Count == 2);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
            Assert.IsTrue(frame.Stream[1].OpCode == opCode2);
            var actual = frame.ReturnResult;
            Assert.IsNotNull(actual);
            var expected = 1;
            Assert.IsTrue((int)actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        private void TestExecuteOpCodeFrame()
        {
            var opCode1 = OpCodes.Ldc_I4_1;
            var opCode2 = OpCodes.Ret;
            var opCodes = (new[] { opCode1, opCode2 }).ToList();
            var frame = ILStackFrameBuilder.Build(opCodes);
            ILStackFrameBuilder.Execute(frame, 1);
            Assert.IsTrue(frame.Stream.Count == 2);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
            Assert.IsTrue(frame.Stream[1].OpCode == opCode2);
            var actual = frame.ReturnResult;
            Assert.IsNotNull(actual);
            var expected = 1;
            Assert.IsTrue((int)actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }
    }


    public class ILStackFrameBuilderGenericTests<TFrameBuilder, TStack>
        where TFrameBuilder : IILStackFrameBuilder<TStack>, new()
        where TStack : IILStackFrame, new()
    {
        TFrameBuilder Builder() => new TFrameBuilder();
        [TestMethod()]
        public void BuildOpCodesTest()
        {
            var opCode1 = OpCodes.Ret;
            var opCodes = (new[] { opCode1 }).ToList();
            var frame = Builder().Build(opCodes);
            Assert.IsTrue(frame.Stream.Count == 1);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
        }

        [TestMethod()]
        public void BuildILInstructionsTest()
        {
            var opCode1 = OpCodes.Ret;
            var instruction = ILInstruction.Create(opCode1);
            var instructions = (new[] { instruction }).ToList();
            var frame = Builder().Build(instructions);
            Assert.IsTrue(frame.Stream.Count == 1);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
        }

        [TestMethod()]
        public void BuildAndExecuteOpCodesTest()
        {
            var opCode1 = OpCodes.Ldc_I4_1;
            var opCode2 = OpCodes.Ret;
            var opCodes = (new[] { opCode1, opCode2 }).ToList();
            var frame = Builder().BuildAndExecute(opCodes);
            Assert.IsTrue(frame.Stream.Count == 2);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
            Assert.IsTrue(frame.Stream[1].OpCode == opCode2);
            var actual = frame.ReturnResult;
            Assert.IsNotNull(actual);
            var expected = 1;
            Assert.IsTrue((int)actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        [TestMethod()]
        public void BuildAndExecuteOpCodesWithTimeoutTest()
        {
            var opCode1 = OpCodes.Ldc_I4_1;
            var opCode2 = OpCodes.Ret;
            var opCodes = (new[] { opCode1, opCode2 }).ToList();
            var frame = Builder().BuildAndExecute(opCodes, 1);
            Assert.IsTrue(frame.Stream.Count == 2);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
            Assert.IsTrue(frame.Stream[1].OpCode == opCode2);
            var actual = frame.ReturnResult;
            Assert.IsNotNull(actual);
            var expected = 1;
            Assert.IsTrue((int)actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        [TestMethod()]
        public void BuildAndExecuteILInstructionsTest()
        {
            var opCode1 = OpCodes.Ldc_I4_1;
            var opCode2 = OpCodes.Ret;
            var instruction1 = ILInstruction.Create(opCode1);
            var instruction2 = ILInstruction.Create(opCode2);
            var instructions = (new[] { instruction1, instruction2 }).ToList();
            var frame = Builder().BuildAndExecute(instructions);
            Assert.IsTrue(frame.Stream.Count == 2);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
            Assert.IsTrue(frame.Stream[1].OpCode == opCode2);
            var actual = frame.ReturnResult;
            Assert.IsNotNull(actual);
            var expected = 1;
            Assert.IsTrue((int)actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        [TestMethod()]
        public void BuildAndExecuteILInstructionsWithTimeoutTest()
        {
            var opCode1 = OpCodes.Ldc_I4_1;
            var opCode2 = OpCodes.Ret;
            var instruction1 = ILInstruction.Create(opCode1);
            var instruction2 = ILInstruction.Create(opCode2);
            var instructions = (new[] { instruction1, instruction2 }).ToList();
            var frame = Builder().BuildAndExecute(instructions, 1);
            Assert.IsTrue(frame.Stream.Count == 2);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
            Assert.IsTrue(frame.Stream[1].OpCode == opCode2);
            var actual = frame.ReturnResult;
            Assert.IsNotNull(actual);
            var expected = 1;
            Assert.IsTrue((int)actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            TestExecuteOpCodeFrame();
            TestExecuteInstructionFrame();
        }

        [TestMethod()]
        public void ExecuteThrow()
        {
            var instructionBuilder = new ILInstructionBuilder();
            instructionBuilder.Write(OpCodes.Ldstr, "test");
            instructionBuilder.Write(OpCodes.Conv_I4);
            instructionBuilder.Write(OpCodes.Ret);
            var builder = Builder();
            var frame = builder.Build(instructionBuilder.Instructions);
            frame.Execute(0, true);
        }

        private void TestExecuteInstructionFrame()
        {
            var opCode1 = OpCodes.Ldc_I4_1;
            var opCode2 = OpCodes.Ret;
            var instruction1 = ILInstruction.Create(opCode1);
            var instruction2 = ILInstruction.Create(opCode2);
            var instructions = (new[] { instruction1, instruction2 }).ToList();
            var builder = Builder();
            var frame = builder.Build(instructions);
            builder.Execute(frame, 1);
            Assert.IsTrue(frame.Stream.Count == 2);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
            Assert.IsTrue(frame.Stream[1].OpCode == opCode2);
            var actual = frame.ReturnResult;
            Assert.IsNotNull(actual);
            var expected = 1;
            Assert.IsTrue((int)actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }

        private void TestExecuteOpCodeFrame()
        {
            var opCode1 = OpCodes.Ldc_I4_1;
            var opCode2 = OpCodes.Ret;
            var opCodes = (new[] { opCode1, opCode2 }).ToList();
            var builder = Builder();
            var frame = builder.Build(opCodes);
            builder.Execute(frame, 1);
            Assert.IsTrue(frame.Stream.Count == 2);
            Assert.IsTrue(frame.Stream[0].OpCode == opCode1);
            Assert.IsTrue(frame.Stream[1].OpCode == opCode2);
            var actual = frame.ReturnResult;
            Assert.IsNotNull(actual);
            var expected = 1;
            Assert.IsTrue((int)actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }
    }

}