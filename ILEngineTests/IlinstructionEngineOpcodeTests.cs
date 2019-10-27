using ILEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngineTests
{

    public class OpCodeTestAttribute : Attribute
    {

    }
    [AttributeUsage(AttributeTargets.Method)]
    public class OpCodeTestObjectAttribute : OpCodeTestAttribute
    {
        public object Arg1 { get; set; }
        public object Arg2 { get; set; }
        public object Expected { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OpCodeIntTestAttribute : OpCodeTestAttribute
    {
        public int[] Parameters;

        public OpCodeIntTestAttribute(params int[] args)
        {
            Expected = args[0];
            Parameters = args.Skip(1).ToArray();
        }


        public int Expected { get; set; }
        public bool Equals(int a, int b) => a == b;
        public bool LessThan(int a, int b) => a < b;
        public bool LessThanOrEqual(int a, int b) => a <= b;
        public bool GreaterThanOrEqual(int a, int b) => a >= b;
        public bool GreaterThan(int a, int b) => a > b;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OpCodeUIntTestAttribute : OpCodeTestAttribute
    {
        public uint[] Parameters;

        public OpCodeUIntTestAttribute(params uint[] args)
        {
            Expected = args[0];
            Parameters = args.Skip(1).ToArray();
        }


        public uint Expected { get; set; }
        public bool Equals(uint a, uint b) => a == b;
        public bool LessThan(uint a, uint b) => a < b;
        public bool LessThanOrEqual(uint a, uint b) => a <= b;
        public bool GreaterThanOrEqual(uint a, uint b) => a >= b;
        public bool GreaterThan(uint a, uint b) => a > b;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OpCodeExpectedExceptionAttribute : OpCodeTestAttribute
    {
        public Type ExpectedExceptionType;
        public object[] Parameters;
        public object Expected;
        public OpCodeExpectedExceptionAttribute(Type expectedExceptionType, object expected, params object[] parameters)
        {
            this.ExpectedExceptionType = expectedExceptionType;
            this.Expected = expected;
            this.Parameters = parameters.ToArray();
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OpCodeIntCompareTestAttribute : OpCodeTestAttribute
    {
        public int[] Parameters;

        public OpCodeIntCompareTestAttribute(bool expected, params int[] args)
        {
            Expected = expected;
            Parameters = args.ToArray();
        }


        public bool Expected { get; set; }
        public bool Equals(bool a, bool b) => a == b;
        //public bool LessThan(int a, int b) => a < b;
        //public bool LessThanOrEqual(int a, int b) => a <= b;
        //public bool GreaterThanOrEqual(int a, int b) => a >= b;
        //public bool GreaterThan(int a, int b) => a > b;
    }

    public class CompiledTest
    {
        public MethodInfo Method;
        public OpCodeTestAttribute TestAttribute;
        public ILOpCodeValues ILOpCodeValue;
        public OpCode OpCode;
        public CompiledTest(MethodInfo method, OpCodeIntTestAttribute testAttribute)
        {
            this.Method = method;
            var parsed = Enum.TryParse(method.Name, out ILOpCodeValues ILOpCodeValue);
            this.OpCode = OpCodeLookup.GetILOpcode((int)ILOpCodeValue);

            this.TestAttribute = testAttribute;
        }
    }

    public class ILinstructionEngineOpcodeTestMethodHelper
    {
        public static List<CompiledTest> CompiledTests;
        static ILinstructionEngineOpcodeTestMethodHelper()
        {
            CompiledTests = new List<CompiledTest>();
            var methods = typeof(TestMethods).GetMethods(BindingFlags.Public | BindingFlags.Static).ToList();

            foreach (var method in methods)
            {
                var intTest = (OpCodeIntTestAttribute)method.GetCustomAttribute(typeof(OpCodeIntTestAttribute));
                if (intTest != null)
                {
                    CompiledTests.Add(new CompiledTest(method, intTest));

                }
            }
        }

        private static void RunIntOpCodeTest(MethodInfo method, OpCodeIntTestAttribute intTest)
        {
            throw new NotImplementedException();
        }

        public static MethodInfo GetMethodForTest(ILOpCodeValues value)
        {
            switch (value)
            {
                case ILOpCodeValues.Add: return GetFunc<int, int, int>(TestMethods.Add);
                case ILOpCodeValues.Add_Ovf: return GetCustomFunc<int, int, int>(TestMethods.Add_Ovf);
                case ILOpCodeValues.Add_Ovf_Un: return GetCustomFunc<uint, uint, uint>(TestMethods.Add_Ovf_Un);
                case ILOpCodeValues.And: return GetFunc<int, int, int>(TestMethods.Add_Ovf);
                case ILOpCodeValues.Arglist: return GetByName(nameof(TestMethods.Arglist));
                case ILOpCodeValues.Beq: return GetFunc<int, int, int>(TestMethods.Beg);
                case ILOpCodeValues.Beq_S: return GetFunc<int, int, int>(TestMethods.Beg);
                default: throw new NotImplementedException();
            }
        }


        private static MethodInfo GetAction(Action target)
        {
            return target.Method;
        }

        private static MethodInfo GetByName(string name) => typeof(TestMethods).GetMethod(name, BindingFlags.Public | BindingFlags.Static);
        private static MethodInfo GetFunc<T1>(Func<T1> target)
        {
            return target.Method;
        }
        private static MethodInfo GetFunc<T1, T2>(Func<T1, T2> target)
        {
            return target.Method;
        }
        private static MethodInfo GetCustomFunc<T1, T2, T3>(Func<T1, T2, T3> target)
        {
            var method = target.Method;
            var body = method.GetMethodBody();
            var locals = body.LocalVariables.OrderBy(x => x.LocalIndex).ToList();
            var resolver = new IlInstructionResolver(method);
            switch (method.Name)
            {
                case nameof(TestMethods.Add_Ovf):
                case nameof(TestMethods.Add_Ovf_Un):
                    {
                        var instructions = IlInstructionReader.FromMethod(method);
                        var parsed = Enum.TryParse<ILOpCodeValues>(method.Name, out ILOpCodeValues result);
                        var opCode = OpCodeLookup.GetILOpcode((int)result);
                        instructions[2] = new IlInstruction() { OpCode = opCode };
                        var compiled = IlinstructionEngineOpcodeTests.CompileMethod(opCode, method, (gen) =>
                          {
                              locals.ForEach(x => gen.DeclareLocal(x.LocalType));
                              instructions.ForEach(x => x.Emit(gen, resolver));
                          });
                        return method;
                    }

                default: throw new NotImplementedException();
            }

        }

        private static MethodInfo GetFunc<T1, T2, T3>(Func<T1, T2, T3> target)
        {
            return target.Method;
        }

        public class TestMethods
        {

            [OpCodeIntTest(2, 1, 1)]
            public static int Add(int a, int b) => unchecked(a + b);


            [OpCodeIntTest(2, 1, 1)]
            public static int Add_Ovf(int a, int b) => a + b;

            [OpCodeUIntTest(2u, 1u, 1u)]
            public static uint Add_Ovf_Un(uint a, uint b) => a + b;

            [OpCodeIntTest(1, 1, 1)]
            public static int And(int a, int b) => a & b;

            [OpCodeExpectedException(typeof(NotImplementedException), null)]
            public static object Arglist(__arglist) => throw new NotImplementedException();


            [OpCodeIntTest(1, 1, 1)]
            public static int Beg_S(int a, int b)
            {
                if (a == b)//ld.arg0, ldarg.1. cmp, breq [4 bytes]=> 8 bytes
                    goto BEQ;
                goto OtherTarget;
                BEQ:
                return a;
                OtherTarget:
                return a ^ b;
            }


            [OpCodeIntTest(1, 1, 1)]
            public static int Beg(int a, int b)
            {
                if (a == b)//ld.arg0, ldarg.1. cmp, breq [4 bytes]=> 8 bytes
                    goto BEQ;
                // 128 bytes of instructions to force a long jump
                var filler_0 = ulong.MaxValue;//Ldc_I8 arg[8bytes] -> 9 bytes
                var filler_1 = ulong.MaxValue;//Ldc_I8 arg[8bytes] -> 9 bytes
                var filler_2 = ulong.MaxValue;//Ldc_I8 arg[8bytes] -> 9 bytes
                var filler_3 = ulong.MaxValue;//Ldc_I8 arg[8bytes] -> 9 bytes
                var filler_4 = ulong.MaxValue;//Ldc_I8 arg[8bytes] -> 9 bytes
                var filler_5 = ulong.MaxValue;//Ldc_I8 arg[8bytes] -> 9 bytes
                var filler_6 = ulong.MaxValue;//Ldc_I8 arg[8bytes] -> 9 bytes
                var filler_7 = ulong.MaxValue;//Ldc_I8 arg[8bytes] -> 9 bytes
                var filler_8 = ulong.MaxValue;//Ldc_I8 arg[8bytes] -> 9 bytes
                var filler_9 = ulong.MaxValue;//Ldc_I8 arg[8bytes] -> 9 bytes
                var filler_10 = ulong.MaxValue;//Ldc_I8 arg[8bytes] -> 9 bytes
                var filler_11 = ulong.MaxValue;//Ldc_I8 arg[8bytes] -> 9 bytes
                var filler_12 = ulong.MaxValue;//Ldc_I8 arg[8bytes] -> 9 bytes
                if ((filler_0 | filler_1) == filler_2 && filler_3 == filler_4 && filler_5 == filler_6 && filler_7 == filler_8 && filler_9 == filler_10 && filler_11 == filler_12)
                {
                    goto OtherTarget;
                }
                else
                {
                    goto OtherTarget2;
                }
                BEQ:
                return a;
                OtherTarget:
                return a ^ b;
                OtherTarget2:
                return b ^ a;
            }



            [OpCodeIntCompareTestAttribute(true, 0, 1)]
            public static bool Ceq(int a, int b) => a == b;


            [OpCodeIntCompareTestAttribute(true, 1, 1)]
            public static bool Clt(int a, int b) => a < b;


            [OpCodeIntCompareTestAttribute(true, 1, 1)]
            public static bool Cle(int a, int b) => a <= b;


            [OpCodeIntCompareTestAttribute(true, 2, 1)]
            public static bool Cgt(int a, int b) => a > b;


            [OpCodeIntCompareTestAttribute(true, 2, 1)]
            public static bool Cge(int a, int b) => a >= b;



            public static void Nop() { }
            public static void Break() { System.Diagnostics.Debugger.Break(); }

        }
    }
    [TestClass()]
    public class IlinstructionEngineOpcodeTests
    {
        private IlInstructionEngine engine = new ILEngine.IlInstructionEngine();


        /// <summary>
        /// Runs unit tests on all methods defined in <see cref="TestMethods"/> that have a <see cref="OpCodeTestAttribute"/> defined
        /// using the arguments specified in the attribute.
        /// </summary>
        /// <remarks>
        /// IL code from the compiled method is first read from the as a byte array of native IL instructions which are then read
        /// as <see cref="IlInstruction"/> using the <see cref="IlInstructionReader"/>. 
        /// The IL instructions are then used to compile a dynamic method using a <see cref="System.Reflection.Emit.MethodBuilder"/>
        /// to asser that the <see cref="IlInstructionReader"/> reads the instructions correctly
        /// and that <see cref="IlInstruction.Emit(ILGenerator, IIlInstructionResolver)"/> method
        /// generates the correct <see cref="IlInstruction.OpCode"/> and optional <see cref="IlInstruction.Arg"/>.
        /// The dynamically compiled method is invoked and its result is compared to the compiled <see cref="CompiledTest.Method"/>
        /// result. The result is then compared to the result produced using <see cref="IlInstructionEngine"/> to run it on the VM
        /// with  <see cref="IlInstructionEngine.ExecuteTyped(MethodInfo, object[])"/>.
        /// </remarks>
        [TestMethod()]
        public void CompiledOpCodeTests()
        {
            var tests = ILinstructionEngineOpcodeTestMethodHelper.CompiledTests;
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

        [TestMethod()]
        public void TestAllOpCodesInEngine()
        {
            var opCodeFields = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static);
            ILOpCodeValues parseResult = ILOpCodeValues.Nop;
            var notCovered = new List<ILOpCodeValues>(Enum.GetValues(typeof(ILOpCodeValues)).Cast<ILOpCodeValues>());
            foreach (var field in opCodeFields)
            {
                var opCode = (OpCode)field.GetValue(null);

                var name = field.Name;
                var lookup = Enum.TryParse(name, out parseResult);
                if (parseResult == ILOpCodeValues.Break) continue;
                if (bool.Parse(bool.TrueString) || notCovered.Contains(parseResult))
                {
                    var used = RunTestForOpCode(opCode);
                    notCovered.RemoveAll(x => used.Contains(x));
                }
            }

        }


        public static MethodInfo CompileMethod(OpCode opCode, MethodInfo existingMethod, Action<ILGenerator> emitAction)
        {
            var parameterTypes = existingMethod.GetParameters().Select(x => x.ParameterType).ToArray();
            return CompileMethod(opCode, existingMethod.ReturnType, parameterTypes, emitAction);
        }
        private static MethodInfo CompileMethod(OpCode opCode, Type returnType, Type[] parameterTypes, Action<ILGenerator> emitAction)
        {
            var testName = "IlUnitTest" + opCode.Value.ToString();
            var asmName = new AssemblyName(testName);
            AppDomain domain = AppDomain.CurrentDomain;

            AssemblyBuilder wrapperAssembly =
                domain.DefineDynamicAssembly(asmName,
                    AssemblyBuilderAccess.RunAndSave);

            // wrapperAssembly.ModuleResolve += WrapperAssembly_ModuleResolve;
            var assemblyPath = asmName.Name + ".dll";

            ModuleBuilder wrapperModule =
                wrapperAssembly.DefineDynamicModule(asmName.Name,
                   assemblyPath);



            // Define a type to contain the method.
            TypeBuilder typeBuilder =
                wrapperModule.DefineType("testName", TypeAttributes.Public);

            //typeBuilder.DefineField("bigInt", typeof(BigInteger), FieldAttributes.Public);

            var mb = typeBuilder.DefineMethod("test", MethodAttributes.Public | MethodAttributes.Static, returnType, parameterTypes);

            var gen = mb.GetILGenerator();

            emitAction(gen);
            var type = typeBuilder.CreateType();
            var compiledMethod = type.GetMethod("test");
            var il = compiledMethod.GetMethodBody().GetILAsByteArray();
            var method = mb;
            return compiledMethod;
        }

        //private Module WrapperAssembly_ModuleResolve(object sender, ResolveEventArgs e)
        //{
        //   // var target = e.Name;

        //    //return typeof(System.Numerics.BigInteger).Module;
        //}

        public ILOpCodeValues[] CompileAndRun<T>(OpCode opCode, MethodInfo srcMethod, T expected, object[] parameters, Func<T, T, bool> comparer)
        {
            var methodParameters = srcMethod.GetParameters();
            var body = srcMethod.GetMethodBody();
            var locals = body.LocalVariables.OrderBy(x => x.LocalIndex).ToList();

            var parameterTypes = methodParameters.Select(x => x.ParameterType).ToArray();

            var instructions = IlInstructionReader.FromMethod(srcMethod);
            Assert.IsTrue(instructions.Any(x => x.OpCode == opCode));

            var result = IlInstructionReader
              .FromMethod(srcMethod)
              .Select(x => (ILOpCodeValues)x.OpCode.Value).ToArray();
            var resolver = new IlInstructionResolver(srcMethod);
            Action<ILGenerator> emitAction = (gen) =>
            {
                locals.ForEach(x => gen.DeclareLocal(x.LocalType));
                instructions.ForEach(x => x.Emit(gen, resolver));
            };

            MethodInfo method = null;
            try
            {
                method = CompileMethod(opCode, typeof(T), parameterTypes, emitAction);
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
            var result = IlInstructionReader
               .FromMethod(method)
               .Select(x => (ILOpCodeValues)x.OpCode.Value).ToArray();


            bool invokedResult = false;
            Exception caughtCompiled = null;
            try
            {
                var invoked = (T)method.Invoke(null, parameters);
                invokedResult = comparer(expected, invoked);

            }
            catch (Exception ex)
            {
                caughtCompiled = ex.InnerException;
            }

            var engineResult = false;
            Exception caughtEngine = null;
            try
            {
                var actualEngine = engine.ExecuteTyped<T>(method, parameters);
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

        public ILOpCodeValues[] CompileAndRun<T>(OpCode opCode, T expected, object[] parameters, Action<ILGenerator> emitAction, Func<T, T, bool> comparer)
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
                method = CompileMethod(opCode, typeof(T), parameterTypes, emitAction);
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to compile method:", ex.Message);
                return null;
            }
            return RunAndTest<T>(method, expected, parameters, comparer);

        }

        private static int TestLdArg0(int a) => a;
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

        private ILOpCodeValues[] RunTestForOpCode(OpCode opCode)
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
                    return CompileAndRun<int>(opCode, typeof(IlinstructionEngineOpcodeTests).GetMethod("TestLdArg0", BindingFlags.NonPublic | BindingFlags.Static), 1,
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


                default: throw new NotImplementedException();


            }
        }

        private ILOpCodeValues[] LocalsAndArgsTest(OpCode opCode, int arg)
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

            var method = typeof(IlinstructionEngineOpcodeTests).GetMethod(nameof(LocalsAndArgsTestMethod), BindingFlags.Public | BindingFlags.Static);
            return CompileAndRun(opCode, method, args[arg], args, (a, b) => ((IComparable)(a)).CompareTo(b) == 0);
        }
    }
}
