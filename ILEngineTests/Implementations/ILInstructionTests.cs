using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using ILEngine.Compilers;
using ILEngine.Models;
using ILEngine.Tests;
using System.Reflection;
using System.Threading;

namespace ILEngine.Implementations.Tests
{
    [TestClass()]
    public class ILInstructionTests
    {
        [TestMethod()]
        public void GetValidOpCodeMetaValidTest()
        {
            var meta = ILInstruction.GetOpCodeMeta(OpCodes.Nop.Value);
            Assert.IsNotNull(meta);
            Assert.IsTrue(meta.OpCode == "nop", $"Expected meta.OpCode ='nop'. Actual {meta.OpCode}");
            Assert.IsTrue(meta.OperandTypeByteSize == 0, $"Expected meta.OperandTypeByteSize == 0. Actual {meta.OperandTypeByteSize}");
        }

        [ExpectedException(typeof(InvalidOpCodeException))]
        [TestMethod()]
        public void GetInvalidOpCodeMetaValidTest()
        {
            var meta = ILInstruction.GetOpCodeMeta(300);
        }


        [TestMethod()]
        public void RequireNullOperandPositiveTest()
        {
            ILInstruction.RequireNullOperand(OpCodes.Nop.Value);
        }

        [ExpectedException(typeof(ILInstructionArgumentException))]
        [TestMethod()]
        public void RequireNullOperandNegativeTest()
        {
            ILInstruction.RequireNullOperand(OpCodes.Ldarga.Value);
        }

        [TestMethod()]
        public void RequireOperandPositiveTest()
        {
            ILInstruction.RequireOperand(OpCodes.Ldarga.Value);
        }

        [ExpectedException(typeof(ILInstructionArgumentException))]
        [TestMethod()]
        public void RequireOperandNegativeTest()
        {
            ILInstruction.RequireOperand(OpCodes.Nop.Value);
        }

        [TestMethod()]
        public void CreateFromILOpCodeValuesTest()
        {
            var noOp = ILInstruction.Create(ILOpCodeValues.Nop);
            Assert.IsNotNull(noOp);
            Assert.IsTrue(noOp.OpCode == OpCodes.Nop);
            Assert.IsNull(noOp.Arg);

        }

        [TestMethod()]
        public void CreateFromILOpCodeValuesWithArgTest()
        {
            var arg = 0;
            var ldarga_S = ILInstruction.Create(ILOpCodeValues.Ldarga_S, arg);
            Assert.IsNotNull(ldarga_S);
            Assert.IsTrue(ldarga_S.OpCode == OpCodes.Ldarga_S);
            Assert.IsNotNull(ldarga_S.Arg);
            Assert.IsTrue((int)ldarga_S.Arg == arg);
        }

        [ExpectedException(typeof(ILInstructionArgumentException))]
        [TestMethod()]
        public void CreateFromILOpCodeValuesFailsWhenSpecifyingArgumentTest()
        {
            var noOp = ILInstruction.Create(ILOpCodeValues.Nop, 0);
        }

        [ExpectedException(typeof(ILInstructionArgumentException))]
        [TestMethod()]
        public void CreateFromILOpCodeValuesFailsWhenNotSpecifyingRequiredArgumentTest()
        {
            var noOp = ILInstruction.Create(ILOpCodeValues.Ldarga_S);
        }


        [TestMethod()]
        public void CreateFromOpCodeTest()
        {
            var noOp = ILInstruction.Create(OpCodes.Nop);
            Assert.IsNotNull(noOp);
            Assert.IsTrue(noOp.OpCode == OpCodes.Nop);
            Assert.IsNull(noOp.Arg);
        }

        [TestMethod()]
        public void CreateFromOpCodeWithArgTest()
        {
            var arg = 0;
            var ldarga_S = ILInstruction.Create(OpCodes.Ldarga_S, arg);
            Assert.IsNotNull(ldarga_S);
            Assert.IsTrue(ldarga_S.OpCode == OpCodes.Ldarga_S);
            Assert.IsNotNull(ldarga_S.Arg);
            Assert.IsTrue((int)ldarga_S.Arg == arg);
        }

