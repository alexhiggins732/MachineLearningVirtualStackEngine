#define testcustom
#undef testcustom
using ILEngine.Compilers;
using ILEngine.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace ILEngine.Tests
{
    public class ILEngineTestRunnerBase<TILEngine, TStackFrame> : ILEngineTestsBase<TILEngine, TStackFrame>
        where TILEngine: class, IILEngine, new()
        where TStackFrame: class, IILStackFrame<TILEngine>, new()
    {

        #region AddTests

        [TestMethod()]
        public void Add_Ovf_Test()
        {
            var frame = BuildAndExecute(ILOpCodeValues.Ldc_I4_1, ILOpCodeValues.Ldc_I4_2, ILOpCodeValues.Add_Ovf, ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, 3);

            //TODO: Run tests for different data types
            //TODO: Run tests for overflows.
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, int.MaxValue);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Add_Ovf);
            builder.Write(ILOpCodeValues.Ret);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithException(frame, typeof(OverflowException));

        }

        [TestMethod()]
        public void Add_Ovf_Un_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, 1);
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ldc_I4, 2);
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Add_Ovf_Un);
            builder.Write(ILOpCodeValues.Ret);
            var frame = BuildAndExecute(builder.Instructions);

            AssertEmptyStackWithResult(frame, 3u);

            //TODO: Run tests for different data types
            //TODO: Run tests for overflows.
            builder.Clear();

            builder.Write(ILOpCodeValues.Ldc_I4, unchecked((int)uint.MaxValue));
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Add_Ovf_Un);
            frame = BuildAndExecute(builder.Instructions);

            AssertEmptyStackWithException(frame, typeof(OverflowException));
        }

        [TestMethod()]
        public void Add_Test()
        {
            var frame = BuildAndExecute(ILOpCodeValues.Ldc_I4_1, ILOpCodeValues.Ldc_I4_2, ILOpCodeValues.Add, ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, 3);

            //TODO: Run tests for different data types
            //TODO: Run tests for overflows.
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, int.MaxValue);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Add);
            builder.Write(ILOpCodeValues.Ret);
            int expected = unchecked(int.MaxValue + 1);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, expected);

            builder.Clear();
            builder.Write(ILOpCodeValues.Ldc_I4, unchecked((int)uint.MaxValue));
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Add);
            builder.Write(ILOpCodeValues.Ret);
            uint expected2 = unchecked(unchecked((int)uint.MaxValue) + 1);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, expected2);
        }

        #endregion

        [TestMethod()]
        public void And_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.And);
            builder.Write(ILOpCodeValues.Ret);
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            builder.Clear();
            frame.Args = new object[] { true, false };
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.And);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, false);

        }

        [TestMethod()]
        public void Arglist_Test()
        {
            var frame = BuildAndExecute(ILOpCodeValues.Arglist);
            AssertEmptyStackWithException(frame, typeof(OpCodeNotImplementedException));
        }

        private void AssertOpCodeNotImplemented(ILOpCodeValues opCode)
        {
            var frame = BuildAndExecute(opCode);
            AssertEmptyStackWithException(frame, typeof(OpCodeNotImplementedException));
        }
        private void AssertOpCodeNotImplemented(ILInstruction instruction)
        {
            var frame = BuildAndExecute(instruction);
            AssertEmptyStackWithException(frame, typeof(OpCodeNotImplementedException));
        }

        #region ConditionalBranchTests

        [TestMethod()]
        public void Beq_S_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_0);//0
            builder.Write(ILOpCodeValues.Ldc_I4_0);//1
            builder.Write(ILOpCodeValues.Beq_S, 3);//2,3
            builder.Write(ILOpCodeValues.Ldc_I4_0);//4
            builder.Write(ILOpCodeValues.Br_S, 1); //5, 6
            builder.Write(ILOpCodeValues.Ldc_I4_1);//7
            builder.Write(ILOpCodeValues.Ret);//8

            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldarg_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldarg_1);
            frame.Stream = builder.Instructions;
            frame.Args = new object[] { new ComparisonTest(0), new ComparisonTest(0) };

            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            frame.Args[0] = new ComparisonTest(1);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);
        }

        [TestMethod()]
        public void Beq_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_0);//0
            builder.Write(ILOpCodeValues.Ldc_I4_0);//1
            builder.Write(ILOpCodeValues.Beq, 3);//2 3,4,5,6
            builder.Write(ILOpCodeValues.Ldc_I4_0);//7
            builder.Write(ILOpCodeValues.Br_S, 1); //8, 9
            builder.Write(ILOpCodeValues.Ldc_I4_1);//10
            builder.Write(ILOpCodeValues.Ret);//11

            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldarg_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldarg_1);
            frame.Stream = builder.Instructions;
            frame.Args = new object[] { new ComparisonTest(0), new ComparisonTest(0) };


            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            frame.Args[0] = new ComparisonTest(1);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);


        }

        [TestMethod()]
        public void BgeTestCompiled()
        {
            Func<int, int, int> testMethod1 = (a, b) =>
            {
                if (a >= b)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            };
            var expected = testMethod1(0, 0);
            var instructions = GetIlInstructions(testMethod1.Method);
            var frame = Build(instructions);
            frame.SetResolver(testMethod1.Method);
            frame.Locals = new[] {
                new ILVariable { Index=0, Type=typeof(int), Name="a"},
                new ILVariable { Index=1, Type=typeof(int), Name="b"},};

            frame.Args = new object[] { this, 0, 0 };

            Execute(frame);
            AssertEmptyStackWithResult(frame, expected);

            frame.Args = new object[] { this, 0, 1 };
            expected = testMethod1(0, 1);
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected);

            frame.Args = new object[] { this, 1, 0 };
            expected = testMethod1(1, 0);
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected);

        }

        public static int BgeTestCompiledMethod()
        {
            var a = 0;
            var b = 0;
            if (a >= b)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        [TestMethod]
        public void BgeTestEmitted()
        {

            Func<string> GetUniqueMethodName = () =>
            {
                return $"{nameof(BgeTestEmitted)}_0";
            };

            //setup
            var method = ILMethodBuilder<int>.Create(GetUniqueMethodName());
            var builder = new ILInstructionBuilder();
            method.Instructions = builder.Instructions;

            builder.Write(ILOpCodeValues.Ldc_I4_0);//0
            builder.Write(ILOpCodeValues.Ldc_I4_0);//1
            builder.Write(ILOpCodeValues.Bge, 3);//2 3,4,5,6
            builder.Write(ILOpCodeValues.Ldc_I4_0);//7
            builder.Write(ILOpCodeValues.Br_S, 1); //8, 9
            builder.Write(ILOpCodeValues.Ldc_I4_1);//10
            builder.Write(ILOpCodeValues.Ret);//11

            var frame = Build(builder.Instructions);
            frame.Stream = builder.Instructions;


            //test bge equal
            var compiled = method.Compile();
            var result = compiled.Invoke(null, new object[] { });
            Assert.AreEqual<int>(1, (int)result);
            Execute(frame);
            AssertEmptyStackWithResult(frame, result);


            //test bge greater
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);
            result = method.Compile().Invoke(null, new object[] { });
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, result);
            Assert.AreEqual<int>(1, (int)result);


            //Test branch bge less
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);
            result = method.Compile().Invoke(null, new object[] { });
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, result);
            Assert.AreEqual<int>(0, (int)result);
        }

        [TestMethod()]
        public void Bge_S_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_0);//0
            builder.Write(ILOpCodeValues.Ldc_I4_0);//1
            builder.Write(ILOpCodeValues.Bge_S, 3);//2 3,4,5,6
            builder.Write(ILOpCodeValues.Ldc_I4_0);//7
            builder.Write(ILOpCodeValues.Br_S, 1); //8, 9
            builder.Write(ILOpCodeValues.Ldc_I4_1);//10
            builder.Write(ILOpCodeValues.Ret);//11

            //Test break equal
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            //Test branch greater
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            //Test branch !(equal or greater)
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

#if testcustom
            //Test branch custom equal
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldarg_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldarg_1);
            frame.Stream = builder.Instructions;
            frame.Args = new object[] { new ComparisonTest(0), new ComparisonTest(0) };

            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test branch custom greater
            frame.Args[0] = new ComparisonTest(1);

            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test branch custom !(equal or greater)
            frame.Args[0] = new ComparisonTest(0);
            frame.Args[1] = new ComparisonTest(1);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);
#endif
        }

        [TestMethod()]
        public void Bge_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_0);//0
            builder.Write(ILOpCodeValues.Ldc_I4_0);//1
            builder.Write(ILOpCodeValues.Bge, 3);//2 3,4,5,6
            builder.Write(ILOpCodeValues.Ldc_I4_0);//7
            builder.Write(ILOpCodeValues.Br_S, 1); //8, 9
            builder.Write(ILOpCodeValues.Ldc_I4_1);//10
            builder.Write(ILOpCodeValues.Ret);//11
            var method = ILMethodBuilder<int>.Create(nameof(Bge_Test));
            method.Instructions = builder.Instructions;

            //Test break equal
            var frame = BuildAndExecute(builder.Instructions);
            frame.Stream = builder.Instructions;

            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
            var compiledResult = method.Compile().Invoke(null, new object[] { });
            AssertEmptyStackWithResult(frame, compiledResult);

            //Test branch greater
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
            compiledResult = method.Compile().Invoke(null, new object[] { });
            AssertEmptyStackWithResult(frame, compiledResult);


            //Test branch !(equal or greater)
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);
            compiledResult = method.Compile().Invoke(null, new object[] { });
            AssertEmptyStackWithResult(frame, compiledResult);


#if testcustom
            //Test branch custom equal
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldarg_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldarg_1);
            frame.Args = new object[] { 0, 0 };
            method.AddParameters(frame.Args.Select(x => x.GetType()).ToArray());

            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
            compiledResult = method.Compile().Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);

            //Test branch custom greater
            frame.Args[0] = new ComparisonTest(1);

            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
            compiledResult = method.Compile().Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);

            //Test branch custom !(equal or greater)
            frame.Args[0] = new ComparisonTest(0);
            frame.Args[1] = new ComparisonTest(1);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);
            compiledResult = method.Compile().Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);
