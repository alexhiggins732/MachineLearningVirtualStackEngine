#define testcustom
#undef testcustom
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;



namespace ILEngine.Tests
{
    public class ILEngineTestsBase<TEngine, TStackFrame>
        where TEngine : IILEngine, new()
        where TStackFrame : IILStackFrame<TEngine>, new()
    {

        protected TEngine NewEngine()
        {
            return new TEngine();
        }

        protected TStackFrame NewFrame()
        {
            return new TStackFrame();
        }
        protected TStackFrame BuildTestFrame(List<ILInstruction> stream,
            IILInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            var result = NewFrame();
            result.Stream = stream;
            result.Resolver = resolver;
            result.Args = args;
            result.Locals = locals;
            result.Reset();
            return result;
        }

        protected void AssertExceptionIsNull(TStackFrame frame)
        {
            Assert.IsNull(frame.Exception, $"Unexpected exception: {frame.Exception}");
        }

        protected void AssertException<TException>(TStackFrame frame)
        {
            AssertException(frame, typeof(TException));

        }
        protected void AssertException(TStackFrame frame, Type exceptionType)
        {
            Assert.IsNotNull(frame.Exception, $"Exception is null");
            Assert.IsInstanceOfType(frame.Exception, exceptionType);
        }

        protected void AssertEmptyStack(TStackFrame frame)
        {
            Assert.IsTrue(frame.Stack.Count == 0, $"Frame stack is not empty: Found {frame.Stack.Count} items on the stack.");
        }
        protected void AssertEmptyStackWithException(TStackFrame frame, Type exceptionType)
        {
            AssertEmptyStack(frame);
            AssertException(frame, exceptionType);
        }

        protected void AssertFrameResult(TStackFrame frame, dynamic expected)
        {
            var actual = frame.ReturnResult;
            if (actual != null && actual.GetType().IsArray && expected != null && expected.GetType().IsArray)
            {
                Type actualArrayType = actual.GetType();
                Type expectedArrayType = expected.GetType();
                Assert.IsTrue(actualArrayType == expectedArrayType, $"Unexpected Array Type\r\nActual:\r\n{actualArrayType}\r\nExpected:\r\n{expectedArrayType}");

                var actualElementType = actualArrayType.GetElementType();
                var expectedElementType = actualArrayType.GetElementType();
                Assert.IsTrue(actualElementType == expectedElementType, $"Unexpected Array ElementType\r\nActual:\r\n{actualElementType}\r\nExpected:\r\n{expectedElementType}");

                Assert.IsTrue(actual.Length == expected.Length, $"Unexpected Array Length Mismatch\r\nActual:\r\n{actual.Length}\r\nExpected:\r\n{expected.Length}");

                for (var i = 0; i < actual.Length; i++)
                {
                    Assert.IsTrue(actual[i] == expected[i], $"Unexpected Array Element Mismatch\r\nActual[{i}]:\r\n{actual[i]}\r\nExpected[{i}]:\r\n{expected[i]}");

                }
            }
            else
            {
                Assert.IsTrue(actual == expected, $"Unexpected result\r\nActual:\r\n{actual}\r\nExpected:\r\n{expected}");
            }

        }
        //protected void AssertFrameResult<TResult>(TStackFrame frame, object expected, Func<object, TResult> converter)
        //{
        //    var actual = frame.ReturnResult;
        //    Assert.IsTrue(converter(actual) == converter(expected), $"Unexpected result\r\nActual:\r\n{actual}\r\nExpected:\r\n{expected}");
        //}
        protected void AssertEmptyStackWithResult(TStackFrame frame, dynamic expected)
        {
            AssertExceptionIsNull(frame);
            AssertEmptyStack(frame);
            AssertFrameResult(frame, expected);
        }
        //protected void AssertEmptyStackResult<TResult>(TStackFrame frame, object expected, Func<object, TResult> converter)
        //{
        //    AssertExceptionIsNull(frame);
        //    AssertEmptyStack(frame);
        //    AssertFrameResult(frame, expected, converter);
        //}


        protected TStackFrame Build(params ILOpCodeValues[] instructions)
        {
            var builder = new ILInstructionBuilder();
            builder.Write(instructions);
            var frame = BuildTestFrame(builder.Instructions);
            return frame;
        }

        protected TStackFrame Build(params ILInstruction[] instructions)
        {
            var frame = BuildTestFrame(instructions.ToList());
            return frame;
        }

        protected TStackFrame Build(List<ILInstruction> instructions)
        {
            var frame = BuildTestFrame(instructions);
            return frame;
        }
        protected TStackFrame BuildAndExecute(params ILOpCodeValues[] instructions)
        {
            var frame = Build(instructions);
            Execute(frame);
            return frame;
        }
        protected TStackFrame BuildAndExecute(List<ILInstruction> instructions)
        {
            var frame = Build(instructions);
            Execute(frame);
            return frame;
        }
        protected TStackFrame BuildAndExecute(params ILInstruction[] instructions)
        {
            var frame = Build(instructions);
            Execute(frame);
            return frame;
        }
        protected void Execute(TStackFrame frame)
        {
            var engine = NewEngine();
            try
            {
                engine.ExecuteFrame(frame);
            }
            catch (Exception ex)
            {
                frame.Exception = ex;
                if (engine.ThrowOnException)
                {
                    throw ex;
                }
            }
        }

        protected List<ILInstruction> GetIlInstructions(MethodInfo method)
            => ILInstructionReader.FromByteCode(method.GetMethodBody().GetILAsByteArray());

    }
}