        [ExpectedException(typeof(ILInstructionArgumentException))]
        [TestMethod()]
        public void CreateFromOpCodeFailsWhenSpecifyingArgumentTest()
        {
            var noOp = ILInstruction.Create(OpCodes.Nop, 0);
        }

        [ExpectedException(typeof(ILInstructionArgumentException))]
        [TestMethod()]
        public void CreateFromOpCodeFailsWhenNotSpecifyingRequiredArgumentTest()
        {
            var noOp = ILInstruction.Create(OpCodes.Ldarga_S);
        }



        [TestMethod()]
        public void ToStringTest()
        {
            var instruction1 = ILInstruction.Create(OpCodes.Nop);
            Assert.IsNotNull(instruction1);
            var expected1 = $"IL_0000 nop";
            Assert.IsTrue(instruction1.ToString() == expected1, $"Actual: {instruction1.ToString()}\r\nExpected:{expected1}");

            var instruction2 = ILInstruction.Create(OpCodes.Ldarga_S, 1);
            Assert.IsNotNull(instruction2);
            var expected2 = $"IL_0000 ldarga.s 1";
            Assert.IsTrue(instruction2.ToString() == expected2, $"Actual: {instruction2.ToString()}\r\nExpected:{expected2}");
        }


        [TestMethod]
        public void TestEmitField()
        {
            var c = new FieldTest(1);
            var cType = c.GetType();
            var cMethod = cType.GetField(nameof(c.Value));
            var expected = c.Value;
            var builder = new ILInstructionBuilder();
            builder.Write(OpCodes.Ldarg_0);
            builder.Write(OpCodes.Ldfld, cMethod.MetadataToken);
            builder.Write(OpCodes.Ret);
            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());
            ilMethod.AddParameters(new[] { cType });
            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, new[] { c });

            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

        public static Type BuildSwitchTestType()
        {
            AppDomain myDomain = Thread.GetDomain();
            AssemblyName myAsmName = new AssemblyName();
            myAsmName.Name = "MyDynamicAssembly";

            AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(
                                myAsmName,
                                AssemblyBuilderAccess.Run);
            ModuleBuilder myModBuilder = myAsmBuilder.DefineDynamicModule(
                                "MyJumpTableDemo");

            TypeBuilder myTypeBuilder = myModBuilder.DefineType("JumpTableDemo",
                                    TypeAttributes.Public);
            MethodBuilder myMthdBuilder = myTypeBuilder.DefineMethod("SwitchTest",
                                     MethodAttributes.Public |
                                     MethodAttributes.Static,
                                                     typeof(string),
                                                     new Type[] { typeof(int) });

            ILGenerator myIL = myMthdBuilder.GetILGenerator();

            Label defaultCase = myIL.DefineLabel();
            Label endOfMethod = myIL.DefineLabel();

            // We are initializing our jump table. Note that the labels
            // will be placed later using the MarkLabel method. 

            Label[] jumpTable = new Label[] { myIL.DefineLabel(),
                      myIL.DefineLabel() };/*,
                      myIL.DefineLabel(),
                      myIL.DefineLabel(),
                      myIL.DefineLabel() };*/

            // arg0, the number we passed, is pushed onto the stack.
            // In this case, due to the design of the code sample,
            // the value pushed onto the stack happens to match the
            // index of the label (in IL terms, the index of the offset
            // in the jump table). If this is not the case, such as
            // when switching based on non-integer values, rules for the correspondence
            // between the possible case values and each index of the offsets
            // must be established outside of the ILGenerator.Emit calls,
            // much as a compiler would.

            myIL.Emit(OpCodes.Ldarg_0);
            myIL.Emit(OpCodes.Switch, jumpTable);

            // Branch on default case
            myIL.Emit(OpCodes.Br_S, defaultCase);

            // Case arg0 = 0
            myIL.MarkLabel(jumpTable[0]);
            myIL.Emit(OpCodes.Ldc_I4_0);
            myIL.Emit(OpCodes.Br_S, endOfMethod);

            // Case arg0 = 1
            myIL.MarkLabel(jumpTable[1]);
            myIL.Emit(OpCodes.Ldc_I4_1);
            myIL.Emit(OpCodes.Br_S, endOfMethod);

            //// Case arg0 = 2
            //myIL.MarkLabel(jumpTable[2]);
            //myIL.Emit(OpCodes.Ldc_I4_2);
            //myIL.Emit(OpCodes.Br_S, endOfMethod);

            //// Case arg0 = 3
            //myIL.MarkLabel(jumpTable[3]);
            //myIL.Emit(OpCodes.Ldc_I4_3);
            //myIL.Emit(OpCodes.Br_S, endOfMethod);

            //// Case arg0 = 4
            //myIL.MarkLabel(jumpTable[4]);
            //myIL.Emit(OpCodes.Ldc_I4_4);
            //myIL.Emit(OpCodes.Br_S, endOfMethod);

            // Default case
            myIL.MarkLabel(defaultCase);
            myIL.Emit(OpCodes.Ldarg_0);

            myIL.MarkLabel(endOfMethod);
            myIL.Emit(OpCodes.Ret);

            return myTypeBuilder.CreateType();

        }