#endif
        }

        [TestMethod()]
        public void Bge_Un_S_Test()
        {
            //test unsigned break equal
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Bge_Un_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);

            //Test unsigned break equal
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            //test unsigned break greater than
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            //Test unsigned break !(equal or greater)
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);


            //Test custom unsigned break equal
            frame.Args = new object[] { new ComparisonTestUn(unchecked((uint)int.MinValue)), new ComparisonTestUn(unchecked((uint)int.MinValue)) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Bge_Un_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test custom unsigned break greater
            frame.Args[1] = new ComparisonTestUn(unchecked((int)0));
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test custom unsigned!(equal or greater)
            frame.Args = new object[] { new ComparisonTestUn(unchecked((int)0)), new ComparisonTestUn(unchecked((uint)int.MinValue)) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);
        }

        [TestMethod()]
        public void Bge_Un_Test()
        {
            //test unsigned break equal
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Bge_Un, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);

            //Test unsigned break equal
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            //test unsigned break greater than
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            //Test unsigned break !(equal or greater)
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);


            //Test custom unsigned break equal
            frame.Args = new object[] { new ComparisonTestUn(unchecked((uint)int.MinValue)), new ComparisonTestUn(unchecked((uint)int.MinValue)) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Bge_Un, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test custom unsigned break greater
            frame.Args[1] = new ComparisonTestUn(unchecked((int)0));
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test custom unsigned!(equal or greater)
            frame.Args = new object[] { new ComparisonTestUn(unchecked((int)0)), new ComparisonTestUn(unchecked((uint)int.MinValue)) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

        }

        [TestMethod()]
        public void Bgt_S_Test()
        {
            //test break greater
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Bgt_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);


            //test break greater;
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);


            //test break equal;
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);

            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            //test break less;
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);

            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);


            //Test custom break greater
            frame.Args = new object[] { new ComparisonTest(1), new ComparisonTest(0) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Bgt_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test custom break equal
            frame.Args[0] = new ComparisonTest(0);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom break less
            frame.Args = new object[] { new ComparisonTest(0), new ComparisonTest(1) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);
        }

        [TestMethod()]
        public void Bgt_Test()
        {
            //test break greater
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Bgt, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);


            //test break greater;
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);


            //test break equal;
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);

            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            //test break less;
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);

            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);


            //Test custom break greater
            frame.Args = new object[] { new ComparisonTest(1), new ComparisonTest(0) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Bgt, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test custom break equal
            frame.Args[0] = new ComparisonTest(0);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom break less
            frame.Args = new object[] { new ComparisonTest(0), new ComparisonTest(1) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);
        }

        [TestMethod()]
        public void Bgt_Un_S_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ldc_I4, (object)0);
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Bgt_Un_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);

            //Test unsigned break greater
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            //test unsigned break equal
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            //Test unsigned break less
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);


            //Test custom unsigned break greater
            frame.Args = new object[] { new ComparisonTestUn(unchecked((uint)int.MinValue)), new ComparisonTestUn(unchecked((uint)0)) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Bgt_Un_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test custom unsigned break equal
            frame.Args[0] = new ComparisonTestUn(unchecked((int)0));
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom unsigned break less
            frame.Args = new object[] { new ComparisonTestUn(unchecked((int)0)), new ComparisonTestUn(unchecked((uint)int.MinValue)) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);
        }

        [TestMethod()]
        public void Bgt_Un_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ldc_I4, (object)0);
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Bgt_Un, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);

            //Test unsigned break greater
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            //test unsigned break equal
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            //Test unsigned break less
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);


            //Test custom unsigned break greater
            frame.Args = new object[] { new ComparisonTestUn(unchecked((uint)int.MinValue)), new ComparisonTestUn(unchecked((uint)0)) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Bgt_Un, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test custom unsigned break equal
            frame.Args[0] = new ComparisonTestUn(unchecked((int)0));
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom unsigned break less
            frame.Args = new object[] { new ComparisonTestUn(unchecked((int)0)), new ComparisonTestUn(unchecked((uint)int.MinValue)) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);
        }

        [TestMethod()]
        public void Ble_S_Test()
        {
            //test break greater
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Ble_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);


            //test break greater;
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);


            //test break equal;
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);

            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            //test break less;
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);

            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);


            //Test custom break greater
            frame.Args = new object[] { new ComparisonTest(1), new ComparisonTest(0) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Ble_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom break equal
            frame.Args[0] = new ComparisonTest(0);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test custom break less
            frame.Args = new object[] { new ComparisonTest(0), new ComparisonTest(1) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ble_Test()
        {
            //test break greater
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Ble, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);


            //test break greater;
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);


            //test break equal;
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);

            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            //test break less;
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);

            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);


            //Test custom break greater
            frame.Args = new object[] { new ComparisonTest(1), new ComparisonTest(0) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Ble, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom break equal
            frame.Args[0] = new ComparisonTest(0);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test custom break less
            frame.Args = new object[] { new ComparisonTest(0), new ComparisonTest(1) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ble_Un_S_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ldc_I4, (object)0);
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ble_Un_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);

            //Test unsigned break greater
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            //test unsigned break equal
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            //Test unsigned break less
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);


            //Test custom unsigned break greater
            frame.Args = new object[] { new ComparisonTestUn(unchecked((uint)int.MinValue)), new ComparisonTestUn(unchecked((uint)0)) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Ble_Un_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom unsigned break equal
            frame.Args[0] = new ComparisonTestUn(unchecked((int)0));
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test custom unsigned break less
            frame.Args = new object[] { new ComparisonTestUn(unchecked((int)0)), new ComparisonTestUn(unchecked((uint)int.MinValue)) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ble_Un_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ldc_I4, (object)0);
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ble_Un, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);

            //Test unsigned break greater
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            //test unsigned break equal
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            //Test unsigned break less
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);


            //Test custom unsigned break greater
            frame.Args = new object[] { new ComparisonTestUn(unchecked((uint)int.MinValue)), new ComparisonTestUn(unchecked((uint)0)) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Ble_Un, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom unsigned break equal
            frame.Args[0] = new ComparisonTestUn(unchecked((int)0));
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test custom unsigned break less
            frame.Args = new object[] { new ComparisonTestUn(unchecked((int)0)), new ComparisonTestUn(unchecked((uint)int.MinValue)) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Blt_S_Test()
        {
            //test break greater
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Blt_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);


            //test break greater;
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);


            //test break equal;
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);

            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            //test break less;
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);

            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);


            //Test custom break greater
            frame.Args = new object[] { new ComparisonTest(1), new ComparisonTest(0) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Blt_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom break equal
            frame.Args[0] = new ComparisonTest(0);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom break less
            frame.Args = new object[] { new ComparisonTest(0), new ComparisonTest(1) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Blt_Test()
        {
            //test break greater
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Blt, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);


            //test break greater;
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);


            //test break equal;
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);

            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            //test break less;
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);

            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);


            //Test custom break greater
            frame.Args = new object[] { new ComparisonTest(1), new ComparisonTest(0) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Blt, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom break equal
            frame.Args[0] = new ComparisonTest(0);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom break less
            frame.Args = new object[] { new ComparisonTest(0), new ComparisonTest(1) };
            Execute(frame);

            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Blt_Un_S_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ldc_I4, (object)0);
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Blt_Un_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);

            //Test unsigned break greater
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            //test unsigned break equal
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            //Test unsigned break less
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);


            //Test custom unsigned break greater
            frame.Args = new object[] { new ComparisonTestUn(unchecked((uint)int.MinValue)), new ComparisonTestUn(unchecked((uint)0)) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Blt_Un_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom unsigned break equal
            frame.Args[0] = new ComparisonTestUn(unchecked((int)0));
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom unsigned break less
            frame.Args = new object[] { new ComparisonTestUn(unchecked((int)0)), new ComparisonTestUn(unchecked((uint)int.MinValue)) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Blt_Un_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ldc_I4, (object)0);
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Blt_Un, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);

            //Test unsigned break greater
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            //test unsigned break equal
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            //Test unsigned break less
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);


            //Test custom unsigned break greater
            frame.Args = new object[] { new ComparisonTestUn(unchecked((uint)int.MinValue)), new ComparisonTestUn(unchecked((uint)0)) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Blt_Un, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom unsigned break equal
            frame.Args[0] = new ComparisonTestUn(unchecked((int)0));
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom unsigned break less
            frame.Args = new object[] { new ComparisonTestUn(unchecked((int)0)), new ComparisonTestUn(unchecked((uint)int.MinValue)) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Bne_Un_S_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ldc_I4, (object)0);
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Bne_Un_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);

            //Test unsigned break greater
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            //test unsigned break equal
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            //Test unsigned break less
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);


            //Test custom unsigned break greater
            frame.Args = new object[] { new ComparisonTestUn(unchecked((uint)int.MinValue)), new ComparisonTestUn(unchecked((uint)0)) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Bne_Un_S, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test custom unsigned break equal
            frame.Args[0] = new ComparisonTestUn(unchecked((int)0));
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom unsigned break less
            frame.Args = new object[] { new ComparisonTestUn(unchecked((int)0)), new ComparisonTestUn(unchecked((uint)int.MinValue)) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Bne_Un_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ldc_I4, (object)0);
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Bne_Un, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);

            //Test unsigned break greater
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);

            //test unsigned break equal
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

            //Test unsigned break less
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, unchecked((int)int.MinValue));
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);


            //Test custom unsigned break greater
            frame.Args = new object[] { new ComparisonTestUn(unchecked((uint)int.MinValue)), new ComparisonTestUn(unchecked((uint)0)) };
            builder.Clear();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Bne_Un, 3);
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Br_S, 1);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);
            frame.Stream = builder.Instructions;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //Test custom unsigned break equal
            frame.Args[0] = new ComparisonTestUn(unchecked((int)0));
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //Test custom unsigned break less
            frame.Args = new object[] { new ComparisonTestUn(unchecked((int)0)), new ComparisonTestUn(unchecked((uint)int.MinValue)) };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Box_Test()
        {
            var intType = typeof(int);
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Box, intType.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.SetResolver(intType);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);
        }

        [TestMethod()]
        public void Br_S_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_0);//0
            builder.Write(ILOpCodeValues.Ldc_I4_0);//1
            builder.Write(ILOpCodeValues.Beq_S, 3);//2,3
            builder.Write(ILOpCodeValues.Ldc_I4_0);//4
            builder.Write(ILOpCodeValues.Br_S, 1); //5, 6
            builder.Write(ILOpCodeValues.Ldc_I4_1);//7
            builder.Write(ILOpCodeValues.Ret);//8
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Br_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_0);//0
            builder.Write(ILOpCodeValues.Ldc_I4_0);//1
            builder.Write(ILOpCodeValues.Beq_S, 3);//2,3
            builder.Write(ILOpCodeValues.Ldc_I4_0);//4
            builder.Write(ILOpCodeValues.Br_S, 1); //5, 6
            builder.Write(ILOpCodeValues.Ldc_I4_1);//7
            builder.Write(ILOpCodeValues.Ret);//8
            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Break_Test()
        {
            var frame = Build(ILOpCodeValues.Break, ILOpCodeValues.Ret);
            frame.TriggerBreak = System.Diagnostics.Debugger.IsAttached;
            Execute(frame);
            AssertEmptyStackWithResult(frame, null);
        }

        [TestMethod()]
        public void BreakOnDebug_Test()
        {
            var frame = Build(ILOpCodeValues.Break, ILOpCodeValues.Ret);
            frame.TriggerBreak = System.Diagnostics.Debugger.IsAttached;
            var engine = NewEngine();
            Assert.IsTrue(engine.BreakOnDebug == false);
            engine.BreakOnDebug = true;
            Execute(frame);
            AssertEmptyStackWithResult(frame, null);
        }

        [TestMethod()]
        public void Brfalse_S_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);//0
            builder.Write(ILOpCodeValues.Brfalse_S, 3);//2,3
            builder.Write(ILOpCodeValues.Ldc_I4_0);//4
            builder.Write(ILOpCodeValues.Br_S, 1); //5, 6
            builder.Write(ILOpCodeValues.Ldc_I4_1);//7
            builder.Write(ILOpCodeValues.Ret);//8

            var frame = Build(builder.Instructions);

            //test break false 0;
            frame.Args = new object[] { 0 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //test break false 0;
            frame.Args[0] = false;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //test break false 0;
            frame.Args = new object[] { 1 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //test break false 0;
            frame.Args[0] = true;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            frame.Args[0] = new ILVariable();
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            frame.Args[0] = null;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Brfalse_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);//0
            builder.Write(ILOpCodeValues.Brfalse, 3);//2,3
            builder.Write(ILOpCodeValues.Ldc_I4_0);//4
            builder.Write(ILOpCodeValues.Br_S, 1); //5, 6
            builder.Write(ILOpCodeValues.Ldc_I4_1);//7
            builder.Write(ILOpCodeValues.Ret);//8

            var frame = Build(builder.Instructions);

            //test break false 0;
            frame.Args = new object[] { 0 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //test break false 0;
            frame.Args[0] = false;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //test break false 0;
            frame.Args = new object[] { 1 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //test break false 0;
            frame.Args[0] = true;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            frame.Args[0] = new ILVariable();
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            frame.Args[0] = null;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

        }

        [TestMethod()]
        public void Brtrue_S_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);//0
            builder.Write(ILOpCodeValues.Brtrue_S, 3);//2,3
            builder.Write(ILOpCodeValues.Ldc_I4_0);//4
            builder.Write(ILOpCodeValues.Br_S, 1); //5, 6
            builder.Write(ILOpCodeValues.Ldc_I4_1);//7
            builder.Write(ILOpCodeValues.Ret);//8

            var frame = Build(builder.Instructions);

            //test break false 0;
            frame.Args = new object[] { 0 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //test break false 0;
            frame.Args[0] = false;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //test break false 0;
            frame.Args = new object[] { 1 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //test break false 0;
            frame.Args[0] = true;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            frame.Args[0] = new ILVariable();
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            frame.Args[0] = null;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);
        }

        [TestMethod()]
        public void Brtrue_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);//0
            builder.Write(ILOpCodeValues.Brtrue, 3);//2,3
            builder.Write(ILOpCodeValues.Ldc_I4_0);//4
            builder.Write(ILOpCodeValues.Br_S, 1); //5, 6
            builder.Write(ILOpCodeValues.Ldc_I4_1);//7
            builder.Write(ILOpCodeValues.Ret);//8

            var frame = Build(builder.Instructions);

            //test break false 0;
            frame.Args = new object[] { 0 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //test break false 0;
            frame.Args[0] = false;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);

            //test break false 0;
            frame.Args = new object[] { 1 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            //test break false 0;
            frame.Args[0] = true;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            frame.Args[0] = new ILVariable();
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);

            frame.Args[0] = null;
            Execute(frame);
            AssertEmptyStackWithResult(frame, 0);
        }

        #endregion

        #region CallTests



        private void RunCallTest(ILOpCodeValues opCode)
        {
            var model = ILEngineUnitTestModel.InstanceField;
            var modelType = model.GetType();
            var setMethod = modelType.GetMethod(nameof(model.SetValue));
            Assert.IsNotNull(setMethod);
            var builder = new ILInstructionBuilder();


            //test instance void call
            builder.Write(ILOpCodeValues.Ldobj, model);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(opCode, setMethod.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.SetResolver(setMethod);
            Execute(frame);
            AssertEmptyStackWithResult(frame, null);
            Assert.IsTrue(model.Value == 1);



            //test instance method call
            var getMethod = modelType.GetMethod(nameof(model.GetValue));
            Assert.IsNotNull(getMethod);
            builder.Instructions.RemoveAt(1);
            builder.Instructions[1] = ILInstruction.Create(opCode, getMethod.MetadataToken);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);


            //test static void call
            model.Value = 0;
            builder.Instructions.RemoveAt(0);
            builder.Instructions.Insert(0, ILInstruction.Create(ILOpCodeValues.Ldc_I4_1));
            var setMethodStatic = modelType.GetMethod(nameof(model.SetValueStatic));
            Assert.IsNotNull(setMethodStatic);
            builder.Instructions[1] = ILInstruction.Create(opCode, setMethodStatic.MetadataToken);
            Execute(frame);
            AssertEmptyStackWithResult(frame, null);
            Assert.IsTrue(ILEngineUnitTestModel.InstanceField.Value == 1);

            //test static method call
            var getMethodStatic = modelType.GetMethod(nameof(model.GetValueStatic));
            Assert.IsNotNull(getMethodStatic);
            builder.Instructions.RemoveAt(0);
            builder.Instructions[0] = ILInstruction.Create(opCode, getMethodStatic.MetadataToken);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);


            builder.Instructions[0] = ILInstruction.Create(opCode, getMethodStatic);
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);


            // call default constructor
            var defaultCtor = modelType.GetConstructor(new Type[] { });
            Assert.IsNotNull(defaultCtor);
            builder.Instructions[0] = ILInstruction.Create(opCode, defaultCtor.MetadataToken);
            Execute(frame);
            AssertEmptyStackWithResult(frame, new ILEngineUnitTestModel());

            // call parameterized constructor
            var paramCtor = modelType.GetConstructor(new Type[] { typeof(int) });
            Assert.IsNotNull(paramCtor);
            builder.Instructions.Insert(0, ILInstruction.Create(ILOpCodeValues.Ldc_I4_1));
            builder.Instructions[1] = ILInstruction.Create(opCode, paramCtor.MetadataToken);
            Execute(frame);
            AssertEmptyStackWithResult(frame, new ILEngineUnitTestModel(1));
        }

        [TestMethod()]
        public void Call_Test()
        {
            RunCallTest(ILOpCodeValues.Call);
        }

        [TestMethod()]
        public void Calli_Test()
        {
            RunCallTest(ILOpCodeValues.Calli);
        }

        [TestMethod()]
        public void Callvirt_Test()
        {
            RunCallTest(ILOpCodeValues.Callvirt);
        }
      

        [TestMethod()]
        public void Castclass_Test()
        {

            //TODO: Happy path for now. An efficient full implementation of class conversion requires some heavy lifting.

            var sourceClass = new DerivedSource(1);


            var l = new List<DestClass>();
            l.Add(sourceClass);

            var builder = new ILInstructionBuilder();
            var destType = typeof(SourceClass);
            builder.Write(ILOpCodeValues.Ldobj, sourceClass);
            builder.Write(ILOpCodeValues.Castclass, destType.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);

            var frame = Build(builder.Instructions);
            frame.SetResolver(destType);
            Execute(frame);
            var expected = new DestClass(1);
            AssertEmptyStackWithResult(frame, expected);
        }

        #endregion


        #region CompareTests

        #region CompareHelpers
        public class ExpectedCompResults
        {
            public int WhenValuesAreEqual;
            public int WhenFirstValueIsGreater;
            public int WhenFirstValueIsLess;
        }


        List<ILOpCodeValues> LocalsSignedComparisonTestTemplate(ILOpCodeValues compOpCode)
            => (new[] { ILOpCodeValues.Ldc_I4_1, ILOpCodeValues.Ldc_I4_1, compOpCode, ILOpCodeValues.Ret }).ToList();

        List<ILOpCodeValues> LocalsUnsignedComparisonTestTemplate(ILOpCodeValues compOpCode)
            => (new[] { ILOpCodeValues.Ldc_I4_1, ILOpCodeValues.Conv_U4, ILOpCodeValues.Ldc_I4_1, ILOpCodeValues.Conv_U4, compOpCode, ILOpCodeValues.Ret }).ToList();


        void SignedComparisonTest(ILOpCodeValues compOp, ExpectedCompResults expected)
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1, ILOpCodeValues.Ldc_I4_1, compOp, ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            var method = ILMethodBuilder<int>.Create(nameof(Cgt_Test));
            method.Instructions = builder.Instructions;

            //test when equal
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenValuesAreEqual);
            var compiledResult = method.Compile().Invoke(null, new object[] { });
            AssertEmptyStackWithResult(frame, compiledResult);


            //test when greater
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_2);
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenFirstValueIsGreater);
            compiledResult = method.Compile().Invoke(null, new object[] { });
            AssertEmptyStackWithResult(frame, compiledResult);


            //test when less
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenFirstValueIsLess);
            compiledResult = method.Compile().Invoke(null, new object[] { });
            AssertEmptyStackWithResult(frame, compiledResult);


            //test with args setup

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldarg_0);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldarg_1);
            frame.Args = new object[] { 0, 0 };
            method.ParameterTypes = frame.Args.Select(x => x.GetType()).ToArray();
            var compiledMethod = method.Compile();




            //test with args when equal

            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenValuesAreEqual);
            compiledResult = compiledMethod.Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);

            //test with args when greater
            frame.Args[0] = 2;
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenFirstValueIsGreater);
            compiledResult = compiledMethod.Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);

            //test with args when less
            frame.Args[0] = 0;
            frame.Args[1] = 1;
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenFirstValueIsLess);
            compiledResult = compiledMethod.Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);



