using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace ILEngine.Tests
{




    [TestClass()]
    public class IlLStackFrameWithDiagnosticsTests
    {
        [TestMethod]
        public void TestEmptyStackWithoutArgsOrLocalsAndNoOperandExclusions()
        {
            var allOpCodes = OpCodeLookup.OpCodes.Select(x => x.Value).AsQueryable<OpCode>();

            var opcodesbyname = OpCodeLookup.OpCodesByName;

            var filters = OpCodeFilters.EmptyStackWithNoArgsLocalsAndNoInlineOperandFilters();
            allOpCodes = allOpCodes.Where(x => filters.All((filter) => filter(x)));

            var rem = allOpCodes.ToList();
            Assert.IsTrue(rem.Count == 12);
            for (var i = 0; i < rem.Count; i++)
            {
                var opCodes = rem.Skip(i).Take(1).ToList();
                opCodes.Add(OpCodes.Ret);
                var result = ILStackFrameBuilder.BuildAndExecute(opCodes);
                Assert.IsTrue(result.ExecutedInstructions == 2);
                Assert.IsNull(result.Exception);
            }

        }

        [TestMethod]
        public void TestBuildEmptyStackWithArgsAndLocalsAndInlineOnlyOperandFilters()
        {
            var allOpCodes = OpCodeLookup.OpCodes.Select(x => x.Value).AsQueryable<OpCode>();

            var opcodesbyname = OpCodeLookup.OpCodesByName;

            List<object> args = new List<object>();
            List<ILVariable> iLVariables = new List<ILVariable>();
            var filters = OpCodeFilters.BuildEmptyStackWithArgsAndLocalsAndInlineOnlyOperandFilters(args.ToArray(), iLVariables.ToArray());
            int expected = 12;
            var rem = allOpCodes.Where(x => filters.All((filter) => filter(x))).ToList();

            Assert.IsTrue(rem.Count == 12);
            for (var argCount = 1; argCount < 5; argCount++)
            {
                args.Add(argCount - 1);
                filters = OpCodeFilters.BuildEmptyStackWithArgsAndLocalsAndInlineOnlyOperandFilters(args.ToArray(), iLVariables.ToArray());
                expected += 1;
                rem = allOpCodes.Where(x => filters.All((filter) => filter(x))).ToList();
                Assert.IsTrue(rem.Count == expected);
            }
            for (var variableCount = 1; variableCount < 5; variableCount++)
            {
                iLVariables.Add(new ILVariable { Name = $"var{variableCount}", Index = variableCount - 1, Type = typeof(int), Value = variableCount - 1 });
                filters = OpCodeFilters.BuildEmptyStackWithArgsAndLocalsAndInlineOnlyOperandFilters(args.ToArray(), iLVariables.ToArray());
                expected += 1;
                rem = allOpCodes.Where(x => filters.All((filter) => filter(x))).ToList();
                Assert.IsTrue(rem.Count == expected);
            }



            //rem = allOpCodes.ToList();
            ;
            for (var i = 0; i < rem.Count; i++)
            {
                var opCodes = rem.Skip(i).Take(1).ToList();
                opCodes.Add(OpCodes.Ret);
                var result = ILStackFrameBuilder.BuildAndExecute(opCodes, args: args.ToArray(), locals: iLVariables.ToArray());
                Assert.IsTrue(result.ExecutedInstructions == 2);
                Assert.IsNull(result.Exception);
            }

        }

        [TestMethod]
        public void TestFlowControlExclusions()
        {
            var allOpCodes = OpCodeLookup.OpCodes.Select(x => x.Value).AsQueryable<OpCode>();

            var opcodesbyname = OpCodeLookup.OpCodesByName;

            var exclusions = new List<Func<OpCode, bool>>();

            var flowControlExclusions = new List<Func<FlowControl, bool>>();
            flowControlExclusions.Add(x => x == FlowControl.Meta);
#pragma warning disable CS0618 // Type or member is obsolete
            flowControlExclusions.Add(x => x == FlowControl.Phi);
#pragma warning restore CS0618 // Type or member is obsolete
            flowControlExclusions.Add(x => x == FlowControl.Break);



            allOpCodes = allOpCodes.Where(x => !flowControlExclusions.Any((exclude) => exclude(x.FlowControl)));


            var rem = allOpCodes.ToList();
            Assert.IsTrue(rem.Count == 214);


        }

        [TestMethod()]
        public void ResetTest()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldc_I4_0);
            builder.Write(ILOpCodeValues.Ldc_I4_1);
            builder.Write(ILOpCodeValues.Ret);

            var frame = new ILStackFrameWithDiagnostics();
            frame.Stream = builder.Instructions;
            frame.Execute();

            Assert.IsTrue(frame.ReturnResult == 1);
            Assert.IsTrue(frame.Stack.Count == 1);
            frame.Reset();

            Assert.IsNull(frame.ReturnResult);
            Assert.IsTrue(frame.Stack.Count == 0);
        }

        [TestMethod()]
        public void ReadNextTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void IncTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void MoveNextTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void ExecuteTest()
        {
            var frame = new ILStackFrameWithDiagnostics();
            Action empty = () => { };
            var instructions = ILInstructionReader.FromMethod(empty.Method);
            frame.Stream = instructions;
            frame.SetResolver(empty.Method);
            frame.Execute();

            Assert.IsNull(frame.ReturnResult);
            Assert.IsNull(frame.Exception);


            Assert.IsTrue(frame.Module.MetadataToken == empty.Method.Module.MetadataToken);

            var thisType = this.GetType();
            var testMethod = thisType.GetMethod(nameof(ExecuteTest));
            Assert.IsNotNull(testMethod);
            var methodSigToken = testMethod.GetMethodBody().LocalSignatureMetadataToken;
            Assert.IsTrue(methodSigToken >0);
            var resolvedSig = frame.ResolveSignatureToken(methodSigToken);
            Assert.IsNotNull(resolvedSig);
            Assert.IsTrue(resolvedSig.Length > 0);

            frame.Execute(1);
            Assert.IsNull(frame.ReturnResult);
            Assert.IsNull(frame.Exception);

            Action sleepToLong = () => System.Threading.Thread.Sleep(4000);
            instructions = ILInstructionReader.FromMethod(sleepToLong.Method);
            frame.Stream = instructions;
            frame.Execute(1);

            Assert.IsNull(frame.ReturnResult);
            Assert.IsNotNull(frame.Exception);
            Assert.IsInstanceOfType(frame.Exception, typeof(ActionTimeoutException));


            Action throwException = () => throw new ArgumentException(nameof(throwException));
            instructions = ILInstructionReader.FromMethod(throwException.Method);
            frame.Stream = instructions;
            frame.Execute(1);

            Assert.IsNull(frame.ReturnResult);
            Assert.IsNotNull(frame.Exception);
            Assert.IsInstanceOfType(frame.Exception, typeof(ArgumentException));

         

        }

        [TestMethod()]
        public void TestExceptionHandling()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILOpCodeValues.Ldobj, "test");
            builder.Write(ILOpCodeValues.Conv_I4);
            var frame = new ILStackFrameWithDiagnostics();
            frame.Stream = builder.Instructions;
            frame.Execute(100);
            Assert.IsInstanceOfType(frame.Exception, typeof(FormatException));



        }


        [TestMethod()]
        public void TestLoadAndStoreLocals()
        {


            var frame = new ILStackFrameWithDiagnostics();

            var intType = typeof(int);
            var rng = Enumerable.Range(0, 8);
            frame.Locals = rng
                .Select(idx => new ILVariable { Index = idx, Type = intType, Value = idx }).ToArray();
            var stream = new ILInstructionBuilder();
            frame.Stream = stream.Instructions;




            stream.Write(OpCodes.Ldloc_0);
            stream.Write(OpCodes.Stloc_0);
            stream.Write(OpCodes.Ldloc_0);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Locals[0].Value, $"Locals 0 failed, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldloc_1);
            stream.Write(OpCodes.Stloc_1);
            stream.Write(OpCodes.Ldloc_1);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Locals[1].Value, $"Locals 1 failed, return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            stream.Write(OpCodes.Ldloc_2);
            stream.Write(OpCodes.Stloc_2);
            stream.Write(OpCodes.Ldloc_2);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Locals[2].Value, $"Locals 2 failed, return result = {frame.ReturnResult ?? "null"}"); ;

            stream.Clear();
            stream.Write(OpCodes.Ldloc_3);
            stream.Write(OpCodes.Stloc_3);
            stream.Write(OpCodes.Ldloc_3);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Locals[3].Value, $"Locals 3 failed, return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            stream.Write(OpCodes.Ldloc, 4);
            stream.Write(OpCodes.Stloc, 4);
            stream.Write(OpCodes.Ldloc, 4);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Locals[4].Value, $"Ldloc(arg=4) failed, return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            stream.Write(OpCodes.Ldloc_S, 5);
            stream.Write(OpCodes.Stloc_S, 5);
            stream.Write(OpCodes.Ldloc_S, 5);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Locals[5].Value, $"Ldloc_S(arg=5) failed, return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            stream.Write(OpCodes.Ldloca, 6);
            stream.Write(OpCodes.Stloc_S, 6);
            stream.Write(OpCodes.Ldloca, 6);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Locals[6].Value, $"Ldloca(arg=6) failed, return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            stream.Write(OpCodes.Ldloca_S, 7);
            stream.Write(OpCodes.Stloc_S, 7);
            stream.Write(OpCodes.Ldloca_S, 7);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Locals[7].Value, $"Ldloca_S(arg=6) failed, return result = {frame.ReturnResult ?? "null"}");

        }


        [TestMethod()]
        public void TestLoadAndStoreArgs()
        {


            var frame = new ILStackFrameWithDiagnostics();

            var intType = typeof(int);
            var rng = Enumerable.Range(0, 8);
            frame.Args = rng
                .Select(idx => (object)idx).ToArray();
            var stream = new ILInstructionBuilder();

            frame.Stream = stream.Instructions;




            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Starg, 0);
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Args[0], $"Args 0 failed, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldarg_1);
            stream.Write(OpCodes.Starg_S, 1);
            stream.Write(OpCodes.Ldarg_1);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Args[1], $"Args 1 failed, return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            stream.Write(OpCodes.Ldarg_2);
            stream.Write(OpCodes.Starg, 2);
            stream.Write(OpCodes.Ldarg_2);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Args[2], $"Args 2 failed, return result = {frame.ReturnResult ?? "null"}"); ;

            stream.Clear();
            stream.Write(OpCodes.Ldarg_3);
            stream.Write(OpCodes.Starg, 3);
            stream.Write(OpCodes.Ldarg_3);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Args[3], $"Args 3 failed, return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            stream.Write(OpCodes.Ldarg, 4);
            stream.Write(OpCodes.Starg, 4);
            stream.Write(OpCodes.Ldarg, 4);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Args[4], $"Args(arg=4) failed, return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            stream.Write(OpCodes.Ldarg_S, 5);
            stream.Write(OpCodes.Starg_S, 5);
            stream.Write(OpCodes.Ldarg_S, 5);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Args[5], $"Args(arg=5) failed, return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            stream.Write(OpCodes.Ldarga, 6);
            stream.Write(OpCodes.Starg_S, 6);
            stream.Write(OpCodes.Ldarga, 6);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Args[6], $"Ldarga(arg=6) failed, return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            stream.Write(OpCodes.Ldarga_S, 7);
            stream.Write(OpCodes.Starg_S, 7);
            stream.Write(OpCodes.Ldarga_S, 7);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == (int)frame.Args[7], $"Ldarga_S(arg=6) failed, return result = {frame.ReturnResult ?? "null"}");

        }

        [TestMethod]
        public void TestLoadConstants()
        {
            var frame = new ILStackFrameWithDiagnostics();

            var stream = new ILInstructionBuilder();

            frame.Stream = stream.Instructions;



            stream.Clear();
            stream.Write(OpCodes.Ldc_I4_0);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == 0, $"Ldc_I4_0 failed, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_I4_1);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == 1, $"Ldc_I4_1 failed, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_I4_2);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == 2, $"Ldc_I4_2 failed, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_I4_3);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == 3, $"Ldc_I4_3 failed, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_I4_4);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == 4, $"Ldc_I4_4 failed, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_I4_5);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == 5, $"Ldc_I4_5 failed, return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            stream.Write(OpCodes.Ldc_I4_6);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == 6, $"Ldc_I4_6 failed, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_I4_7);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == 7, $"Ldc_I4_7 failed, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_I4_8);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == 8, $"Ldc_I4_8 failed, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_I4_M1);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == -1, $"Ldc_I4_M1 failed, return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            stream.Write(OpCodes.Ldc_I4, 1);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == 1, $"Ldc_I4(1) failed, return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            stream.Write(OpCodes.Ldc_I4_S, byte.MaxValue);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == byte.MaxValue, $"Ldc_I4_s(255) failed, return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            stream.Write(OpCodes.Ldc_I8, long.MaxValue);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == long.MaxValue, $"Ldc_I8(long.MaxValue) failed, return result = {frame.ReturnResult ?? "null"}");



            stream.Clear();
            stream.Write(OpCodes.Ldc_R4, 0);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == 0, $"Ldc_R4(float.MaxValue) failed, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_R4, float.MaxValue);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == float.MaxValue, $"Ldc_R4(float.MaxValue) failed, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_R4, float.MinValue);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == float.MinValue, $"Ldc_R4(float.MinValue) failed, return result = {frame.ReturnResult ?? "null"}");



            stream.Clear();
            stream.Write(OpCodes.Ldc_R8, 0);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == 0, $"Ldc_R8(float.MaxValue) failed, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_R8, double.MaxValue);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == double.MaxValue, $"Ldc_R8(double.MaxValue) failed, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_R8, double.MinValue);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == double.MinValue, $"Ldc_R8(double.MinValue) failed, return result = {frame.ReturnResult ?? "null"}");
        }

        [TestMethod]
        public void TesCheckInfinity()
        {
            var frame = new ILStackFrameWithDiagnostics();

            var stream = new ILInstructionBuilder();

            frame.Stream = stream.Instructions;



            stream.Clear();
            stream.Write(ILOpCodeValues.Ldc_R4, float.NegativeInfinity);
            stream.Write(OpCodes.Ckfinite);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == true, $"Ckfinite float.NegativeInfinity, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_R4, float.PositiveInfinity);
            stream.Write(OpCodes.Ckfinite);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == true, $"Ckfinite float.PositiveInfinity, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_R4, 1.0f);
            stream.Write(OpCodes.Ckfinite);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == false, $"Ckfinite float.PositiveInfinity, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_R4, 1);
            stream.Write(OpCodes.Ckfinite);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == false, $"Ckfinite float.PositiveInfinity, return result = {frame.ReturnResult ?? "null"}");



            stream.Clear();
            stream.Write(OpCodes.Ldc_R8, double.NegativeInfinity);
            stream.Write(OpCodes.Ckfinite);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == true, $"Ckfinite double.NegativeInfinityd, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_R8, double.PositiveInfinity);
            stream.Write(OpCodes.Ckfinite);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == true, $"Ckfinite double.PositiveInfinity, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_R8, 1.0);
            stream.Write(OpCodes.Ckfinite);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == false, $"Ckfinite 1.0, return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            stream.Write(OpCodes.Ldc_R8, 1);
            stream.Write(OpCodes.Ckfinite);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsTrue(frame.ReturnResult == false, $"Ckfinite 1, return result = {frame.ReturnResult ?? "null"}");

        }

        [TestMethod]
        public void TestConvSigned()
        {
            var frame = new ILStackFrameWithDiagnostics();

            var stream = new ILInstructionBuilder();

            frame.Stream = stream.Instructions;



            stream.Clear();
            frame.Args = new object[] { sbyte.MinValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_I);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (int)sbyte.MinValue, $"Conv_I sbyte.MinValue return result = {frame.ReturnResult ?? "null"}");



            stream.Clear();
            frame.Args = new object[] { byte.MinValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_I);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (int)byte.MinValue, $"Conv_I byte.MinValue return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            frame.Args = new object[] { byte.MaxValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_I);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (int)byte.MaxValue, $"Conv_I byte.MaxValue return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            frame.Args = new object[] { sbyte.MinValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_I1);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (int)sbyte.MinValue, $"Conv_I1 sbyte.MinValue return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            frame.Args = new object[] { sbyte.MaxValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_I1);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (int)sbyte.MaxValue, $"Conv_I1 sbyte.MaxValue  return result = {frame.ReturnResult ?? "null"}");



            stream.Clear();
            frame.Args = new object[] { short.MinValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_I2);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (int)short.MinValue, $"Conv_I1 short.MinValue return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            frame.Args = new object[] { short.MaxValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_I2);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (int)short.MaxValue, $"Conv_I2 short.MaxValue  return result = {frame.ReturnResult ?? "null"}");



            stream.Clear();
            frame.Args = new object[] { int.MinValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_I4);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (int)int.MinValue, $"Conv_I4 int.MinValue return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            frame.Args = new object[] { int.MaxValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_I4);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (int)int.MaxValue, $"Conv_I4 int.MaxValue  return result = {frame.ReturnResult ?? "null"}");



            stream.Clear();
            frame.Args = new object[] { long.MinValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_I8);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == long.MinValue, $"Conv_I8 long.MinValue return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            frame.Args = new object[] { long.MaxValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_I8);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == long.MaxValue, $"Conv_I8 long.MaxValue  return result = {frame.ReturnResult ?? "null"}");
        }


        [TestMethod]
        public void TestConvUnsigned()
        {
            var frame = new ILStackFrameWithDiagnostics();

            var stream = new ILInstructionBuilder();

            frame.Stream = stream.Instructions;



            stream.Clear();
            frame.Args = new object[] { byte.MinValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_U);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (uint)byte.MinValue, $"Conv_U byte.MinValue return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            frame.Args = new object[] { byte.MaxValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_U);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (uint)byte.MaxValue, $"Conv_U byte.MaxValue return result = {frame.ReturnResult ?? "null"}");


            stream.Clear();
            frame.Args = new object[] { byte.MinValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_U1);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (uint)byte.MinValue, $"Conv_U1 byte.MinValue return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            frame.Args = new object[] { byte.MaxValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_U1);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (uint)byte.MaxValue, $"Conv_U1 byte.MaxValue  return result = {frame.ReturnResult ?? "null"}");



            stream.Clear();
            frame.Args = new object[] { ushort.MinValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_U2);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (uint)ushort.MinValue, $"Conv_U2 ushort.MinValue return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            frame.Args = new object[] { ushort.MaxValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_U2);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (uint)ushort.MaxValue, $"Conv_U2 ushort.MaxValue  return result = {frame.ReturnResult ?? "null"}");



            stream.Clear();
            frame.Args = new object[] { uint.MinValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_U4);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (uint)uint.MinValue, $"Conv_U4 uint.MinValue return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            frame.Args = new object[] { uint.MaxValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_U4);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == (uint)uint.MaxValue, $"Conv_U4 uint.MaxValue  return result = {frame.ReturnResult ?? "null"}");



            stream.Clear();
            frame.Args = new object[] { ulong.MinValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_U8);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == ulong.MinValue, $"ConvU_U8 ulong.MinValue return result = {frame.ReturnResult ?? "null"}");

            stream.Clear();
            frame.Args = new object[] { ulong.MaxValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_U8);
            stream.Write(OpCodes.Ret);
            frame.Execute();
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == ulong.MaxValue, $"Conv_U8 ulong.MaxValue  return result = {frame.ReturnResult ?? "null"}");
        }

        [TestMethod]
        public void TestConvNoOverflowCheck()
        {
            var frame = new ILStackFrameWithDiagnostics();

            var stream = new ILInstructionBuilder();

            frame.Stream = stream.Instructions;

            stream.Clear();
            frame.Args = new object[] { uint.MaxValue };
            stream.Write(OpCodes.Ldarg_0);
            stream.Write(OpCodes.Conv_I);
            stream.Write(OpCodes.Ret);
            frame.Execute();

            int expected = unchecked((int)Convert.ToInt64(uint.MaxValue));
            Assert.IsNull(frame.Exception, $"Frame returned exception: {frame.Exception?.Message}");
            Assert.IsTrue(frame.ReturnResult == expected, $"Conv_I uint.MaxValue failed\r\nExpected: {expected}\r\nActual:{frame.ReturnResult ?? "null"}");
        }
    }
}