        [TestMethod]
        public void TestEmitCompiledSwitch()
        {
            var compiledType = BuildSwitchTestType();
            var compiledMethod = compiledType.GetMethod("SwitchTest");
            Assert.IsNotNull(compiledMethod);


            var compiledInstructions = ILInstructionReader.FromMethod(compiledMethod).ToArray();
            //TODO: auto label read instructions.

            //mark default case jump target
            compiledInstructions[7].Label = 0;
            //set default case jump target
            compiledInstructions[2].Arg = compiledInstructions[7];
            // set break target;
            compiledInstructions[8].Label = 1;

            //set jump targets for switch breaks statements
            compiledInstructions[4].Arg = compiledInstructions[6].Arg = compiledInstructions[8];

            //mark switch jump targets;

            compiledInstructions[3].Label = 2;
            compiledInstructions[5].Label = 3;

            //set switch jump targets = 
            compiledInstructions[1].Arg = new[] { compiledInstructions[3], compiledInstructions[5] };


            var builder = new ILInstructionBuilder();
            builder.Write(compiledInstructions.ToArray());

            //TODO: implement auto fixup of instuctions.

            var frame = ILStackFrameBuilder.Build(builder.Instructions);

            frame.Args = new object[] { 1 };
            frame.Reset();
            int Position = -1;
            var jumpTable = frame.Stream.ToDictionary(x => (int)x.ByteIndex, x => ++Position);

            frame.Execute();
            var expected = 1;
            Assert.IsNull(frame.Exception, $"Executing switch: add throw an exception {frame?.Exception}");
            Assert.IsTrue(frame.Stack.Count == 0, "Stack was not cleared executing switch: add");
            Assert.IsTrue((int)frame.ReturnResult == expected, $"Actual: {frame.ReturnResult}\r\nExpected: {expected}");

            frame.Args = new object[] { 0 };
            frame.Execute();
            expected = 0;
            Assert.IsNull(frame.Exception, $"Executing switch: add throw an exception {frame?.Exception}");
            Assert.IsTrue(frame.Stack.Count == 0, "Stack was not cleared executing switch: add");
            Assert.IsTrue((int)frame.ReturnResult == expected, $"Actual: {frame.ReturnResult}\r\nExpected: {expected}");
            
            frame.Args = new object[] { 2 };
            frame.Execute();
            expected = 2;
            Assert.IsNull(frame.Exception, $"Executing switch: add throw an exception {frame?.Exception}");
            Assert.IsTrue(frame.Stack.Count == 0, "Stack was not cleared executing switch: add");
            Assert.IsTrue((int)frame.ReturnResult == expected, $"Actual: {frame.ReturnResult}\r\nExpected: {expected}");

        }

