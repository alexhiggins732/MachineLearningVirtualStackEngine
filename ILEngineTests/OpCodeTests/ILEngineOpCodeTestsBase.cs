using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;

namespace ILEngine.Tests
{
    public class ILEngineOpCodeTestsBase<TEngine, TStackFrame>
        where TEngine : IILEngine, new()
        where TStackFrame : IILStackFrame<TEngine>, new()
    {
        private TEngine engine = new TEngine();

        /// <summary>
        /// Runs unit tests on all methods defined in <see cref="TestMethods"/> that have a <see cref="OpCodeTestAttribute"/> defined
        /// using the arguments specified in the attribute.
        /// </summary>
        /// <remarks>
        /// IL code from the compiled method is first read from the as a byte array of native IL instructions which are then read
        /// as <see cref="ILInstruction"/> using the <see cref="ILInstructionReader"/>. 
        /// The IL instructions are then used to compile a dynamic method using a <see cref="System.Reflection.Emit.MethodBuilder"/>
        /// to asser that the <see cref="ILInstructionReader"/> reads the instructions correctly
        /// and that <see cref="ILInstruction.Emit(ILGenerator, IILInstructionResolver)"/> method
        /// generates the correct <see cref="ILInstruction.OpCode"/> and optional <see cref="ILInstruction.Arg"/>.
        /// The dynamically compiled method is invoked and its result is compared to the compiled <see cref="CompiledTest.Method"/>
        /// result. The result is then compared to the result produced using <see cref="ILInstructionEngine"/> to run it on the VM
        /// with  <see cref="TEngine.ExecuteFrame(IILStackFrame)"/>.
        /// </remarks>
        [TestMethod()]
        public void CompiledOpCodeTests()
        {
            var tests = ILEngineOpcodeTestMethodHelper.CompiledTests;
            foreach (var test in tests)
            {
                if (test.TestAttribute is OpCodeIntTestAttribute intTest)
                {
                    CompileAndRun<int>(test.OpCode, test.Method, intTest.Expected, intTest.Parameters.Cast<object>().ToArray(), (a, b) => intTest.Equals(a, b));
                }
                else if (test.TestAttribute is OpCodeIntCompareTestAttribute intCompareTest)
                {
                    CompileAndRun<bool>(test.OpCode, test.Method, intCompareTest.Expected, intCompareTest.Parameters.Cast<object>().ToArray(), (a, b) => intCompareTest.Equals(a, b));
                }
                else if (test.TestAttribute is OpCodeUIntTestAttribute uintTest)
                {
                    CompileAndRun<uint>(test.OpCode, test.Method, uintTest.Expected, uintTest.Parameters.Cast<object>().ToArray(), (a, b) => uintTest.Equals(a, b));
                }
                else if (test.TestAttribute is OpCodeExpectedExceptionAttribute excTest)
                {
                    CompileAndRun<object>(test.OpCode, test.Method, excTest.Expected, excTest.Parameters.Cast<object>().ToArray(), (a, b) => throw new NotImplementedException());
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
        //TODO: Complete implementation or remove
        [ExpectedException(typeof(NotImplementedException), "IncompleteTestMethod")]
        [TestMethod()]
        public void TestAllOpCodesInEngine()
        {
            var opCodeFields = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static);
            ILOpCodeValues parseResult = ILOpCodeValues.Nop;
            var notCovered = new List<ILOpCodeValues>(Enum.GetValues(typeof(ILOpCodeValues)).Cast<ILOpCodeValues>());
            var notImplementedList = new List<Exception>();
            foreach (var field in opCodeFields)
            {
                var opCode = (OpCode)field.GetValue(null);

                var name = field.Name;
                var lookup = Enum.TryParse(name, out parseResult);
                if (parseResult == ILOpCodeValues.Break) continue;

                if (bool.Parse(bool.TrueString) || notCovered.Contains(parseResult))
                {
                    ILOpCodeValues[] used = null;
                    Exception caught = null;
                    try
                    {
                        used = RunTestForOpCode(opCode);
                        Assert.IsNotNull(used, $"OpCode test {opCode} failed to return used opcodes");
                        notCovered.RemoveAll(x => used.Contains(x));
                    }
                    catch (NotImplementedException niex)
                    {
                        var listEx = new NotImplementedException($"Test {opCode} is not implemented", niex);
                        notImplementedList.Add(listEx);
                    }
                    catch (Exception ex)
                    {
                        caught = ex;
                    }
                    Assert.IsNull(caught, $"Error executing Test {opCode}: {caught}");
                  
                }
            }
            Assert.IsTrue(notImplementedList.Count > 0, "Test failed to throw missing test exception");
            throw notImplementedList[0];
        }


        protected ILOpCodeValues[] CompileAndRun<T>(OpCode opCode, MethodInfo srcMethod, T expected, object[] parameters, Func<T, T, bool> comparer)
        {
            var methodParameters = srcMethod.GetParameters();
            var body = srcMethod.GetMethodBody();
            var locals = body.LocalVariables.OrderBy(x => x.LocalIndex).ToList();

            var parameterTypes = methodParameters.Select(x => x.ParameterType).ToArray();

            var instructions = ILInstructionReader.FromMethod(srcMethod);
            if (opCode != OpCodes.Arglist)
            {
                Assert.IsTrue(instructions.Any(x => x.OpCode == opCode), $"Opcode {opCode} missing from compiled method");
            }


            var result = ILInstructionReader
              .FromMethod(srcMethod)
              .Select(x => (ILOpCodeValues)x.OpCode.Value).ToArray();
            var resolver = new ILInstructionResolver(srcMethod);
            Action<ILGenerator> emitAction = (gen) =>
            {
                locals.ForEach(x => gen.DeclareLocal(x.LocalType));
                instructions.ForEach(x => x.Emit(gen, resolver));
            };

            MethodInfo method = null;
            try
            {
                method = ILEngineOpCodeTestMethodCompiler.CompileMethod(opCode, typeof(T), parameterTypes, emitAction);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to compile method:", ex.Message);
                return null;
            }
            return RunAndTest<T>(method, expected, parameters, comparer);

        }

        private ILOpCodeValues[] RunAndTest<T>(MethodInfo method, T expected, object[] parameters, Func<T, T, bool> comparer)
        {
            var result = ILInstructionReader
               .FromMethod(method)
               .Select(x => (ILOpCodeValues)x.OpCode.Value).ToArray();


            bool invokedResult = false;
            Exception caughtCompiled = null;
            T invocationResult = default;
            try
            {
                invocationResult = (T)method.Invoke(null, parameters);
                invokedResult = comparer(expected, invocationResult);

            }
            catch (Exception ex)
            {
                caughtCompiled = ex.InnerException;
            }

            var engineResult = false;
            Exception caughtEngine = null;
            try
            {
                var frame = new TStackFrame();
                frame.CopyFrom(method);
                frame.Args = parameters;
                engine.ExecuteFrame(frame);
                //var actualEngine = engine.ExecuteTyped<T>(method, parameters);
                if (!(frame.ReturnResult is T tresult))
                {
                    if (frame.ReturnResult is IConvertible tConvertible)
                    {
                        frame.ReturnResult = Convert.ChangeType(tConvertible, typeof(T));
                    }
                    else
                    {
                        frame.ReturnResult = (T)frame.ReturnResult;
                    }
                }
                var actualEngine = (T)frame.ReturnResult;
                engineResult = comparer(expected, actualEngine);
            }
            catch (Exception ex)
            {
                caughtEngine = ex;
            }


            if (caughtCompiled is null && caughtEngine is null)
            {
                Assert.IsTrue(invokedResult == engineResult);
            }
            else
            {
                if (!(caughtCompiled is null) && !(caughtEngine is null))
                {
                    Assert.IsTrue(caughtCompiled.Message == caughtEngine.Message);
                }
                else
                {
                    if (!(caughtCompiled is null))
                        Assert.Fail("Compiled method threw an exception: {0}", caughtCompiled.ToString());
                    else
                        Assert.Fail("Engine method threw an exception: {0}", caughtEngine.ToString());
                }
            }


            return result;



        }

        protected ILOpCodeValues[] CompileAndRun<T>(OpCode opCode, T expected, object[] parameters, Action<ILGenerator> emitAction, Func<T, T, bool> comparer)
        {
            Type[] parameterTypes = new Type[] { };
            if (parameters != null && parameters.Length > 0)
            {
                parameterTypes = new Type[parameters.Length];
                for (var i = 0; i < parameters.Length; i++) parameterTypes[i] = parameters[i]?.GetType() ?? typeof(object);
            }


            MethodInfo method = null;

            try
            {
                method = ILEngineOpCodeTestMethodCompiler.CompileMethod(opCode, typeof(T), parameterTypes, emitAction);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to compile method:", ex.Message);
                return null;
            }
            return RunAndTest<T>(method, expected, parameters, comparer);

        }

        public static int TestLdArg0(int a) => a;

        public static object LocalsAndArgsTestMethod(
                 char arg_a, bool arg_b, sbyte arg_c, byte arg_d,
                 short arg_e, ushort arg_f, int arg_g, uint arg_h,
                 long arg_i, ulong arg_j, float arg_k, double arg_l,
                 decimal arg_m, BigInteger arg_n, int ret)
        {
            var a = arg_a;
            var b = arg_b;
            var c = arg_c;
            var d = arg_d;
            var e = arg_e;
            var f = arg_f;
            var g = arg_g;
            var h = arg_h;
            var i = arg_i;
            var j = arg_j;
            var k = arg_k;
            var l = arg_l;
            var m = arg_m;
            var n = arg_n;
            if (ret == 0) return a;
            if (ret == 1) return b;
            if (ret == 2) return c;
            if (ret == 3) return d;
            if (ret == 4) return e;
            if (ret == 5) return f;
            if (ret == 6) return g;
            if (ret == 7) return h;
            if (ret == 8) return i;
            if (ret == 9) return j;
            if (ret == 10) return k;
            if (ret == 11) return l;
            if (ret == 12) return m;
            if (ret == 13) return n;
            return ret;

        }

        protected ILOpCodeValues[] RunTestForOpCode(OpCode opCode)
        {
            //ILGenerator gen = null;
            switch (opCode.Value)
            {
                case (short)ILOpCodeValues.Nop:
                    return CompileAndRun<object>(opCode, null, new object[] { }, (x) =>
                    {
                        x.Emit(OpCodes.Nop);
                        x.Emit(OpCodes.Ldnull);
                        x.Emit(OpCodes.Ret);
                    }
                    , (a, b) => a is null && b is null
                    );

                case (short)ILOpCodeValues.Add:
                    return CompileAndRun(opCode, 2, new object[] { 1, 2, }, (x) =>
                    {
                        x.Emit(OpCodes.Ldarg_0);
                        x.Emit(OpCodes.Ldarg_1);
                        x.Emit(OpCodes.Add);
                        x.Emit(OpCodes.Ret);
                    }
                    , (a, b) => a == b
                    );
                case (short)ILOpCodeValues.Break:
                    return CompileAndRun<object>(opCode, null, new object[] { }, (x) =>
                    {
                        x.Emit(OpCodes.Nop);
                        x.Emit(OpCodes.Ldnull);
                        x.Emit(OpCodes.Break);
                        x.Emit(OpCodes.Ret);
                    }
                   , (a, b) => a is null && b is null
                   );

                case (short)ILOpCodeValues.Ldarg_0:
                    var srcType = this.GetType();
                    var srcMethod = srcType.GetMethod(nameof(TestLdArg0), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    Assert.IsNotNull(srcMethod, $"Failed to get method {srcType.Name}.{nameof(TestLdArg0)}");
                    return CompileAndRun<int>(opCode, srcMethod, 1,
                        new object[] { 1 }, (a, b) => a == b);


                case (short)ILOpCodeValues.Ldarg_1:
                    return CompileAndRun(opCode, 1, new object[] { 0, 1 }, (x) =>
                    {
                        x.Emit(OpCodes.Ldarg_1);
                        x.Emit(OpCodes.Ret);
                    }
                   , (a, b) => a == b
                   );
                case (short)ILOpCodeValues.Ldarg_2:
                    return CompileAndRun(opCode, 1, new object[] { 0, 0, 1 }, (x) =>
                    {
                        x.Emit(OpCodes.Ldarg_2);
                        x.Emit(OpCodes.Ret);
                    }
                   , (a, b) => a == b
                   );
                case (short)ILOpCodeValues.Ldarg_3:
                    return CompileAndRun(opCode, 1, new object[] { 0, 0, 0, 1 }, (x) =>
                    {
                        x.Emit(OpCodes.Ldarg_3);
                        x.Emit(OpCodes.Ret);
                    }
                   , (a, b) => a == b
                   );
                case (short)ILOpCodeValues.Ldarg_S:
                    return CompileAndRun(opCode, 1, new object[] { 0, 0, 0, 1 }, (x) =>
                    {
                        x.Emit(OpCodes.Ldarg_S, (byte)3);
                        x.Emit(OpCodes.Ret);
                    }
                   , (a, b) => a == b
                   );
                case (short)ILOpCodeValues.Stloc_0:
                case (short)ILOpCodeValues.Ldloc_0:
                    return LocalsAndArgsTest(opCode, 0);
                case (short)ILOpCodeValues.Stloc_1:
                case (short)ILOpCodeValues.Ldloc_1:
                    return LocalsAndArgsTest(opCode, 1);
                case (short)ILOpCodeValues.Stloc_2:
                case (short)ILOpCodeValues.Ldloc_2:
                    return LocalsAndArgsTest(opCode, 2);
                case (short)ILOpCodeValues.Stloc_3:
                case (short)ILOpCodeValues.Ldloc_3:
                    return LocalsAndArgsTest(opCode, 3);
                case (short)ILOpCodeValues.Stloc_S:
                case (short)ILOpCodeValues.Ldloc_S:
                    return LocalsAndArgsTest(opCode, 13);

                default:
                    //return new ILOpCodeValues[] { ILOpCodeValues.Nop};
                    //break;
                    throw new NotImplementedException();


            }
        }

        protected ILOpCodeValues[] LocalsAndArgsTest(OpCode opCode, int arg)
        {
            var args = new object[] {
                    '0',
                    true,
                    (sbyte)2,
                    (byte)3,
                    (short)4,
                    (ushort)5,
                    (int)6,
                    (uint)7,
                    (long)8,
                    (ulong)9,
                    (float)10,
                    (double)11,
                    (decimal)12,
                    (BigInteger)13,
                    arg
            };

            var sourceType = this.GetType();
            var methodName = nameof(LocalsAndArgsTestMethod);
            var method = sourceType.GetMethod(methodName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
            Assert.IsNotNull(method, $"Failed to resolve method {sourceType.Name}.{methodName}");

            return CompileAndRun(opCode, method, args[arg], args, (a, b) => ((IComparable)(a)).CompareTo(b) == 0);
        }
    }

}
