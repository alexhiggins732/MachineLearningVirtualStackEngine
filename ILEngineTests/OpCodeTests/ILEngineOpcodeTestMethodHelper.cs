using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ILEngine.Tests
{
    public class ILEngineOpcodeTestMethodHelper
    {
        public static List<CompiledTest> CompiledTests => GetCompiledTests();
        static List<CompiledTest> GetCompiledTests()
        {
            var compiledTests = new List<CompiledTest>();
            var methods = typeof(TestMethods).GetMethods(BindingFlags.Public | BindingFlags.Static).ToList();

            foreach (var method in methods)
            {

                var opCodeTestAttribute = method.GetCustomAttribute(typeof(OpCodeTestAttribute));


                if (opCodeTestAttribute is OpCodeIntTestAttribute intTest)
                {
                    compiledTests.Add(new CompiledTest(method, intTest));
                }
                else if (opCodeTestAttribute is OpCodeIntCompareTestAttribute intCompareTest)
                {
                    compiledTests.Add(new CompiledTest(method, intCompareTest));
                }
                else if (opCodeTestAttribute is OpCodeUIntTestAttribute uintTest)
                {
                    compiledTests.Add(new CompiledTest(method, uintTest));
                }
                else if (opCodeTestAttribute is OpCodeExpectedExceptionAttribute excTest)
                {
                    compiledTests.Add(new CompiledTest(method, excTest));
                }

            }
            return compiledTests;
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
                //case ILOpCodeValues.Arglist: return GetByName(nameof(TestMethods.Arglist));
                case ILOpCodeValues.Beq: return GetFunc<int, int, int>(TestMethods.Br_S);
                case ILOpCodeValues.Beq_S: return GetFunc<int, int, int>(TestMethods.Br_S);
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
            var resolver = new ILInstructionResolver(method);
            switch (method.Name)
            {
                case nameof(TestMethods.Add_Ovf):
                case nameof(TestMethods.Add_Ovf_Un):
                    {
                        var instructions = ILInstructionReader.FromMethod(method);
                        var parsed = Enum.TryParse<ILOpCodeValues>(method.Name, out ILOpCodeValues result);
                        var opCode = OpCodeLookup.GetILOpcode((int)result);
                        instructions[2] = new ILInstruction() { OpCode = opCode };
                        
                        var compiled = ILEngineOpCodeTestMethodCompiler.CompileMethod(opCode, method, (gen) =>
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

        /// <summary>
        /// Contains methods to be invoked as part of unit tests. MSIL from these methods will be extracted and 
        /// and executed using an <see cref="ILEngine.IILEngine"/> to assure the the engine executes in the same manner
        /// as statically compiled code. These tests should also be built manually using an <see cref="ILInstructionBuilder"/>
        /// to ensure the builder API contains an completed implementation of compiled code.
        /// </summary>
        public class TestMethods
        {

            [OpCodeIntTest(2, 1, 1)]
            public static int Add(int a, int b) => unchecked(a + b);


            [OpCodeIntTest(2, 1, 1)]
            public static int Add_Ovf(int a, int b) => checked(a + b);

            [OpCodeUIntTest(2u, 1u, 1u)]
            public static uint Add_Ovf_Un(uint a, uint b) => checked(a + b);

            [OpCodeIntTest(1, 1, 1)]
            public static int And(int a, int b) => a & b;

            //TODO: Implement. After upgrading to .Net Framework 4.8, this method gets compiled in Anonymously Hosted DynamicMethods Assembly
            //  which the token resolver is not resolving correctly.
            //[OpCodeExpectedException(typeof(OpCodeNotImplementedException), null)]
            //public static object Arglist(__arglist) => throw new OpCodeNotImplementedException(ILOpCodeValues.Arglist);


            [OpCodeIntTest(1, 1, 1)]
            public static int Brfalse_S(int a, int b)
            {
                //var test= ILOpCodeValues.Brfalse_S
                if (a == b)//ld.arg0, ldarg.1. cmp, breq [4 bytes]=> 8 bytes
                    return 1;
                else
                    return 0;

            }


            [OpCodeIntTest(1, 1, 1)]
            public static int Br_S(int a, int b)
            {
                //var testCode = ILOpCodeValues.Br_S;
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


            [OpCodeUIntCompareTestAttribute(true, 1u, 1u)]
            public static bool Clt_Un(uint a, uint b) => a <= b;


            [OpCodeIntCompareTestAttribute(true, 2, 1)]
            public static bool Cgt(int a, int b) => a > b;


            [OpCodeUIntCompareTestAttribute(true, 2u, 1u)]
            public static bool Cgt_Un(uint a, uint b) => a >= b;

            //public static void Nop() { }
            //public static void Break() { System.Diagnostics.Debugger.Break(); }

        }
    }
}