        [TestMethod]
        public void TestEmitSwitch()
        {
            var endSwitchInstruction = ILInstruction.Create(ILOpCodeValues.Nop);
            endSwitchInstruction.Label = 2;
            var addInstructions = new[]
            {
                ILInstruction.Create(ILOpCodeValues.Add),
                ILInstruction.Create(ILOpCodeValues.Br_S, endSwitchInstruction)
            };
            var subInstructions = new[]
            {
                ILInstruction.Create(ILOpCodeValues.Sub),
                ILInstruction.Create(ILOpCodeValues.Br_S, endSwitchInstruction)
            };
            addInstructions[0].Label = 0;
            subInstructions[0].Label = 1;

            var exceptionType = typeof(ArgumentOutOfRangeException);
            var ctor = exceptionType.GetConstructor(Type.EmptyTypes);
            var defaultInstuctions = new[]
            {
                 ILInstruction.Create(ILOpCodeValues.Newobj, ctor.MetadataToken),
                 ILInstruction.Create(ILOpCodeValues.Throw)
            };

            var switchInstuction = ILInstruction.Create(ILOpCodeValues.Switch,
                new[] { addInstructions[0], subInstructions[0] });
            var builder = new ILInstructionBuilder();

            //var b= arg[b];
            builder.Write(ILOpCodeValues.Ldarg_1);
            //var a= arg[1];
            builder.Write(ILOpCodeValues.Ldarg_2);
            //switch(arg[0])
            builder.Write(ILOpCodeValues.Ldarg_0);
            builder.Write(switchInstuction);
            //case default
            builder.Write(defaultInstuctions);
            //case 0: add
            builder.Write(addInstructions);
            //case 1: sub
            builder.Write(subInstructions);
            builder.Write(endSwitchInstruction);
            builder.Write(ILOpCodeValues.Ret);



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


            //var type = BuildSwitchTestType();
            //var switchMethod = type.GetMethod("SwitchTest");
            //Assert.IsNotNull(switchMethod);
            //var instructions = ILInstructionReader.FromMethod(switchMethod);

            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());
            ilMethod.AddParameters(new[] { typeof(int), typeof(int), typeof(int) });
            ilMethod.AddInstructions(builder.Instructions.ToArray());