#if testcustom


            //test  user class when equal
            frame.Args = new object[] { new ComparisonTest(0), new ComparisonTest(0) };
            method.ParameterTypes = frame.Args.Select(x => x.GetType()).ToArray();
            compiledMethod = method.Compile();


            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenValuesAreEqual);
            compiledResult = compiledMethod.Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);

            //test user class when greater
            frame.Args[0] = new ComparisonTest(1);
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenFirstValueIsGreater);
            compiledResult = compiledMethod.Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);

            //test  user class when less
            frame.Args[0] = new ComparisonTest(0);
            frame.Args[1] = new ComparisonTest(1);
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenFirstValueIsLess);
            compiledResult = compiledMethod.Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);

#endif
        }


        void UnsignedComparisonTest(ILOpCodeValues opCode, ExpectedCompResults expected)
        {
            List<ILOpCodeValues> localsTestInstructions = LocalsUnsignedComparisonTestTemplate(opCode);
            var builder = new ILInstructionBuilder();
            builder.Write(localsTestInstructions.ToArray());
            var frame = Build(builder.Instructions);
            var method = ILMethodBuilder<int>.Create(nameof(Cgt_Test));
            method.Instructions = builder.Instructions;

            //test with locals when equal
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenValuesAreEqual);
            var compiledResult = method.Compile().Invoke(null, new object[] { });
            AssertEmptyStackWithResult(frame, compiledResult);


            //test with locals when greater
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_2);
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenFirstValueIsGreater);
            compiledResult = method.Compile().Invoke(null, new object[] { });
            AssertEmptyStackWithResult(frame, compiledResult);


            //test with locals when less
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_0);
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_1);
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenFirstValueIsLess);
            compiledResult = method.Compile().Invoke(null, new object[] { });
            AssertEmptyStackWithResult(frame, compiledResult);


            //test with args

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldarg_0);
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Ldarg_1);
            frame.Args = new object[] { 0, 0 };
            method.ParameterTypes = frame.Args.Select(x => x.GetType()).ToArray();
            var compiledMethod = method.Compile();




            //test args when equal

            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenValuesAreEqual);
            compiledResult = compiledMethod.Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);

            //test args when greater
            frame.Args[0] = 2;
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenFirstValueIsGreater);
            compiledResult = compiledMethod.Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);

            //test args when less
            frame.Args[0] = 0;
            frame.Args[1] = 1;
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenFirstValueIsLess);
            compiledResult = compiledMethod.Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);


#if testcustom
            //test user class when equal
            frame.Args = new object[] { new ComparisonTest(0), new ComparisonTest(0) };
            method.ParameterTypes = frame.Args.Select(x => x.GetType()).ToArray();
            compiledMethod = method.Compile();


            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenValuesAreEqual);
            compiledResult = compiledMethod.Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);

            //test user class when greater
            frame.Args[0] = new ComparisonTest(1);
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenFirstValueIsGreater);
            compiledResult = compiledMethod.Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);

            //tesh user class when less
            frame.Args[0] = new ComparisonTest(0);
            frame.Args[1] = new ComparisonTest(1);
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected.WhenFirstValueIsLess);
            compiledResult = compiledMethod.Invoke(null, frame.Args);
            AssertEmptyStackWithResult(frame, compiledResult);