            ilMethod.Module = exceptionType.Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, new object[] { 0, 1, 2 });
            expected = 3;
            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");

            actual = method.Invoke(null, new object[] { 1, 1, 2 });
            expected = -1;
            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");

            Exception exception = null;
            try
            {
                actual = method.Invoke(null, new object[] { 2, 1, 2 });
            }
            catch (TargetInvocationException ex)
            {
                exception = ex.InnerException;
            }
            Assert.IsNotNull(exception, $"Failed to catch argument exception");
            Assert.IsInstanceOfType(exception, exceptionType);
        }



        [TestMethod]
        public void TestEmitString()
        {


            Func<string> tst = () => "hello";
            Func<int> tstToken = () =>
            {
                var tstMethod = tst.Method;
                var methodBytes = tstMethod.GetMethodBody().GetILAsByteArray();
                var ilStream = ILInstructionReader.FromByteCode(tstMethod.GetMethodBody().GetILAsByteArray());
                return (int)ilStream[0].Arg;
            };
            int stringToken = tstToken();

            var expected = tst();
            var builder = new ILInstructionBuilder();
            builder.Write(OpCodes.Ldstr, stringToken);
            builder.Write(OpCodes.Ret);
            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());

            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, Type.EmptyTypes);

            Assert.IsTrue((string)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }


        [TestMethod]
        public void TestEmitInlineVar()
        {
            var expected = 1;
            var builder = new ILInstructionBuilder();
            builder.Write(OpCodes.Ldarg, 0);
            builder.Write(OpCodes.Ret);
            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());

            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.AddParameters(new[] { typeof(int) });
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, new object[] { expected });

            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

        [TestMethod]
        public void TestEmitInlineBrTarget()
        {
            var expected = 1;
            var builder = new ILInstructionBuilder();
            builder.Write(OpCodes.Br, 0);
            builder.Write(OpCodes.Ldc_I4_1);
            builder.Write(OpCodes.Ret);
            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());

            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.AddParameters(new[] { typeof(int) });
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, new object[] { expected });

            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }
        [TestMethod]
        public void TestEmitShortInlineBrTarget()
        {
            var expected = 1;
            var builder = new ILInstructionBuilder();
            builder.Write(OpCodes.Br_S, 0);
            builder.Write(OpCodes.Ldc_I4_1);
            builder.Write(OpCodes.Ret);
            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());

            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.AddParameters(new[] { typeof(int) });
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, new object[] { expected });

            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

        [TestMethod]
        public void TestEmitShortInlineI()
        {
            var expected = 1;
            var builder = new ILInstructionBuilder();
            builder.Write(OpCodes.Ldc_I4_S, 1);
            builder.Write(OpCodes.Ret);
            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());

            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.AddParameters(new[] { typeof(int) });
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, new object[] { expected });

            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }


        [TestMethod]
        public void InlineTypeToken()
        {
            var expected = 4;
            var builder = new ILInstructionBuilder();
            var intType = typeof(int);
            builder.Write(OpCodes.Sizeof, intType.MetadataToken);
            builder.Write(OpCodes.Ret);
            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());

            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.AddParameters(new[] { typeof(int) });
            ilMethod.Module = intType.Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, new object[] { expected });

            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

        [TestMethod]
        public void TestEmitShortInlineVar()
        {
            var expected = 1;
            var builder = new ILInstructionBuilder();
            builder.Write(OpCodes.Ldarg_S, 0);
            builder.Write(OpCodes.Ret);
            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());

            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.AddParameters(new[] { typeof(int) });
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, new object[] { expected });

            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

        [TestMethod]
        public void TestEmitMethod()
        {
            var c = new ILEngineUnitTestModel(1);
            var cType = c.GetType();
            var cMethod = cType.GetMethod(nameof(c.GetValue));
            var expected = c.Value;

            var builder = new ILInstructionBuilder();

            builder.Write(OpCodes.Ldarg_0);
            builder.Write(OpCodes.Call, cMethod.MetadataToken);
            builder.Write(OpCodes.Ret);

            var frame = ILStackFrameBuilder.Build(builder.Instructions);
            frame.SetResolver(this.GetType());
            frame.Args = new object[] { c };
            frame.Execute();


            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());
            ilMethod.AddParameters(new[] { cType });
            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, new object[] { c });

            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }



        [TestMethod]
        public void TestEmitMethodInfo()
        {
            var c = new ILEngineUnitTestModel(1);
            var cType = c.GetType();
            var cMethod = cType.GetMethod(nameof(c.GetValue));
            var expected = c.Value;

            var builder = new ILInstructionBuilder();

            builder.Write(OpCodes.Ldarg_0);
            builder.Write(OpCodes.Call, cMethod);
            builder.Write(OpCodes.Ret);

            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());
            ilMethod.AddParameters(new[] { cType });
            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, new object[] { c });

            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

        [TestMethod]
        public void TestEmitConstructor()
        {

            var c = new ILEngineUnitTestModel(0);
            var cType = c.GetType();
            var cMethod = cType.GetConstructor(Type.EmptyTypes);
            var cResult = cMethod.Invoke(null);
            Assert.IsNotNull(cMethod);
            var expected = c;

            var builder = new ILInstructionBuilder();
            builder.Write(OpCodes.Newobj, cMethod.MetadataToken);
            builder.Write(OpCodes.Ret);


            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());
            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, null);

            Assert.IsTrue((ILEngineUnitTestModel)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

        [ExpectedException(typeof(OpCodeNotImplementedException))]
        [TestMethod]
        public void TestEmitSign()
        {


            //var expected = 0;
            //var builder = new ILInstructionBuilder();
            //builder.Write(OpCodes.Calli, 0);
            //builder.Write(OpCodes.Ret);


            //var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());
            //ilMethod.AddInstructions(builder.Instructions.ToArray());
            //ilMethod.Module = this.GetType().Module;
            //var method = ilMethod.Compile();
            //var actual = method.Invoke(null, null);
            throw new OpCodeNotImplementedException(ILOpCodeValues.Calli);

        }

        [TestMethod]
        public void TestEmitLdtokenField()
        {


            var builder = new ILInstructionBuilder();

            var builderType = builder.GetType();
            var intType = typeof(int);

            var tokenMember = builderType.GetField(nameof(builder.Instructions));
            var expected = tokenMember.FieldHandle;


            builder.Write(ILOpCodeValues.Ldtoken, tokenMember.MetadataToken);
            builder.Write(OpCodes.Ret);


            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());
            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.Module = builderType.Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, null);

            Assert.IsTrue((RuntimeFieldHandle)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");

        }

        [TestMethod]
        public void TestEmitLdtokenMethod()
        {

            var intType = typeof(int);

            var tokenMember = intType.GetMethod(nameof(int.GetType));
            var expected = tokenMember.MethodHandle;

            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldtoken, tokenMember.MetadataToken);
            builder.Write(OpCodes.Ret);


            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());
            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.Module = intType.Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, null);

            Assert.IsTrue((RuntimeMethodHandle)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

        [TestMethod]
        public void TestEmitLdtokenType()
        {

            var intType = typeof(int);


            var expected = intType.TypeHandle;

            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldtoken, intType.MetadataToken);
            builder.Write(OpCodes.Ret);


            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());
            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.Module = intType.Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, null);

            Assert.IsTrue(actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

        [TestMethod]
        public void TestEmitConstructorInfo()
        {

            var c = new ILEngineUnitTestModel(0);
            var cType = c.GetType();
            var cMethod = cType.GetConstructor(Type.EmptyTypes);
            var cResult = cMethod.Invoke(null);
            Assert.IsNotNull(cMethod);
            var expected = c;

            var builder = new ILInstructionBuilder();
            builder.Write(OpCodes.Newobj, cMethod);
            builder.Write(OpCodes.Ret);


            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());
            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.Module = cType.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, null);

            Assert.IsTrue((ILEngineUnitTestModel)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

        [TestMethod]
        public void TestEmitInlineI4()
        {
            var expected = 1;
            var builder = new ILInstructionBuilder();
            builder.Write(OpCodes.Ldc_I4, expected);
            builder.Write(OpCodes.Ret);
            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());
            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, Type.EmptyTypes);
            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

        [TestMethod]
        public void TestEmitInlineI8()
        {
            var expected = 1L;
            var builder = new ILInstructionBuilder();
            builder.Write(OpCodes.Ldc_I8, expected);
            builder.Write(OpCodes.Ret);
            var ilMethod = new ILMethod(MethodBase.GetCurrentMethod().Name, expected.GetType());
            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, Type.EmptyTypes);
            Assert.IsTrue((long)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

        [TestMethod]
        public void TestEmitInlineR4()
        {

            var expected = 1f;
            var builder = new ILInstructionBuilder();
            builder.Write(OpCodes.Ldc_R4, expected);
            builder.Write(OpCodes.Ret);
            var ilMethod = new ILMethod(System.Reflection.MethodBase.GetCurrentMethod().Name, expected.GetType());
            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, Type.EmptyTypes);
            Assert.IsTrue((float)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

        [TestMethod]
        public void TestEmitInlineR8()
        {

            var expected = 1d;
            var builder = new ILInstructionBuilder();
            builder.Write(OpCodes.Ldc_R8, expected);
            builder.Write(OpCodes.Ret);
            var ilMethod = new ILMethod(System.Reflection.MethodBase.GetCurrentMethod().Name, expected.GetType());
            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, Type.EmptyTypes);
            Assert.IsTrue((double)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

        [TestMethod]
        public void TestEmitInlineNone()
        {

            var expected = 1;
            var builder = new ILInstructionBuilder();
            builder.Write(OpCodes.Ldc_I4_1);
            builder.Write(OpCodes.Ret);
            var ilMethod = new ILMethod(System.Reflection.MethodBase.GetCurrentMethod().Name, expected.GetType());
            ilMethod.AddInstructions(builder.Instructions.ToArray());
            ilMethod.Module = this.GetType().Module;
            var method = ilMethod.Compile();
            var actual = method.Invoke(null, Type.EmptyTypes);
            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\nExpected:{expected}");
        }

    }
}