#endif
        }

        #endregion

        [TestMethod()]
        public void Ceq_Test()
        {

            var frame = BuildAndExecute(ILOpCodeValues.Ldc_I4_1, ILOpCodeValues.Ldc_I4_1, ILOpCodeValues.Ceq, ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, 1);

            frame = BuildAndExecute(ILOpCodeValues.Ldc_I4_1, ILOpCodeValues.Ldc_I4_2, ILOpCodeValues.Ceq, ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, 0);

            var src1 = new DestClass(1);
            var src2 = new DestClass(1);
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldobj, src1);
            builder.Write(ILOpCodeValues.Ldobj, src2);
            builder.Write(ILOpCodeValues.Ceq);
            builder.Write(ILOpCodeValues.Ret);
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 1);
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldobj, new DestClass(2));
            frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, 0);

        }

        [TestMethod()]
        public void Cgt_Test()
        {
            var expected = new ExpectedCompResults
            {
                WhenValuesAreEqual = 0,
                WhenFirstValueIsGreater = 1,
                WhenFirstValueIsLess = 0
            };
            SignedComparisonTest(ILOpCodeValues.Cgt, expected);
        }

        [TestMethod()]
        public void Cgt_Un_Test()
        {

            var expected = new ExpectedCompResults
            {
                WhenValuesAreEqual = 0,
                WhenFirstValueIsGreater = 1,
                WhenFirstValueIsLess = 0
            };

            UnsignedComparisonTest(ILOpCodeValues.Cgt_Un, expected);

            var frame = BuildAndExecute(ILOpCodeValues.Ldc_I4_1, ILOpCodeValues.Conv_U4, ILOpCodeValues.Ldc_I4_1, ILOpCodeValues.Conv_U4, ILOpCodeValues.Cgt_Un, ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, 0);

            frame = BuildAndExecute(ILOpCodeValues.Ldc_I4_2, ILOpCodeValues.Conv_U4, ILOpCodeValues.Ldc_I4_1, ILOpCodeValues.Conv_U4, ILOpCodeValues.Cgt_Un, ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ckfinite_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_R4, float.NegativeInfinity);
            builder.Write(ILOpCodeValues.Ckfinite);
            builder.Write(ILOpCodeValues.Ret);
            var frame = BuildAndExecute(builder.Instructions);
            var expected = true;
            AssertEmptyStackWithResult(frame, expected);


            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_R4, float.PositiveInfinity);
            frame = BuildAndExecute(builder.Instructions);
            expected = true;
            AssertEmptyStackWithResult(frame, expected);


            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_R4, 0f);
            frame = BuildAndExecute(builder.Instructions);
            expected = false;
            AssertEmptyStackWithResult(frame, expected);


            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_R8, double.PositiveInfinity);
            frame = BuildAndExecute(builder.Instructions);
            expected = true;

            AssertEmptyStackWithResult(frame, expected);
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_R8, double.PositiveInfinity);
            frame = BuildAndExecute(builder.Instructions);
            expected = true;
            AssertEmptyStackWithResult(frame, expected);


            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_R8, 0.0d);
            frame = BuildAndExecute(builder.Instructions);
            expected = false;
            AssertEmptyStackWithResult(frame, expected);

        }

        [TestMethod()]
        public void Clt_Test()
        {
            var expected = new ExpectedCompResults
            {
                WhenValuesAreEqual = 0,
                WhenFirstValueIsGreater = 0,
                WhenFirstValueIsLess = 1
            };
            SignedComparisonTest(ILOpCodeValues.Clt, expected);
        }

        [TestMethod()]
        public void Clt_Un_Test()
        {

            var expected = new ExpectedCompResults
            {
                WhenValuesAreEqual = 0,
                WhenFirstValueIsGreater = 0,
                WhenFirstValueIsLess = 1
            };
            UnsignedComparisonTest(ILOpCodeValues.Clt_Un, expected);
        }
        #endregion

        [TestMethod()]
        public void Constrained_Test()
        {
            AssertOpCodeNotImplemented(ILInstruction.Create(ILOpCodeValues.Constrained, 1));
        }

        [TestMethod()]
        public void Conv_I1_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, int.MinValue);
            builder.Write(ILOpCodeValues.Conv_I1);
            builder.Write(ILOpCodeValues.Ret);
            var frame = BuildAndExecute(builder.Instructions);
            var expected = unchecked((sbyte)int.MinValue);
            AssertEmptyStackWithResult(frame, expected);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, int.MaxValue);
            frame = BuildAndExecute(builder.Instructions);
            expected = unchecked((sbyte)int.MaxValue);
            AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Conv_I2_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, int.MinValue);
            builder.Write(ILOpCodeValues.Conv_I2);
            builder.Write(ILOpCodeValues.Ret);
            var frame = BuildAndExecute(builder.Instructions);
            var expected = unchecked((short)int.MinValue);
            AssertEmptyStackWithResult(frame, expected);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, int.MaxValue);
            frame = BuildAndExecute(builder.Instructions);
            expected = unchecked((short)int.MaxValue);
            AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Conv_I4_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I8, long.MinValue);
            builder.Write(ILOpCodeValues.Conv_I4);
            builder.Write(ILOpCodeValues.Ret);
            var frame = BuildAndExecute(builder.Instructions);
            var expected = unchecked((int)long.MinValue);
            AssertEmptyStackWithResult(frame, expected);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I8, long.MaxValue);
            frame = BuildAndExecute(builder.Instructions);
            expected = unchecked((int)long.MaxValue);
            AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Conv_I8_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Conv_I8);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);


            frame.Args = new object[] { ulong.MaxValue };
            Execute(frame);
            var actual = frame.ReturnResult;
            var expected = unchecked((long)ulong.MaxValue);
            AssertEmptyStackWithResult(frame, expected);
        }


        [TestMethod()]
        public void Conv_I_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Conv_I);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Args = new object[] { ulong.MaxValue };
            Execute(frame);

            var actual = frame.ReturnResult;
            var expected = unchecked((int)ulong.MaxValue);
            AssertEmptyStackWithResult(frame, expected);

        }


        private void TestConversionWithOverflow(ILOpCodeValues opCodeValue)
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(opCodeValue);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);


            frame.Args = new object[] { 0 };
            Execute(frame);

            var actual = frame.ReturnResult;
            var expected = 0;
            AssertEmptyStackWithResult(frame, expected);

            frame.Args = new object[] { long.MaxValue };
            Execute(frame);
            AssertEmptyStackWithException(frame, typeof(OverflowException));

            frame.Args = new object[] { long.MinValue };
            Execute(frame);
            AssertEmptyStackWithException(frame, typeof(OverflowException));
        }

        private void TestConversionWithOverflowUnsigned(ILOpCodeValues opCodeValue)
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(opCodeValue);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);


            frame.Args = new object[] { 0 };
            Execute(frame);

            var actual = frame.ReturnResult;
            var expected = 0;
            AssertEmptyStackWithResult(frame, expected);

            frame.Args = new object[] { ulong.MaxValue };
            Execute(frame);
            AssertEmptyStackWithException(frame, typeof(OverflowException));


        }

        private void TestConversionUnsignedWithOverflow(ILOpCodeValues opCodeValue)
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(opCodeValue);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);


            frame.Args = new object[] { 0ul };
            Execute(frame);

            var actual = frame.ReturnResult;
            var expected = 0;
            AssertEmptyStackWithResult(frame, expected);

            frame.Args = new object[] { long.MinValue };
            Execute(frame);
            AssertEmptyStackWithException(frame, typeof(OverflowException));

            frame.Args = new object[] { long.MaxValue };
            Execute(frame);
            AssertEmptyStackWithException(frame, typeof(OverflowException));
        }

        private void TestConversionUnsignedWithOverflowUnsigned(ILOpCodeValues opCodeValue)
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(opCodeValue);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);


            frame.Args = new object[] { 0 };
            Execute(frame);

            var actual = frame.ReturnResult;
            var expected = 0;
            AssertEmptyStackWithResult(frame, expected);

            frame.Args = new object[] { ulong.MaxValue };
            Execute(frame);
            AssertEmptyStackWithException(frame, typeof(OverflowException));


        }

        [TestMethod()]
        public void Conv_Ovf_I1_Test()
        {
            TestConversionWithOverflow(ILOpCodeValues.Conv_Ovf_I1);
        }

        [TestMethod()]
        public void Conv_Ovf_I1_Un_Test()
        {
            TestConversionWithOverflowUnsigned(ILOpCodeValues.Conv_Ovf_I1_Un);
        }

        [TestMethod()]
        public void Conv_Ovf_I2_Test()
        {
            TestConversionWithOverflow(ILOpCodeValues.Conv_Ovf_I2);
        }

        [TestMethod()]
        public void Conv_Ovf_I2_Un_Test()
        {
            TestConversionWithOverflowUnsigned(ILOpCodeValues.Conv_Ovf_I2_Un);
        }
        [TestMethod()]
        public void Conv_Ovf_I4_Test()
        {
            TestConversionWithOverflow(ILOpCodeValues.Conv_Ovf_I4);
        }

        [TestMethod()]
        public void Conv_Ovf_I4_Un_Test()
        {
            TestConversionWithOverflowUnsigned(ILOpCodeValues.Conv_Ovf_I4_Un);
        }

        [TestMethod()]
        public void Conv_Ovf_I8_Test()
        {
            //Can't overflow an I8 with a native signed value, so test as unsigned
            TestConversionWithOverflowUnsigned(ILOpCodeValues.Conv_Ovf_I8);
        }

        [TestMethod()]
        public void Conv_Ovf_I8_Un_Test()
        {
            TestConversionWithOverflowUnsigned(ILOpCodeValues.Conv_Ovf_I8_Un);
        }


        [TestMethod()]
        public void Conv_Ovf_I_Test()
        {
            TestConversionWithOverflow(ILOpCodeValues.Conv_Ovf_I);
        }
        [TestMethod()]
        public void Conv_Ovf_I_Un_Test()
        {
            TestConversionWithOverflow(ILOpCodeValues.Conv_Ovf_I_Un);
        }

        [TestMethod()]
        public void Conv_Ovf_U1_Test()
        {
            TestConversionUnsignedWithOverflow(ILOpCodeValues.Conv_Ovf_U1);
        }

        [TestMethod()]
        public void Conv_Ovf_U1_Un_Test()
        {
            TestConversionUnsignedWithOverflowUnsigned(ILOpCodeValues.Conv_Ovf_U1_Un);
        }

        [TestMethod()]
        public void Conv_Ovf_U2_Test()
        {
            TestConversionUnsignedWithOverflow(ILOpCodeValues.Conv_Ovf_U2);
        }

        [TestMethod()]
        public void Conv_Ovf_U2_Un_Test()
        {
            TestConversionUnsignedWithOverflowUnsigned(ILOpCodeValues.Conv_Ovf_U2_Un);
        }

        [TestMethod()]
        public void Conv_Ovf_U4_Test()
        {
            TestConversionUnsignedWithOverflow(ILOpCodeValues.Conv_Ovf_U4);
        }

        [TestMethod()]
        public void Conv_Ovf_U4_Un_Test()
        {
            TestConversionUnsignedWithOverflowUnsigned(ILOpCodeValues.Conv_Ovf_U4_Un);
        }

        [TestMethod()]
        public void Conv_Ovf_U8_Test()
        {
            var opCodeValue = ILOpCodeValues.Conv_Ovf_U8;
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(opCodeValue);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);


            frame.Args = new object[] { 0ul };
            Execute(frame);

            var actual = frame.ReturnResult;
            var expected = 0ul;
            AssertEmptyStackWithResult(frame, expected);

            frame.Args = new object[] { long.MinValue };
            Execute(frame);
            AssertEmptyStackWithException(frame, typeof(OverflowException));
        }

        [TestMethod()]
        public void Conv_Ovf_U8_Un_Test()
        {
            //Can't overflow U8 with unsigned, so test with I8
            var opCodeValue = ILOpCodeValues.Conv_Ovf_U8_Un;
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(opCodeValue);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);


            frame.Args = new object[] { 0ul };
            Execute(frame);

            var actual = frame.ReturnResult;
            var expected = 0ul;
            AssertEmptyStackWithResult(frame, expected);

            frame.Args = new object[] { long.MinValue };
            Execute(frame);
            AssertEmptyStackWithException(frame, typeof(OverflowException));
        }

        [TestMethod()]
        public void Conv_Ovf_U_Test()
        {
            TestConversionUnsignedWithOverflow(ILOpCodeValues.Conv_Ovf_U);
        }

        [TestMethod()]
        public void Conv_Ovf_U_Un_Test()
        {
            TestConversionUnsignedWithOverflowUnsigned(ILOpCodeValues.Conv_Ovf_U_Un);
        }

        [TestMethod()]
        public void Conv_R4_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, int.MinValue);
            builder.Write(ILOpCodeValues.Conv_R4);
            builder.Write(ILOpCodeValues.Ret);
            var frame = BuildAndExecute(builder.Instructions);
            var expected = unchecked((float)int.MinValue);
            AssertEmptyStackWithResult(frame, expected);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, int.MaxValue);
            frame = BuildAndExecute(builder.Instructions);
            expected = unchecked((float)int.MaxValue);
            AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Conv_R8_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, int.MinValue);
            builder.Write(ILOpCodeValues.Conv_R8);
            builder.Write(ILOpCodeValues.Ret);
            var frame = BuildAndExecute(builder.Instructions);
            var expected = unchecked((double)int.MinValue);
            AssertEmptyStackWithResult(frame, expected);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I8, int.MaxValue);
            frame = BuildAndExecute(builder.Instructions);
            expected = unchecked((double)int.MaxValue);
            AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Conv_R_Un_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Conv_R_Un);
            builder.Write(ILOpCodeValues.Ret);

            var frame = Build(builder.Instructions);
            frame.Args = new object[] { uint.MaxValue };
            Execute(frame);

            var expected = unchecked((Single)uint.MaxValue);
            AssertEmptyStackWithResult(frame, expected);

        }

        [TestMethod()]
        public void Conv_U1_Test()
        {
            var builder = new ILInstructionBuilder();

            builder.Write(ILOpCodeValues.Ldc_I4, int.MinValue);
            builder.Write(ILOpCodeValues.Conv_U1);
            builder.Write(ILOpCodeValues.Ret);
            var frame = BuildAndExecute(builder.Instructions);
            var expected = unchecked((byte)int.MinValue);
            AssertEmptyStackWithResult(frame, expected);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, int.MaxValue);
            frame = BuildAndExecute(builder.Instructions);
            expected = unchecked((byte)int.MaxValue);
            AssertEmptyStackWithResult(frame, expected);

        }

        [TestMethod()]
        public void Conv_U2_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, int.MinValue);
            builder.Write(ILOpCodeValues.Conv_U2);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            Execute(frame);
            var expected = unchecked((ushort)int.MinValue);
            AssertEmptyStackWithResult(frame, expected);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, int.MaxValue);
            frame = BuildAndExecute(builder.Instructions);
            expected = unchecked((ushort)int.MaxValue);
            AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Conv_U4_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, int.MinValue);
            builder.Write(ILOpCodeValues.Conv_U4);
            builder.Write(ILOpCodeValues.Ret);
            var frame = BuildAndExecute(builder.Instructions);
            var expected = unchecked((uint)int.MinValue);
            AssertEmptyStackWithResult(frame, expected);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, int.MaxValue);
            frame = BuildAndExecute(builder.Instructions);
            expected = unchecked((uint)int.MaxValue);
            AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Conv_U8_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I8, long.MinValue);
            builder.Write(ILOpCodeValues.Conv_U8);
            builder.Write(ILOpCodeValues.Ret);
            var frame = BuildAndExecute(builder.Instructions);
            var expected = unchecked((ulong)long.MinValue);
            AssertEmptyStackWithResult(frame, expected);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I8, long.MaxValue);
            frame = BuildAndExecute(builder.Instructions);
            expected = unchecked((ulong)long.MaxValue);
            AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Conv_U_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4, int.MinValue);
            builder.Write(ILOpCodeValues.Conv_U);
            builder.Write(ILOpCodeValues.Ret);
            var frame = BuildAndExecute(builder.Instructions);
            var expected = unchecked((uint)int.MinValue);
            AssertEmptyStackWithResult(frame, expected);


            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I4, int.MaxValue);
            frame = BuildAndExecute(builder.Instructions);
            expected = unchecked((uint)int.MaxValue);
            AssertEmptyStackWithResult(frame, expected);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I8, int.MaxValue);
            frame = BuildAndExecute(builder.Instructions);
            var expected2 = unchecked((ulong)(long)int.MaxValue);
            AssertEmptyStackWithResult(frame, expected2);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I8, int.MinValue);
            frame = BuildAndExecute(builder.Instructions);
            expected2 = unchecked((ulong)(long)int.MinValue);
            AssertEmptyStackWithResult(frame, expected2);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I8, long.MaxValue);
            frame = BuildAndExecute(builder.Instructions);
             expected2 = unchecked((ulong)long.MaxValue);
            AssertEmptyStackWithResult(frame, expected2);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldc_I8, long.MinValue);
            frame = BuildAndExecute(builder.Instructions);
            expected2 = unchecked((ulong)long.MinValue);
            AssertEmptyStackWithResult(frame, expected2);





        }

        [TestMethod()]
        public void Cpblk_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Cpblk);
        }

        [TestMethod()]
        public void Cpobj_Test()
        {
            AssertOpCodeNotImplemented(ILInstruction.Create(ILOpCodeValues.Cpobj, 0));
        }

        [TestMethod()]
        public void Div_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Div);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Args = new object[] { 2.0, 3.5 };
            Execute(frame);
            var expected = 3.5 / 2.0;
            AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Div_Un_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Div_Un);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Args = new object[] { 2.0, 3.5 };
            Execute(frame);
            var expected = 3.5 / 2.0;
            AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Dup_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldobj, new[] { 1 });
            builder.Write(ILOpCodeValues.Dup);
            builder.Write(ILOpCodeValues.Pop);
            builder.Write(ILOpCodeValues.Ldlen);
            builder.Write(ILOpCodeValues.Ret);
            var frame = BuildAndExecute(builder.Instructions);
            var expected = 1;
            AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Endfilter_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Endfilter);
        }

        [TestMethod()]
        public void Endfinally_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Endfinally);
        }

        [TestMethod()]
        public void Exec_MSIL_I_Test()
        {
            AssertOpCodeNotImplemented(ILInstruction.Create(ILOpCodeValues.Exec_MSIL_I, 0));
        }

        [TestMethod()]
        public void Exec_MSIL_S_Test()
        {
            AssertOpCodeNotImplemented(ILInstruction.Create(ILOpCodeValues.Exec_MSIL_S, 0));
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod()]
        public void ExecuteFrameThrowException_Test()
        {

            var builder = new ILInstructionBuilder();
            var exType = typeof(ArgumentException);
            var ctor = exType.GetConstructor(Type.EmptyTypes);
            Assert.IsNotNull(ctor);
            builder.Write(ILOpCodeValues.Newobj, ctor.MetadataToken);
            builder.Write(ILOpCodeValues.Throw);
          
            var engine = NewEngine();
            engine.ThrowOnException = true;
            var frame = Build(builder.Instructions);
            frame.SetResolver(exType.Module);
            engine.ExecuteFrame(frame);
        }

        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod()]
        public void ExecuteFrameThrowNullReferenceException_Test()
        {

            var builder = new ILInstructionBuilder();
            var exType = typeof(NullReferenceException);
            builder.Write(ILOpCodeValues.Ldnull);
            builder.Write(ILOpCodeValues.Throw);

            var engine = NewEngine();
            engine.ThrowOnException = true;
            var frame = Build(builder.Instructions);
            frame.SetResolver(exType.Module);
            engine.ExecuteFrame(frame);
        }

        [TestMethod()]
        public void ExecuteFrame_Test()
        {
            var frame = BuildAndExecute(ILOpCodeValues.Ldc_I4_0, ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, 0);
        }

        [TestMethod()]
        public void ExecuteFrameWithoutReturn_Test()
        {
            var frame = BuildAndExecute(ILOpCodeValues.Nop);
            AssertEmptyStackWithException(frame, typeof(InvalidInstructionsException));
        }


        [ExpectedException(typeof(InvalidOpCodeException))]
        [TestMethod()]
        public virtual void ExecuteOpCode_Test()
        {
            var engine = NewEngine();
            engine.StackFrame = NewFrame();
            var frame = Build((ILOpCodeValues)300);
            //engine.ExecuteFrame(frame);
            //Assert.IsNotNull(engine.StackFrame.Exception, "Engine exception not set");
            //throw engine.StackFrame.Exception;
        }

        [TestMethod()]
        public void FlowControlTest()
        {
            var engine = NewEngine();
            Assert.IsTrue(engine.FlowControlTarget == ILStackFrameFlowControlTarget.NotImplemented);
            engine.FlowControlTarget = ILStackFrameFlowControlTarget.MoveNext;
            Assert.IsTrue(engine.FlowControlTarget == ILStackFrameFlowControlTarget.MoveNext);
        }

        [TestMethod()]
        public void Initblk_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Initblk);
        }

        [TestMethod()]
        public void Initobj_Test()
        {

            AssertOpCodeNotImplemented(ILInstruction.Create(ILOpCodeValues.Initobj, 0));

            //TODO: Implement ILOpCodeValues.Initobj

            //var builder = new ILInstructionBuilder();

            //var type = typeof(DestClass);
            //builder.Write(ILOpCodeValues.Ldloc_0);
            //builder.Write(ILOpCodeValues.Initobj, type.MetadataToken);
            //builder.Write(ILOpCodeValues.Ldloc_0);
            //builder.Write(ILOpCodeValues.Ret);
            //var frame = Build(builder.Instructions);
            //var loc = new ILVariable { Type = typeof(DestClass) };
            //frame.Locals = new[] { loc };
            //frame.SetResolver(type);
            //Execute(frame);
            //var expected = new DestClass();

            //AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Isinst_Test()
        {
            var derived = new DerivedSource(1);
            var sourceType = typeof(SourceClass);

            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldobj, derived);
            builder.Write(ILOpCodeValues.Isinst, sourceType.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.SetResolver(sourceType);

            var expected = true;
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected);

            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldobj, new SourceClass(0));
            Execute(frame);

            expected = true;
            AssertEmptyStackWithResult(frame, expected);


            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldobj, new DestClass(0));
            Execute(frame);

            expected = false;
            AssertEmptyStackWithResult(frame, expected);
        }


        [TestMethod()]
        public void Jmp_Test()
        {
            var frame = Build(ILInstruction.Create(ILOpCodeValues.Jmp, null));
            Execute(frame);
            AssertEmptyStackWithException(frame, typeof(OpCodeNotImplementedException));
        }

        [TestMethod()]
        public void Ldarg_0_Test()
        {
            var frame = Build(ILOpCodeValues.Ldarg_0, ILOpCodeValues.Ret);
            frame.Args = new object[] { 1 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ldarg_1_Test()
        {
            var frame = Build(ILOpCodeValues.Ldarg_1, ILOpCodeValues.Ret);
            frame.Args = new object[] { 0, 1 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ldarg_2_Test()
        {
            var frame = Build(ILOpCodeValues.Ldarg_2, ILOpCodeValues.Ret);
            frame.Args = new object[] { 0, 0, 1 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ldarg_3_Test()
        {
            var frame = Build(ILOpCodeValues.Ldarg_3, ILOpCodeValues.Ret);
            frame.Args = new object[] { 0, 0, 0, 1 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ldarg_S_Test()
        {
            var frame = Build(ILInstruction.Create(ILOpCodeValues.Ldarg_S, 3), ILInstruction.Create(ILOpCodeValues.Ret));
            frame.Args = new object[] { 0, 0, 0, 1 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ldarg_Test()
        {
            var frame = Build(ILInstruction.Create(ILOpCodeValues.Ldarga, 3), ILInstruction.Create(ILOpCodeValues.Ret));
            frame.Args = new object[] { 0, 0, 0, 1 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ldarga_S_Test()
        {
            var frame = Build(ILInstruction.Create(ILOpCodeValues.Ldarga_S, 3), ILInstruction.Create(ILOpCodeValues.Ret));
            frame.Args = new object[] { 0, 0, 0, 1 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ldarga_Test()
        {
            var frame = Build(ILInstruction.Create(ILOpCodeValues.Ldarga_S, 3), ILInstruction.Create(ILOpCodeValues.Ret));
            frame.Args = new object[] { 0, 0, 0, 1 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        void ExecuteAndAssertExpected(ILOpCodeValues opCode, object expected)
        {
            var frame = BuildAndExecute(opCode, ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, expected);
        }
        void ExecuteAndAssertExpected(ILInstruction instruction, object expected)
        {
            var frame = BuildAndExecute(instruction, ILInstruction.Create(ILOpCodeValues.Ret));
            AssertEmptyStackWithResult(frame, expected);
        }

        void ExecuteAndAssertExpected(ILInstruction[] instructions, object expected)
        {
            var frame = BuildAndExecute(instructions.Concat(new[] { ILInstruction.Create(ILOpCodeValues.Ret) }).ToArray());
            AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Ldc_I4_0_Test()
        {
            ExecuteAndAssertExpected(ILOpCodeValues.Ldc_I4_0, 0);
        }

        [TestMethod()]
        public void Ldc_I4_1_Test()
        {
            ExecuteAndAssertExpected(ILOpCodeValues.Ldc_I4_1, 1);
        }

        [TestMethod()]
        public void Ldc_I4_2_Test()
        {
            ExecuteAndAssertExpected(ILOpCodeValues.Ldc_I4_2, 2);
        }

        [TestMethod()]
        public void Ldc_I4_3_Test()
        {
            ExecuteAndAssertExpected(ILOpCodeValues.Ldc_I4_3, 3);
        }

        [TestMethod()]
        public void Ldc_I4_4_Test()
        {
            ExecuteAndAssertExpected(ILOpCodeValues.Ldc_I4_4, 4);
        }

        [TestMethod()]
        public void Ldc_I4_5_Test()
        {
            ExecuteAndAssertExpected(ILOpCodeValues.Ldc_I4_5, 5);

        }

        [TestMethod()]
        public void Ldc_I4_6_Test()
        {
            ExecuteAndAssertExpected(ILOpCodeValues.Ldc_I4_6, 6);
        }

        [TestMethod()]
        public void Ldc_I4_7_Test()
        {
            ExecuteAndAssertExpected(ILOpCodeValues.Ldc_I4_7, 7);
        }

        [TestMethod()]
        public void Ldc_I4_8_Test()
        {
            ExecuteAndAssertExpected(ILOpCodeValues.Ldc_I4_8, 8);
        }

        [TestMethod()]
        public void Ldc_I4_M1_Test()
        {
            ExecuteAndAssertExpected(ILOpCodeValues.Ldc_I4_M1, -1);
        }

        [TestMethod()]
        public void Ldc_I4_S_Test()
        {
            ExecuteAndAssertExpected(ILInstruction.Create(ILOpCodeValues.Ldc_I4_S, 1), 1);
        }

        [TestMethod()]
        public void Ldc_I4_Test()
        {
            ExecuteAndAssertExpected(ILInstruction.Create(ILOpCodeValues.Ldc_I4, 1), 1);
        }

        [TestMethod()]
        public void Ldc_I8_Test()
        {
            ExecuteAndAssertExpected(ILInstruction.Create(ILOpCodeValues.Ldc_I8, 1L), 1L);
        }

        [TestMethod()]
        public void Ldc_R4_Test()
        {
            ExecuteAndAssertExpected(ILInstruction.Create(ILOpCodeValues.Ldc_R4, 1f), 1f);
        }

        [TestMethod()]
        public void Ldc_R8_Test()
        {
            ExecuteAndAssertExpected(ILInstruction.Create(ILOpCodeValues.Ldc_R4, 1d), 1d);
        }

        void TestLoadElement<T>(ILInstruction instruction, T expected)
        {
            var builder = new ILInstructionBuilder();
            var arr = new T[] { expected };

            //builder.Write(ILOpCodeValues.Ldc_I4_1);// push array val on stack
            builder.Write(ILOpCodeValues.Ldobj, arr); //push array on the stack
            builder.Write(ILOpCodeValues.Ldc_I4_0); // push array index on stack

            builder.Write(instruction);
            builder.Write(ILOpCodeValues.Ret);

            var frame = Build(builder.Instructions);
            Execute(frame);
            AssertEmptyStackWithResult(frame, expected);
        }
        void TestLoadElement<T>(ILOpCodeValues opCode, T expected)
            => TestLoadElement<T>(ILInstruction.Create(opCode), expected);

        [TestMethod()]
        public void Ldelem_I1_Test()
        {
            TestLoadElement(ILOpCodeValues.Ldelem_I1, (sbyte)1);
        }

        [TestMethod()]
        public void Ldelem_I2_Test()
        {
            TestLoadElement(ILOpCodeValues.Ldelem_I2, (short)1);
        }

        [TestMethod()]
        public void Ldelem_I4_Test()
        {
            TestLoadElement(ILOpCodeValues.Ldelem_I4, 1);
        }

        [TestMethod()]
        public void Ldelem_I8_Test()
        {
            TestLoadElement(ILOpCodeValues.Ldelem_I8, 1L);
        }

        [TestMethod()]
        public void Ldelem_I_Test()
        {
            TestLoadElement(ILOpCodeValues.Ldelem_I, 1);
        }

        [TestMethod()]
        public void Ldelem_R4_Test()
        {
            TestLoadElement(ILOpCodeValues.Ldelem_R4, 1f);
        }

        [TestMethod()]
        public void Ldelem_R8_Test()
        {
            TestLoadElement(ILOpCodeValues.Ldelem_R8, 1d);
        }

        [TestMethod()]
        public void Ldelem_Ref_Test()
        {
            TestLoadElement(ILOpCodeValues.Ldelem_Ref, "test");
        }

        [TestMethod()]
        public void Ldelem_Test()
        {
            var t = typeof(string);
            TestLoadElement(ILInstruction.Create(ILOpCodeValues.Ldelem, t.MetadataToken), "test");
        }

        [TestMethod()]
        public void Ldelem_U1_Test()
        {
            TestLoadElement(ILOpCodeValues.Ldelem_U1, (byte)1);
        }

        [TestMethod()]
        public void Ldelem_U2_Test()
        {
            TestLoadElement(ILOpCodeValues.Ldelem_U2, (ushort)1);
        }


        [TestMethod()]
        public void Ldelem_U4_Test()
        {
            TestLoadElement(ILOpCodeValues.Ldelem_U4, 1u);
        }

        [TestMethod()]
        public void Ldelema_Test()
        {
            var t = typeof(string);
            TestLoadElement(ILInstruction.Create(ILOpCodeValues.Ldelema, t.MetadataToken), "test");
        }

        [TestMethod()]
        public void Ldfld_Test()
        {
            var c = new FieldTest(1);
            var t = c.GetType();

            var valueField = t.GetField(nameof(c.Value));

            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldobj, c);
            builder.Write(ILOpCodeValues.Ldfld, valueField.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Resolver = new ILInstructionResolver(t.Module);
            Execute(frame);
            AssertEmptyStackWithResult(frame, c.Value);

            c.Value = 2;
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldfld, valueField);
            Execute(frame);
            AssertEmptyStackWithResult(frame, c.Value);
        }

        [TestMethod()]
        public void Ldflda_Test()
        {
            var c = new FieldTest(1);
            var t = c.GetType();

            var valueField = t.GetField(nameof(c.Value));

            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldobj, c);
            builder.Write(ILOpCodeValues.Ldflda, valueField.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);

            var frame = Build(builder.Instructions);
            frame.Resolver = new ILInstructionResolver(t.Module);
            Execute(frame);
            AssertEmptyStackWithResult(frame, c.Value);

            c.Value = 2;
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldflda, valueField);
            Execute(frame);
            AssertEmptyStackWithResult(frame, c.Value);
        }

        [TestMethod()]
        public void Ldftn_Test()
        {
            var builder = new ILInstructionBuilder();

            var src = new SourceClass(1);
            var ftnTest = src.GetType().GetMethod(nameof(src.ftnTest));
            Assert.IsNotNull(ftnTest);
            builder.Write(ILOpCodeValues.Ldftn, ftnTest.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);

            var frame = Build(builder.Instructions);
            frame.Resolver = new ILInstructionResolver(ftnTest);
            Execute(frame);

            var expected = ftnTest.MethodHandle.GetFunctionPointer();
            AssertEmptyStackWithResult(frame, expected);

        }

        private void ExecuteLdindAndAssertExpected<T>(ILOpCodeValues opCode, T expected)
        {
            var instructions = new[] { ILInstruction.Create(ILOpCodeValues.Ldobj, expected), ILInstruction.Create(opCode) };
            ExecuteAndAssertExpected(instructions, expected);
        }

        [TestMethod()]
        public void Ldind_I1_Test()
        {
            ExecuteLdindAndAssertExpected(ILOpCodeValues.Ldind_I1, (sbyte)1);
        }

        [TestMethod()]
        public void Ldind_I2_Test()
        {
            ExecuteLdindAndAssertExpected(ILOpCodeValues.Ldind_I2, (short)1);
        }

        [TestMethod()]
        public void Ldind_I4_Test()
        {
            ExecuteLdindAndAssertExpected(ILOpCodeValues.Ldind_I4, (int)1);
        }

        [TestMethod()]
        public void Ldind_I8_Test()
        {
            ExecuteLdindAndAssertExpected(ILOpCodeValues.Ldind_I8, (long)1);
        }

        [TestMethod()]
        public void Ldind_I_Test()
        {
            ExecuteLdindAndAssertExpected(ILOpCodeValues.Ldind_I, (int)1);
        }

        [TestMethod()]
        public void Ldind_R4_Test()
        {
            ExecuteLdindAndAssertExpected(ILOpCodeValues.Ldind_R4, 1.0f);
        }

        [TestMethod()]
        public void Ldind_R8_Test()
        {
            ExecuteLdindAndAssertExpected(ILOpCodeValues.Ldind_R8, 1.0d);
        }

        [TestMethod()]
        public void Ldind_Ref_Test()
        {
            ExecuteLdindAndAssertExpected(ILOpCodeValues.Ldind_Ref, "test");
        }

        [TestMethod()]
        public void Ldind_U1_Test()
        {
            ExecuteLdindAndAssertExpected(ILOpCodeValues.Ldind_U1, (byte)1);
        }

        [TestMethod()]
        public void Ldind_U2_Test()
        {
            ExecuteLdindAndAssertExpected(ILOpCodeValues.Ldind_U2, (ushort)1);
        }

        [TestMethod()]
        public void Ldind_U4_Test()
        {
            ExecuteLdindAndAssertExpected(ILOpCodeValues.Ldind_U4, (uint)1);
        }

        [TestMethod()]
        public void Ldlen_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            var intType = typeof(int);
            builder.Write(ILOpCodeValues.Newarr, intType);
            builder.Write(ILOpCodeValues.Ldlen);
            builder.Write(ILOpCodeValues.Ret);

            var frame = Build(builder.Instructions);
            frame.Resolver = new ILInstructionResolver(intType.Module);
            Execute(frame);


            AssertEmptyStackWithResult(frame, 1);
        }

        #region LdLocTests

        [TestMethod()]
        public void Ldloc_0_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldloc_0);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Locals = new ILVariable[] { new ILVariable { Value = 1, Type = typeof(int) } };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ldloc_1_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldloc_1);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Locals = new ILVariable[] {
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 1, Type = typeof(int) }
            };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ldloc_2_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldloc_2);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Locals = new ILVariable[] {
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 1, Type = typeof(int) }
            };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ldloc_3_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldloc_3);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Locals = new ILVariable[] {
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 1, Type = typeof(int) }
            };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ldloc_S_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldloc_S, 3);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Locals = new ILVariable[] {
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 1, Type = typeof(int) }
            };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ldloc_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldloc, 3);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Locals = new ILVariable[] {
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 1, Type = typeof(int) }
            };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ldloca_S_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldloc_S, 3);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Locals = new ILVariable[] {
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 1, Type = typeof(int) }
            };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Ldloca_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldloca, 3);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Locals = new ILVariable[] {
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 1, Type = typeof(int) }
            };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        #endregion

        [TestMethod()]
        public void Ldnull_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldnull);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);

            Execute(frame);
            AssertEmptyStackWithResult(frame, null);
        }

        [TestMethod()]
        public void Ldobj_Test()
        {
            var ldObj = new { value = 1 };
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldobj, ldObj);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);

            Execute(frame);
            AssertEmptyStackWithResult(frame, ldObj);
        }

        [TestMethod()]
        public void Ldsfld_Test()
        {
            var c = new FieldTest(1);
            var t = c.GetType();

            var valueField = t.GetField(nameof(FieldTest.StaticValue));
            Assert.IsNotNull(valueField, $"Failed to resolve {nameof(FieldTest)}.{nameof(FieldTest.StaticValue)}");
            
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldsfld, valueField.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Resolver = new ILInstructionResolver(t.Module);
            Execute(frame);
            AssertEmptyStackWithResult(frame, FieldTest.StaticValue);

            FieldTest.StaticValue = 2;
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldsfld, valueField);
            Execute(frame);
            AssertEmptyStackWithResult(frame, FieldTest.StaticValue);
        }

        [TestMethod()]
        public void Ldsflda_Test()
        {
            var c = new FieldTest(1);
            var t = c.GetType();

            var valueField = t.GetField(nameof(FieldTest.StaticValue));
            Assert.IsNotNull(valueField, $"Failed to resolve {nameof(FieldTest)}.{nameof(FieldTest.StaticValue)}");

            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldsflda, valueField.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Resolver = new ILInstructionResolver(t.Module);
            Execute(frame);
            AssertEmptyStackWithResult(frame, FieldTest.StaticValue);

            FieldTest.StaticValue = 2;
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldsflda, valueField);
            Execute(frame);
            AssertEmptyStackWithResult(frame, FieldTest.StaticValue);
        }

        [TestMethod()]
        public void Ldstr_Test()
        {
            ExecuteAndAssertExpected(ILInstruction.Create(ILOpCodeValues.Ldstr, "test"), "test");

            Func<string> funcConstString = () => "test";
            var il = funcConstString.Method.GetMethodBody().GetILAsByteArray();
            var token = BitConverter.ToInt32(il, 1);


            var t = typeof(ILEngineCompiledTests);
            var resolver = new ILInstructionResolver(t.Module);



            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldstr, token);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Resolver = resolver;
            Execute(frame);
            AssertEmptyStackWithResult(frame, "test");
        }

        [TestMethod()]
        public void Ldtoken_Test()
        {

            var builder = new ILInstructionBuilder();

            var builderType = builder.GetType();
            var intType = typeof(int);

            var instructionsField = builderType.GetField(nameof(builder.Instructions));



            //Test resolve type token


            //Test resolve fieldinfo token
            builder.Write(ILOpCodeValues.Ldtoken, instructionsField.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);

            var frame = BuildAndExecute(builder.Instructions);
            AssertEmptyStackWithResult(frame, instructionsField.FieldHandle);

            frame.Resolver = new ILInstructionResolver(intType.Module);
            builder.Instructions[0] = ILInstruction.Create(ILOpCodeValues.Ldtoken, intType.MetadataToken);
            Execute(frame);
            //override the result to all for comparison
            frame.ReturnResult = ((RuntimeTypeHandle)frame.ReturnResult).Value;
            AssertEmptyStackWithResult(frame, intType.TypeHandle.Value);



        }

        [ExpectedException(typeof(NotImplementedException))]
        [TestMethod()]
        public void LdtokenInvalid_Test()
        {
            var builder = new ILInstructionBuilder();

            var builderType = builder.GetType();
            var intType = typeof(int);
            var method = intType.GetMethod(nameof(int.GetTypeCode));
            builder.Write(ILOpCodeValues.Ldtoken, method.MetadataToken);
            var frame = Build(builder.Instructions);
            frame.SetResolver(intType);
            Execute(frame);
            AssertEmptyStackWithException(frame, typeof(NotImplementedException));
            throw frame.Exception;
        }


        [TestMethod()]
        public void Ldvirtftn_Test()
        {
            var builder = new ILInstructionBuilder();

            var t = new SourceClass(1);
            var ftnTest = t.GetType().GetMethod(nameof(SourceClass.ftnTest));
            Assert.IsNotNull(ftnTest);
            builder.Write(ILOpCodeValues.Ldvirtftn, ftnTest.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);

            var frame = Build(builder.Instructions);
            frame.Resolver = new ILInstructionResolver(ftnTest);
            Execute(frame);

            var expected = ftnTest.MethodHandle.GetFunctionPointer();
            AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Leave_S_Test()
        {
            AssertOpCodeNotImplemented(ILInstruction.Create(ILOpCodeValues.Leave_S, 1));
        }

        [TestMethod()]
        public void Leave_Test()
        {
            AssertOpCodeNotImplemented(ILInstruction.Create(ILOpCodeValues.Leave, 1));
        }

        [TestMethod()]
        public void Localloc_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Localloc);
        }

        [TestMethod()]
        public void Mkrefany_Test()
        {
            AssertOpCodeNotImplemented(ILInstruction.Create(ILOpCodeValues.Mkrefany, 1));
        }

        [TestMethod()]
        public void Mul_Ovf_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Mul_Ovf);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);

            frame.Args = new object[] { 2, 2 };
            Execute(frame);
            var expected = 4;
            AssertEmptyStackWithResult(frame, expected);

            frame.Args = new object[] { int.MaxValue, int.MaxValue };
            Execute(frame);

            AssertEmptyStackWithException(frame, typeof(OverflowException));
        }

        [TestMethod()]
        public void Mul_Ovf_Un_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Mul_Ovf_Un);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);

            frame.Args = new object[] { 2u, 2u };
            Execute(frame);
            var expected = 4u;
            AssertEmptyStackWithResult(frame, expected);

            frame.Args = new object[] { uint.MaxValue, uint.MaxValue };
            Execute(frame);

            AssertEmptyStackWithException(frame, typeof(OverflowException));
        }

        [TestMethod()]
        public void Mul_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Mul);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);

            frame.Args = new object[] { 2, 2 };
            Execute(frame);
            var expectedSigned = 4;
            AssertEmptyStackWithResult(frame, expectedSigned);

            frame.Args = new object[] { int.MaxValue, int.MaxValue };
            Execute(frame);
            expectedSigned = unchecked(int.MaxValue * int.MaxValue);
            AssertEmptyStackWithResult(frame, expectedSigned);

            frame.Args = new object[] { 2u, 2u };
            Execute(frame);
            var expectedUnsigned = 4u;
            AssertEmptyStackWithResult(frame, expectedUnsigned);

            frame.Args = new object[] { uint.MaxValue, uint.MaxValue };
            Execute(frame);
            expectedUnsigned = unchecked(uint.MaxValue * uint.MaxValue);
            AssertEmptyStackWithResult(frame, expectedUnsigned);



        }

        [TestMethod()]
        public void Neg_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldarg_0);

            builder.Write(ILOpCodeValues.Neg);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);

            frame.Args = new object[] { 1 };
            Execute(frame);
            var expected = -1;
            AssertEmptyStackWithResult(frame, expected);

            frame.Args = new object[] { 1L };
            Execute(frame);
            var expectedL = -1L;
            AssertEmptyStackWithResult(frame, expectedL);
        }

        [TestMethod()]
        public void Newarr_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            var intType = typeof(int);
            builder.Write(ILOpCodeValues.Newarr, intType);
            builder.Write(ILOpCodeValues.Ret);

            var frame = Build(builder.Instructions);
            frame.Resolver = new ILInstructionResolver(intType.Module);
            Execute(frame);

            int[] expected = new[] { 0 };
            AssertEmptyStackWithResult(frame, expected);

            var uintType = typeof(uint);
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Newarr, uintType.MetadataToken);
            Execute(frame);
            var expected2 = new[] { 0u };
            AssertEmptyStackWithResult(frame, expected2);


        }

        [TestMethod()]
        public void Newobj_Test()
        {
            var builder = new ILInstructionBuilder();

            var objType = typeof(DestClass);
            var ctor = objType.GetConstructor(new[] { typeof(int) });
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Newobj, ctor.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);

            var frame = Build(builder.Instructions);
            frame.Resolver = new ILInstructionResolver(objType.Module);
            Execute(frame);

            var expected = new DestClass(1);
            AssertEmptyStackWithResult(frame, expected);
        }

        [TestMethod()]
        public void Nop_Test()
        {
            var frame = BuildAndExecute(ILOpCodeValues.Nop, ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, null);
        }

        [TestMethod()]
        public void NotImplemented_Test()
        {
            AssertOpCodeNotImplemented((ILOpCodeValues.Prefix2));
        }

        [TestMethod()]
        public void Not_Test()
        {
            var frame = BuildAndExecute(ILOpCodeValues.Ldc_I4_0, ILOpCodeValues.Not, ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, ~0);

            frame = BuildAndExecute(ILOpCodeValues.Ldc_I4_1, ILOpCodeValues.Not, ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, ~1);
        }

        [TestMethod()]
        public void Or_Test()
        {
            var frame = BuildAndExecute(ILOpCodeValues.Ldc_I4_0,
                ILOpCodeValues.Ldc_I4_0,
                ILOpCodeValues.Or,
                ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, 0 | 0);

            frame = BuildAndExecute(ILOpCodeValues.Ldc_I4_0,
                            ILOpCodeValues.Ldc_I4_1,
                            ILOpCodeValues.Or,
                            ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, 0 | 1);

            frame = BuildAndExecute(ILOpCodeValues.Ldc_I4_1,
                            ILOpCodeValues.Ldc_I4_0,
                            ILOpCodeValues.Or,
                            ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, 1 | 0);

            frame = BuildAndExecute(ILOpCodeValues.Ldc_I4_1,
                            ILOpCodeValues.Ldc_I4_1,
                            ILOpCodeValues.Or,
                            ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, 1 | 1);
        }

        [TestMethod()]
        public void Pop_Test()
        {
            var frame = BuildAndExecute(
                ILOpCodeValues.Ldc_I4_0,
                ILOpCodeValues.Ldc_I4_1,
                ILOpCodeValues.Pop,
                ILOpCodeValues.Ret);
            AssertEmptyStackWithResult(frame, 0);
        }


        [TestMethod()]
        public void Prefix1_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Prefix1);
        }

        [TestMethod()]
        public void Prefix2_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Prefix2);
        }

        [TestMethod()]
        public void Prefix3_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Prefix3);
        }

        [TestMethod()]
        public void Prefix4_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Prefix4);
        }

        [TestMethod()]
        public void Prefix5_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Prefix5);
        }

        [TestMethod()]
        public void Prefix6_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Prefix6);
        }

        [TestMethod()]
        public void Prefix7_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Prefix7);
        }

        [TestMethod()]
        public void Prefixref_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Prefixref);
        }

        [TestMethod()]
        public void Readonly_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Readonly);
        }

        [TestMethod()]
        public void Refanytype_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Refanytype);
        }

        [TestMethod()]
        public void Refanyval_Test()
        {
            AssertOpCodeNotImplemented(ILInstruction.Create(ILOpCodeValues.Refanyval, 1));
        }

        [TestMethod()]
        public void Rem_Test()
        {
            var frame = BuildAndExecute(ILOpCodeValues.Ldc_I4_2,
                                ILOpCodeValues.Ldc_I4_5,
                                ILOpCodeValues.Rem,
                                ILOpCodeValues.Ret
                                );
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Rem_Un_Test()
        {
            var frame = BuildAndExecute(
                                ILInstruction.Create(ILOpCodeValues.Ldobj, 2u),
                                ILInstruction.Create(ILOpCodeValues.Ldobj, 5u),
                                ILInstruction.Create(ILOpCodeValues.Rem_Un),
                                ILInstruction.Create(ILOpCodeValues.Ret)
                                 );
            AssertEmptyStackWithResult(frame, 1u);
        }

        [TestMethod()]
        public void Ret_Test()
        {
            var frame = Build(ILOpCodeValues.Ret);
            Execute(frame);
            AssertEmptyStackWithResult(frame, null);
        }

        [TestMethod()]
        public void Rethrow_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Rethrow);
        }

        [TestMethod()]
        public void Shl_Test()
        {
            var frame = BuildAndExecute(
                ILInstruction.Create(ILOpCodeValues.Ldc_I4_3),
                ILInstruction.Create(ILOpCodeValues.Ldc_I4_1),
                ILInstruction.Create(ILOpCodeValues.Shl),
                ILInstruction.Create(ILOpCodeValues.Ret)
                );
            AssertEmptyStackWithResult(frame, 3 << 1);
        }

        [TestMethod()]
        public void Shr_Test()
        {
            var frame = BuildAndExecute(
                ILInstruction.Create(ILOpCodeValues.Ldc_I4_3),
                ILInstruction.Create(ILOpCodeValues.Ldc_I4_1),
                ILInstruction.Create(ILOpCodeValues.Shr),
                ILInstruction.Create(ILOpCodeValues.Ret)
                );
            AssertEmptyStackWithResult(frame, 3 >> 1);
        }

        [TestMethod()]
        public void Shr_Un_Test()
        {
            var frame = BuildAndExecute(
                ILInstruction.Create(ILOpCodeValues.Ldobj, uint.MaxValue),
                ILInstruction.Create(ILOpCodeValues.Ldc_I4_1),
                ILInstruction.Create(ILOpCodeValues.Shr_Un),
                ILInstruction.Create(ILOpCodeValues.Ret)
                );
            AssertEmptyStackWithResult(frame, uint.MaxValue >> 1);
        }

        [TestMethod()]
        public void Sizeof_Test()
        {
            var frame = BuildAndExecute(
               ILInstruction.Create(ILOpCodeValues.Sizeof, typeof(int)),
               ILInstruction.Create(ILOpCodeValues.Ret)
               );
            AssertEmptyStackWithResult(frame, 4);
        }

        [TestMethod()]
        public void Starg_S_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Starg_S, 3);
            builder.Write(ILOpCodeValues.Ldarg_S, 3);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Args = new object[] { 0, 0, 0, 0 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Starg_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Starg, 3);
            builder.Write(ILOpCodeValues.Ldarg, 3);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Args = new object[] { 0, 0, 0, 0 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        void TestStoreElement<T>(ILOpCodeValues opCode, T expected)
            => TestStoreElement(ILInstruction.Create(opCode), expected);

        void TestStoreElement<T>(ILInstruction instruction, T expected)
        {


            var builder = new ILInstructionBuilder();
            var arr = new T[] { default(T) };
            var expectedArr = new[] { expected };

            builder.Write(ILOpCodeValues.Ldobj, arr); //push array on the stack
            builder.Write(ILOpCodeValues.Ldc_I4_0);// push index
            builder.Write(ILOpCodeValues.Ldobj, expected); // push el             
            builder.Write(instruction);     // execute store element
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            Execute(frame);

            Assert.IsNull(frame.ReturnResult);
            frame.ReturnResult = arr;

            AssertEmptyStackWithResult(frame, expectedArr);

        }




        [TestMethod()]
        public void Stelem_I_Test()
        {
            TestStoreElement(ILOpCodeValues.Stelem_I, (int)1);
        }

        [TestMethod()]
        public void Stelem_I1_Test()
        {
            TestStoreElement(ILOpCodeValues.Stelem_I1, (sbyte)1);
        }

        [TestMethod()]
        public void Stelem_I2_Test()
        {
            TestStoreElement(ILOpCodeValues.Stelem_I2, (short)1);
        }

        [TestMethod()]
        public void Stelem_I4_Test()
        {
            TestStoreElement(ILOpCodeValues.Stelem_I4, (int)1);
        }

        [TestMethod()]
        public void Stelem_I8_Test()
        {
            TestStoreElement(ILOpCodeValues.Stelem_I8, (long)1);
        }

        [TestMethod()]
        public void Stelem_R4_Test()
        {
            TestStoreElement(ILOpCodeValues.Stelem_R4, 1.1f);
        }

        [TestMethod()]
        public void Stelem_R8_Test()
        {
            TestStoreElement(ILOpCodeValues.Stelem_R8, 1.1d);
        }

        [TestMethod()]
        public void Stelem_Ref_Test()
        {
            TestStoreElement(ILOpCodeValues.Stelem_Ref, "test");
        }

        [TestMethod()]
        public void Stelem_Test()
        {
            TestStoreElement(ILInstruction.Create(ILOpCodeValues.Stelem, typeof(string).MetadataToken), "test");
        }

        [TestMethod()]
        public void Stfld_Test()
        {
            var c = new FieldTest(0);
            var t = c.GetType();

            var valueField = t.GetField(nameof(c.Value));

            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldobj, c);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Stfld, valueField.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);

            //Test resolving and setting field via meta data token
            var frame = Build(builder.Instructions);
            frame.Resolver = new ILInstructionResolver(t.Module);
            Execute(frame);
            Assert.IsTrue(c.Value == 1);
            AssertEmptyStackWithResult(frame, null);

            c.Value = 0;
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_2);
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Stfld, valueField);
            Execute(frame);

            //Test resolving and setting field via FieldInfo reference
            Assert.IsTrue(c.Value == 2);
            AssertEmptyStackWithResult(frame, null);
        }

        [TestMethod()]
        public void Stind_I1_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Stind_I1);
        }

        [TestMethod()]
        public void Stind_I2_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Stind_I2);
        }

        [TestMethod()]
        public void Stind_I4_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Stind_I4);
        }

        [TestMethod()]
        public void Stind_I8_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Stind_I8);
        }

        [TestMethod()]
        public void Stind_I_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Stind_I);
        }

        [TestMethod()]
        public void Stind_R4_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Stind_R4);
        }

        [TestMethod()]
        public void Stind_R8_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Stind_R8);
        }

        [TestMethod()]
        public void Stind_Ref_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Stind_Ref);
        }

        [TestMethod()]
        public void Stloc_0_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Stloc_0);
            builder.Write(ILOpCodeValues.Ldloc_0);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Locals = new ILVariable[] {
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) }
            };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Stloc_1_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Stloc_1);
            builder.Write(ILOpCodeValues.Ldloc_1);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Locals = new ILVariable[] {
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) }
            };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Stloc_2_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Stloc_2);
            builder.Write(ILOpCodeValues.Ldloc_2);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Locals = new ILVariable[] {
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) }
            };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Stloc_3_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Stloc_3);
            builder.Write(ILOpCodeValues.Ldloc_3);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Locals = new ILVariable[] {
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) }
            };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Stloc_S_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Stloc_S, 3);
            builder.Write(ILOpCodeValues.Ldloc_S, 3);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Locals = new ILVariable[] {
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) }
            };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }

        [TestMethod()]
        public void Stloc_Test()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Stloc, 3);
            builder.Write(ILOpCodeValues.Ldloc, 3);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.Locals = new ILVariable[] {
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) },
                new ILVariable { Value = 0, Type = typeof(int) }
            };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 1);
        }


        [TestMethod()]
        public void Stobj_Test()
        {
            AssertOpCodeNotImplemented(ILInstruction.Create(ILOpCodeValues.Stobj, 1));
        }


        [TestMethod()]
        public void Stsfld_Test()
        {
            var c = new FieldTest(0);
            var t = c.GetType();

            var valueField = t.GetField(nameof(c.Value));

            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldobj, c);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Stsfld, valueField.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);

            //Test resolving and setting field via meta data token
            var frame = Build(builder.Instructions);
            frame.Resolver = new ILInstructionResolver(t.Module);
            Execute(frame);
            Assert.IsTrue(c.Value == 1);
            AssertEmptyStackWithResult(frame, null);

            c.Value = 0;
            builder.Instructions[1] = ILInstruction.Create(ILOpCodeValues.Ldc_I4_2);
            builder.Instructions[2] = ILInstruction.Create(ILOpCodeValues.Stsfld, valueField);
            Execute(frame);

            //Test resolving and setting field via FieldInfo reference
            Assert.IsTrue(c.Value == 2);
            AssertEmptyStackWithResult(frame, null);
        }

        [TestMethod()]
        public void Sub_Ovf_Test()
        {
            var frame = BuildAndExecute(
              ILInstruction.Create(ILOpCodeValues.Ldc_I4_1),
              ILInstruction.Create(ILOpCodeValues.Ldobj, int.MaxValue),
              ILInstruction.Create(ILOpCodeValues.Sub_Ovf),
              ILInstruction.Create(ILOpCodeValues.Ret)
              );
            AssertEmptyStackWithResult(frame, int.MaxValue - 1);

            frame = Build(
                ILInstruction.Create(ILOpCodeValues.Ldc_I4_1),
                ILInstruction.Create(ILOpCodeValues.Ldobj, int.MinValue),
                ILInstruction.Create(ILOpCodeValues.Sub_Ovf),
                ILInstruction.Create(ILOpCodeValues.Ret)
                );
            Execute(frame);
            AssertEmptyStackWithException(frame, typeof(OverflowException));
        }

        [TestMethod()]
        public void Sub_Ovf_Un_Test()
        {
            var frame = BuildAndExecute(
              ILInstruction.Create(ILOpCodeValues.Ldobj, 1u),
              ILInstruction.Create(ILOpCodeValues.Ldobj, uint.MaxValue),
              ILInstruction.Create(ILOpCodeValues.Sub_Ovf_Un),
              ILInstruction.Create(ILOpCodeValues.Ret)
              );
            AssertEmptyStackWithResult(frame, uint.MaxValue - 1);

            frame = BuildAndExecute(
                ILInstruction.Create(ILOpCodeValues.Ldobj, 1u),
                ILInstruction.Create(ILOpCodeValues.Ldobj, uint.MinValue),
                ILInstruction.Create(ILOpCodeValues.Sub_Ovf_Un),
                ILInstruction.Create(ILOpCodeValues.Ret)
                );
            AssertEmptyStackWithException(frame, typeof(OverflowException));
        }

        [TestMethod()]
        public void Sub_Test()
        {

            var frame = BuildAndExecute(
                ILInstruction.Create(ILOpCodeValues.Ldc_I4, int.MaxValue),
                ILInstruction.Create(ILOpCodeValues.Ldc_I4_1),
                ILInstruction.Create(ILOpCodeValues.Sub),
                ILInstruction.Create(ILOpCodeValues.Ret)
           );
            AssertEmptyStackWithResult(frame, int.MaxValue - 1);

            frame = BuildAndExecute(
                ILInstruction.Create(ILOpCodeValues.Ldc_I4, int.MinValue),
                ILInstruction.Create(ILOpCodeValues.Ldc_I4_1),
                ILInstruction.Create(ILOpCodeValues.Sub),
                ILInstruction.Create(ILOpCodeValues.Ret)
                );
            AssertEmptyStackWithResult(frame, unchecked(int.MinValue - 1));

            frame = BuildAndExecute(
                ILInstruction.Create(ILOpCodeValues.Ldc_I4, unchecked((int)uint.MaxValue)),
                ILInstruction.Create(ILOpCodeValues.Conv_U4),
                ILInstruction.Create(ILOpCodeValues.Ldobj, 1u),
                ILInstruction.Create(ILOpCodeValues.Sub),
                ILInstruction.Create(ILOpCodeValues.Ret)
                );
            AssertEmptyStackWithResult(frame, uint.MaxValue - 1);

            frame = BuildAndExecute(
                ILInstruction.Create(ILOpCodeValues.Ldc_I4, unchecked((int)uint.MinValue)),
                ILInstruction.Create(ILOpCodeValues.Conv_U4),
                ILInstruction.Create(ILOpCodeValues.Ldobj, 1u),
                ILInstruction.Create(ILOpCodeValues.Sub),
                ILInstruction.Create(ILOpCodeValues.Ret)
                );
            AssertEmptyStackWithResult(frame, unchecked(uint.MinValue - 1));
        }

        [TestMethod()]
        public void Switch_Test()
        {
            // create a target for each case statement to jump to when completed.
            var endSwitchInstruction = ILInstruction.Create(ILOpCodeValues.Nop);
            // assign it a unique label number.
            endSwitchInstruction.Label = 2;

            //create the case instuctions for add operand
            //and jump to endSwitchInstruction instruction when done.
            var addInstructions = new[]
            {
                ILInstruction.Create(ILOpCodeValues.Add),
                ILInstruction.Create(ILOpCodeValues.Br_S, endSwitchInstruction)
            };

            //create the case instuctions for sub operand
            //and jump to endSwitchInstruction instruction when done.
            var subInstructions = new[]
            {
                ILInstruction.Create(ILOpCodeValues.Sub),
                ILInstruction.Create(ILOpCodeValues.Br_S, endSwitchInstruction)
            };

            //assign a unique label id for the add instruction as a jump target for the switch statement.
            addInstructions[0].Label = 0;

            //assign a unique label id for the sub instruction as a jump target for the switch statement.
            subInstructions[0].Label = 1;


            //create default case instructions to throw an argument out of range exception.
            var exceptionType = typeof(ArgumentOutOfRangeException);
            var ctor = exceptionType.GetConstructor(Type.EmptyTypes);
            var defaultInstuctions = new[]
            {
                 ILInstruction.Create(ILOpCodeValues.Newobj, ctor.MetadataToken),
                 ILInstruction.Create(ILOpCodeValues.Throw)
            };

            // create the switch statement.
            // specifiying the add target for case 0:
            // specify the sub target for case 1:
            var switchInstuction = ILInstruction.Create(ILOpCodeValues.Switch,
                new[] { addInstructions[0], subInstructions[0] });


            //write the instructions.
            var builder = new ILInstructionBuilder();

            //ld arg1 and arg2 as operands.
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Ldarg_2);

            //load arg[0]
            builder.Write(ILOpCodeValues.Ldarg_0);

            //switch(arg[0])
            builder.Write(switchInstuction);

            //case default
            builder.Write(defaultInstuctions);
            //case 0: add
            builder.Write(addInstructions);
            //case 1: sub
            builder.Write(subInstructions);
            //write the end switch jump target
            builder.Write(endSwitchInstruction);
            //write the final return statement so we can compile to native MSIL.
            builder.Write(ILOpCodeValues.Ret);


            //build the instructions and run tests.
            var frame = ILStackFrameBuilder.Build(builder.Instructions);
            frame.Args = new object[] { 0, 1, 2 };
            frame.Execute();

            var expected = 3;
            Assert.IsNull(frame.Exception, $"Executing switch: add throw an exception {frame?.Exception}");
            Assert.IsTrue(frame.Stack.Count == 0, "Stack was not cleared executing switch: add");
            Assert.IsTrue((int)frame.ReturnResult == expected, $"Actual: {frame.ReturnResult}\r\nExpected: {expected}");

            expected = -1;
            frame.Args = new object[] { 1, 1, 2 };
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Executing switch: add throw an exception {frame?.Exception}");
            Assert.IsTrue(frame.Stack.Count == 0, "Stack was not cleared executing switch: add");
            Assert.IsTrue((int)frame.ReturnResult == expected, $"Actual: {frame.ReturnResult}\r\nExpected: {expected}");

            frame.Args = new object[] { 2, 1, 2 };
            frame.Execute();
            Assert.IsNotNull(frame.Exception, $"Executing switch failed to execute default case to and throw and exception.");
            Assert.IsInstanceOfType(frame.Exception, typeof(ArgumentOutOfRangeException), $"Frame failed to throw {nameof(ArgumentOutOfRangeException)}");
            Assert.IsNull(frame.ReturnResult, $"Actual: {frame.ReturnResult}\r\nExpected: [null]");
        }


        [TestMethod()]
        public void Tailcall_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Tailcall);
        }

        [TestMethod()]
        public void Throw_Test()
        {
            var expectionType = typeof(InvalidOperationException);
            var frame = BuildAndExecute(
                ILInstruction.Create(ILOpCodeValues.Ldobj, new InvalidOperationException()),
                ILInstruction.Create(ILOpCodeValues.Throw)
                );
            Assert.IsNotNull(frame.Exception);
            AssertEmptyStackWithException(frame, typeof(InvalidOperationException));
        }

        [TestMethod()]
        public void Unaligned_Test()
        {
            AssertOpCodeNotImplemented(ILInstruction.Create(ILOpCodeValues.Unaligned, 1));
        }

        [TestMethod()]
        public void Unbox_Any_Test()
        {
            var builder = new ILInstructionBuilder();
            var stringType = typeof(string);
            builder.Write(ILOpCodeValues.Ldobj, "test");
            builder.Write(ILOpCodeValues.Unbox_Any, stringType.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.SetResolver(stringType);
            Execute(frame);
            AssertEmptyStackWithResult(frame, "test");
        }

        [TestMethod()]
        public void Unbox_Test()
        {
            var builder = new ILInstructionBuilder();
            var stringType = typeof(string);
            builder.Write(ILOpCodeValues.Ldobj, "test");
            builder.Write(ILOpCodeValues.Unbox, stringType.MetadataToken);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);
            frame.SetResolver(stringType);
            Execute(frame);
            AssertEmptyStackWithResult(frame, "test");
        }

        [TestMethod()]
        public void Volatile_Test()
        {
            AssertOpCodeNotImplemented(ILOpCodeValues.Volatile);
        }

        [TestMethod()]
        public void Xor_Test()
        {
            var builder = new ILInstructionBuilder();

            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(ILOpCodeValues.Ldarg_1);
            builder.Write(ILOpCodeValues.Xor);
            builder.Write(ILOpCodeValues.Ret);
            var frame = Build(builder.Instructions);

            var expected = 1;

            frame.Args = new object[] { 1, 0 };
            Execute(frame);
            expected = 1;
            AssertEmptyStackWithResult(frame, expected);

            frame.Args = new object[] { 0, 1 };
            Execute(frame);
            expected = 1;
            AssertEmptyStackWithResult(frame, expected);


            frame.Args = new object[] { 1, 1 };
            Execute(frame);
            expected = 0;
            AssertEmptyStackWithResult(frame, expected);

            frame.Args = new object[] { 0, 0 };
            Execute(frame);
            expected = 0;
            AssertEmptyStackWithResult(frame, expected);


        }
    }
}