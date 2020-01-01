using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ILEngine.Tests;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using ILEngine.Models;

namespace ILEngine.Implementations.Tests
{
    [TestClass()]
    public class ILInstructionReaderTests :
        ILEngineTestsBase<ILEngineCompiled, ILStackFrameWithDiagnostics<ILEngineCompiled>>
    {
        //TODO: We don't want this exception.
        [ExpectedException(typeof(TargetInvocationException))]
        [TestMethod()]
        public void ConvertIConvertibleByteCodeTest()
        {

            Func<List<string>> IConvertibleByteCodeTests = () =>
                {
                    var failedTests = new List<string>();
                    var except = new[] { TypeCode.DateTime, TypeCode.DBNull, TypeCode.Empty, TypeCode.Object/*,TypeCode.Boolean, TypeCode.Char*/ };
                    var exceptTypes = new[] { typeof(DateTime), typeof(DBNull), typeof(object), typeof(string)/*, typeof(bool), typeof(char)*/ };
                    var typeCodeValues = Enum.GetValues(typeof(TypeCode)).Cast<TypeCode>().Except(except).ToList();
                    var convertType = typeof(Convert);
                    var minMaxValueFlags = BindingFlags.Public | BindingFlags.Static;


                    var converterMethodGroups = convertType.GetMethods().Where(x => x.Name.StartsWith("To")).ToLookup(x => x.Name);
                    foreach (var converterMethodGroup in converterMethodGroups)
                    {
                        var converterMethodName = converterMethodGroup.Key;
                        var converterMethods = converterMethodGroup.ToList();
                        var convertToType = converterMethods.First().ReturnType;
                        if (exceptTypes.Contains(convertToType) || convertToType == typeof(int))
                            continue;
                        ParameterInfo[] parameters;
                        var convertToThis = converterMethods
                        .FirstOrDefault(x =>
                            (parameters = x.GetParameters()).Length == 1 && parameters[0].ParameterType == x.ReturnType);

                        var convertFromInt = converterMethods
                            .FirstOrDefault(x => (parameters = x.GetParameters()).Length == 1 && parameters[0].ParameterType == typeof(int));

                        var defaultValue = convertFromInt.Invoke(null, new object[] { 0 });
                        if (defaultValue is null) failedTests.Add($"{convertToType.Name}.defaultValue is null");

                        TypeCode targetTypeCode = Convert.GetTypeCode(defaultValue);
                        switch (targetTypeCode) // switch isn't implemented yet :/
                        {
                            case TypeCode.Char:
                                {
                                    var maxValue = (object)(char)0;
                                    var minValue = (object)(char)ushort.MaxValue;
                                    var convertedMax = convertToThis.Invoke(null, new object[] { maxValue });
                                    if (convertedMax is null) failedTests.Add($"{convertToType.Name}.convertedMin is null");
                                    if (((IComparable)maxValue).CompareTo(convertedMax) != 0) failedTests.Add($"{convertToType.Name}.maxValue({maxValue}) != (convertedMax){convertedMax}");


                                    var convertedMin = convertToThis.Invoke(null, new[] { minValue });
                                    if (convertedMin is null) failedTests.Add($"{convertToType.Name}.convertedMin is null");
                                    if (((IComparable)minValue).CompareTo(convertedMin) != 0) failedTests.Add($"{convertToType.Name}.minValue({minValue}) != (convertedMin){convertedMin}");

                                }
                                break;
                            case TypeCode.Boolean:
                                {


                                    var maxValue = (object)true;
                                    var convertedMax = convertToThis.Invoke(null, new object[] { maxValue });
                                    if (convertedMax is null) failedTests.Add($"{convertToType.Name}.convertedMin is null");
                                    if (((IComparable)maxValue).CompareTo(convertedMax) != 0) failedTests.Add($"{convertToType.Name}.maxValue({maxValue}) != (convertedMax){convertedMax}");

                                    var minValue = (object)false;
                                    var convertedMin = convertToThis.Invoke(null, new[] { minValue });
                                    if (convertedMin is null) failedTests.Add($"{convertToType.Name}.convertedMin is null");
                                    if (((IComparable)minValue).CompareTo(convertedMin) != 0) failedTests.Add($"{convertToType.Name}.minValue({minValue}) != (convertedMin){convertedMin}");
                                }
                                break;
                            default:
                                {
                                    var maxValueField = convertToType.GetField("MaxValue", minMaxValueFlags);
                                    if (maxValueField is null && convertToType != typeof(bool)) failedTests.Add($"{convertToType.Name}.maxValueField is null");

                                    var maxValue = maxValueField.GetValue(null);
                                    if (maxValue is null) failedTests.Add($"{convertToType.Name}.maxValue is null");


                                    var convertedMax = convertToThis.Invoke(null, new[] { maxValue });
                                    if (convertedMax is null) failedTests.Add($"{convertToType.Name}.convertedMax is null");

                                    if (((IComparable)maxValue).CompareTo(convertedMax) != 0) failedTests.Add($"{convertToType.Name}.maxValue({maxValue}) != (convertedMax){convertedMax}");

                                    var minValueField = convertToType.GetField("MinValue", minMaxValueFlags);
                                    if (minValueField is null) failedTests.Add($"{convertToType.Name}.minValueField is null");

                                    var minValue = minValueField.GetValue(null);
                                    if (minValue is null) failedTests.Add($"{convertToType.Name}.minValue is null");
                                    var convertedMin = convertToThis.Invoke(null, new[] { minValue });
                                    if (convertedMin is null) failedTests.Add($"{convertToType.Name}.convertedMin is null");
                                    if (((IComparable)minValue).CompareTo(convertedMin) != 0) failedTests.Add($"{convertToType.Name}.minValue({minValue}) != (convertedMin){convertedMin}");
                                }
                                break;

                        }

                        foreach (var converterMethod in converterMethods)
                        {
                            parameters = converterMethod.GetParameters();
                            if (parameters.Length == 1 && !exceptTypes.Contains(parameters[0].ParameterType))
                            {
                                var paramDefault = Convert.ChangeType(0, parameters[0].ParameterType);
                                TypeCode paramTypeCode = Convert.GetTypeCode(paramDefault);
                                if (targetTypeCode == TypeCode.Boolean && paramTypeCode == TypeCode.Char)
                                    continue;
                                if (targetTypeCode == TypeCode.Char && (paramTypeCode == TypeCode.Boolean || paramTypeCode == TypeCode.Single || paramTypeCode == TypeCode.Double || paramTypeCode == TypeCode.Decimal))
                                    continue;
                                if (paramTypeCode == TypeCode.Char && (targetTypeCode == TypeCode.Boolean || targetTypeCode == TypeCode.Single || targetTypeCode == TypeCode.Double || targetTypeCode == TypeCode.Decimal))
                                    continue;
                                //var inline = Convert.ToBoolean('0');

                                var convertFromDefault = converterMethod.Invoke(null, new object[] { paramDefault });
                                if (convertFromDefault is null) failedTests.Add($"{converterMethod.Name}({defaultValue}) method is null");
                                var convertToBoolInt = Convert.ToInt32(convertFromDefault);
                                if (convertToBoolInt != 0) failedTests.Add($"{converterMethod.Name}({convertFromDefault}{{0}}) !=0");
                            }
                        }

                    }
                    return failedTests;
                };

            //make sure the test is succesful in c#
            List<string> expected = IConvertibleByteCodeTests();
            var engine = new ILEngine.ILInstructionEngine();
            var engineResult = engine.ExecuteTyped<List<string>>(IConvertibleByteCodeTests.Method, IConvertibleByteCodeTests.Target);
        }

        void AssertResult(ILStackFrameWithDiagnostics<ILEngineCompiled> frame, object expected, OperandType operandType)
        {
            var instructions = frame.Stream;
            Assert.IsTrue(instructions.Any(x => x.OpCode.OperandType == operandType),
               $"Instructions do not contain Opcode of type {operandType}"
               );
            AssertEmptyStackWithResult(frame, expected);
        }

        void ExecuteAndAssertResult(ILStackFrameWithDiagnostics<ILEngineCompiled> frame, object expected, OperandType operandType)
        {
            Execute(frame);
            AssertResult(frame, expected, operandType);
            //AssertEmptyStackWithResult(frame, expected);
        }

        void ExecuteAndAssertResult(List<ILInstruction> instructions, object expected, OperandType operandType)
        {
            var frame = Build(instructions);
            ExecuteAndAssertResult(frame, expected, operandType);
        }

        void TestFromMethod(Func<MethodInfo> mResolver, OperandType operandType)
        {
            var method = mResolver();
            Assert.IsNotNull(method, "Failed to resolve test method");
            var expected = method.Invoke(null, Type.EmptyTypes);
            var instructions = ILInstructionReader.FromMethod(method);
            var frame = Build(instructions);
            frame.CopyFrom(method);
            ExecuteAndAssertResult(frame, expected, operandType);

        }

        [TestMethod]
        public void ReadILMethodBody()
        {
            var ilString = @"
IL_0000: ldarg.0
IL_0001: brtrue.s IL_000e
IL_0003: ldstr ""structure""
IL_0008: newobj instance void System.ArgumentNullException::.ctor(string)
IL_000d: throw
IL_000e: ldarg.0
IL_000f: callvirt instance class System.Type System.Object::GetType()
IL_0014: ldc.i4.1
IL_0015: call int32 System.Runtime.InteropServices.Marshal::SizeOfHelper(class System.Type, bool)
IL_001a: ret";
            var instructions = ILInstructionReader.FromILMethodBodyString(ilString);

            var frame = Build(instructions.ToArray());
            frame.Args = new object[] { 1 };
            Execute(frame);
            AssertEmptyStackWithResult(frame, 4);
        }

        [TestMethod()]
        public void TestReadInlineBrTarget()
        {
            var builder = new ILInstructionBuilder();
            builder.Write(ILInstruction.Create(ILOpCodeValues.Br, 0));
            builder.Write(ILOpCodeValues.Ldc_I8, 1L);
            builder.Write(ILOpCodeValues.Ret);
            var ilMethod = new Models.ILMethod(MethodBase.GetCurrentMethod().Name, typeof(long));
            ilMethod.AddInstructions(builder.Instructions.ToArray());
            var method = ilMethod.Compile();

            var expected = method.Invoke(null, Type.EmptyTypes);
            var instructions = ILInstructionReader.FromMethod(method);
            ExecuteAndAssertResult(instructions, expected, OperandType.InlineBrTarget);


        }


        public class FieldTestClass
        {
            public int Value = 1;
        }

        public static int TestReadInlineFieldMethod() => new FieldTestClass().Value;
        [TestMethod()]
        public void TestReadInlineField()
        {

            var method = this.GetType().GetMethod(nameof(TestReadInlineFieldMethod),
                BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.InlineField);

        }

        public static int TestReadInlineIMethod()
        {
            int a = 1000;
            unsafe { return *(int*)&a; }
        }

        [TestMethod()]
        public void TestReadInlineI()
        {
            var method = this.
                GetType().
                GetMethod(nameof(TestReadInlineIMethod), BindingFlags.Public | BindingFlags.Static);

            TestFromMethod(() => method, OperandType.InlineI);


        }
        public static long TestReadInlineI8Method()
        {
            long a = 10000000000;
            unsafe { return *(long*)&a; }
        }

        [TestMethod()]
        public void TestReadInlineI8()
        {
            var method = this.
                GetType().
                GetMethod(nameof(TestReadInlineI8Method), BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.InlineI8);
        }

        public static DestClass TestReadInlineMethodMethod() => new DestClass();
        [TestMethod()]
        public void TestReadInlineMethod()
        {
            var method = this.
               GetType().
               GetMethod(nameof(TestReadInlineMethodMethod), BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.InlineMethod);
        }

        public static int TestReadInlineNoneMethod() => 1;
        [TestMethod()]
        public void TestReadInlineNone()
        {
            var method = this.
              GetType().
              GetMethod(nameof(TestReadInlineNoneMethod), BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.InlineNone);
        }

        [ExpectedException(typeof(NotImplementedException))]
        [TestMethod()]
        public void TestReadInlinePhi()
        {
            throw new NotImplementedException();
        }

        public static double TestReadInlineRMethod() => double.MinValue;

        [TestMethod()]
        public void TestReadInlineR()
        {
            var method = this.
                GetType().
                GetMethod(nameof(TestReadInlineRMethod), BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.InlineR);
        }

        public static byte[] TestReadInlineSigMethod()
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance;

            Func<MethodInfo, bool> mFilter = (m) => m.GetMethodBody()?.LocalSignatureMetadataToken > 0;

            var method = typeof(ILInstructionReaderTests)
                .Assembly
                .ExportedTypes
                .First(t =>
                    t.GetMethods(bindingFlags).Any(m => mFilter(m))
                 )
                .GetMethods(bindingFlags)
                .First(mFilter);

            var body = method.GetMethodBody();
            var signature = body.LocalSignatureMetadataToken;
            var resolved = method.Module.ResolveSignature(signature);
            return resolved;
        }

        public static int TestReadInlineSigMethod1()
        {
            int index = int.Parse("1");
            int x = 1;
            int y = 1;
            List<Func<int, int, int>> funcs =
                (new[]
                {
                   (Func<int, int, int>)( (a, b) => a+b),
                   (Func<int, int, int>)( (a, b) => a-b),
                }
                ).ToList();
            return funcs[index](x, y);

        }

        public struct kernel
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr GetModuleHandle(string name);
            public delegate IntPtr GetProcAddress(IntPtr module, string name);
        }



        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //public delegate IntPtr GetModuleHandlDel(string name);




        //[DllImport("kernel32.dll", SetLastError = true)]
        //public static extern IntPtr GetProcAddress(IntPtr module, string name);

        //[UnmanagedFunctionPointer(CallingConvention.StdCall)]

        public static IntPtr TestReadInlineSigMethod2()
        {
            var add = kernel.GetModuleHandle("kernel32.dll");

            kernel.GetProcAddress d = Marshal.GetDelegateForFunctionPointer<kernel.GetProcAddress>(add);
            var w = d(add, "GetModuleHandleW");
            return w;

            //var address = GetProcAddress(kernelPtr, "GetModuleHandleW");



            //d.DynamicInvoke(kernelPtr, "GetModuleHandleW");
            //var result = d(kernelPtr, "GetModuleHandleW");

            //var result1 = d.DynamicInvoke(Type.EmptyTypes);
            //return kernelPtr;
        }

        [ExpectedException(typeof(NotImplementedException))]
        [TestMethod()]
        public void TestReadInlineSig()
        {
            TestReadInlineSigMethod2();
            var method = this
               .GetType()
               .GetMethod(nameof(TestReadInlineSigMethod2), BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.InlineSig);
        }



        public static string TestReadInlineStringMethod() => "test";
        [TestMethod()]
        public void TestReadInlineString()
        {
            var method = this.
                GetType().
            GetMethod(nameof(TestReadInlineStringMethod), BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.InlineString);
        }


        public static int TestReadInlineSwitchMethod()
        {
            int a = int.Parse("1");
            switch (a)
            {
                case 0: return 1;
                case 1: return 2;
                case 2: return 3;
                case 3: return 4;
                case 4: return 5;
                default: return -1;
            }

        }
        [TestMethod()]
        public void TestReadInlineSwitch()
        {
            var method = this.
                GetType().
                GetMethod(nameof(TestReadInlineSwitchMethod), BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.InlineSwitch);
        }

        public static FieldInfo TestReadInlineTokMethod() => typeof(DestClass).GetField(nameof(DestClass.Value));
        [TestMethod()]
        public void TestReadInlineTok()
        {
            var method = this.
               GetType().
               GetMethod(nameof(TestReadInlineTokMethod), BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.InlineTok);
        }

        public static int TestReadInlineTypeMethod()
        {
            var a = Activator.CreateInstance(typeof(FieldTestClass));
            var b = (a is FieldTestClass) ? 1 : 0;
            return b;
        }
        [TestMethod()]
        public void TestReadInlineType()
        {
            var method = this.
                        GetType().
                        GetMethod(nameof(TestReadInlineTypeMethod), BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.InlineType);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style",
            "IDE0059:Unnecessary assignment of a value", Justification = "Values need to force MSIL InlineVar generation")]


        public static int TestReadInlineVarMethod()
        {
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            int a0, a1, a2, a3, a4, a5, a6, a7, a8, a9;
            a0 = a1 = a2 = a3 = a4 = a5 = a6 = a7 = a8 = a9 = 1;

            int a10, a11, a12, a13, a14, a15, a16, a17, a18, a19;

            a10 = a11 = a12 = a13 = a14 = a15 = a16 = a17 = a18 = a19 = 1;
            int a20, a21, a22, a23, a24, a25, a26, a27, a28, a29;
            a20 = a21 = a22 = a23 = a24 = a25 = a26 = a27 = a28 = a29 = 1;
            int a30, a31, a32, a33, a34, a35, a36, a37, a38, a39;
            a30 = a31 = a32 = a33 = a34 = a35 = a36 = a37 = a38 = a39 = 1;
            int a40, a41, a42, a43, a44, a45, a46, a47, a48, a49;
            a40 = a41 = a42 = a43 = a44 = a45 = a46 = a47 = a48 = a49 = 1;
            int a50, a51, a52, a53, a54, a55, a56, a57, a58, a59;
            a50 = a51 = a52 = a53 = a54 = a55 = a56 = a57 = a58 = a59 = 1;
            int a60, a61, a62, a63, a64, a65, a66, a67, a68, a69;
            a60 = a61 = a62 = a63 = a64 = a65 = a66 = a67 = a68 = a69 = 1;
            int a70, a71, a72, a73, a74, a75, a76, a77, a78, a79;
            a70 = a71 = a72 = a73 = a74 = a75 = a76 = a77 = a78 = a79 = 1;
            int a80, a81, a82, a83, a84, a85, a86, a87, a88, a89;
            a80 = a81 = a82 = a83 = a84 = a85 = a86 = a87 = a88 = a89 = 1;
            int a90, a91, a92, a93, a94, a95, a96, a97, a98, a99;
            a90 = a91 = a92 = a93 = a94 = a95 = a96 = a97 = a98 = a99 = 1;

            int a100, a101, a102, a103, a104, a105, a106, a107, a108, a109;
            a100 = a101 = a102 = a103 = a104 = a105 = a106 = a107 = a108 = a109 = 1;
            int a110, a111, a112, a113, a114, a115, a116, a117, a118, a119;
            a110 = a111 = a112 = a113 = a114 = a115 = a116 = a117 = a118 = a119 = 1;
            int a120, a121, a122, a123, a124, a125, a126, a127, a128, a129;
            a120 = a121 = a122 = a123 = a124 = a125 = a126 = a127 = a128 = a129 = 1;
            int a130, a131, a132, a133, a134, a135, a136, a137, a138, a139;
            a130 = a131 = a132 = a133 = a134 = a135 = a136 = a137 = a138 = a139 = 1;
            int a140, a141, a142, a143, a144, a145, a146, a147, a148, a149;
            a140 = a141 = a142 = a143 = a144 = a145 = a146 = a147 = a148 = a149 = 1;
            int a150, a151, a152, a153, a154, a155, a156, a157, a158, a159;
            a150 = a151 = a152 = a153 = a154 = a155 = a156 = a157 = a158 = a159 = 1;
            int a160, a161, a162, a163, a164, a165, a166, a167, a168, a169;
            a160 = a161 = a162 = a163 = a164 = a165 = a166 = a167 = a168 = a169 = 1;
            int a170, a171, a172, a173, a174, a175, a176, a177, a178, a179;
            a170 = a171 = a172 = a173 = a174 = a175 = a176 = a177 = a178 = a179 = 1;
            int a180, a181, a182, a183, a184, a185, a186, a187, a188, a189;
            a180 = a181 = a182 = a183 = a184 = a185 = a186 = a187 = a188 = a189 = 1;
            int a190, a191, a192, a193, a194, a195, a196, a197, a198, a199;
            a190 = a191 = a192 = a193 = a194 = a195 = a196 = a197 = a198 = a199 = 1;


            int a200, a201, a202, a203, a204, a205, a206, a207, a208, a209;
            a200 = a201 = a202 = a203 = a204 = a205 = a206 = a207 = a208 = a209 = 1;
            int a210, a211, a212, a213, a214, a215, a216, a217, a218, a219;
            a210 = a211 = a212 = a213 = a214 = a215 = a216 = a217 = a218 = a219 = 1;
            int a220, a221, a222, a223, a224, a225, a226, a227, a228, a229;
            a220 = a221 = a222 = a223 = a224 = a225 = a226 = a227 = a228 = a229 = 1;
            int a230, a231, a232, a233, a234, a235, a236, a237, a238, a239;
            a230 = a231 = a232 = a233 = a234 = a235 = a236 = a237 = a238 = a239 = 1;
            int a240, a241, a242, a243, a244, a245, a246, a247, a248, a249;
            a240 = a241 = a242 = a243 = a244 = a245 = a246 = a247 = a248 = a249 = 1;
            int a250, a251, a252, a253, a254, a255, a256, a257, a258, a259;
            a250 = a251 = a252 = a253 = a254 = a255 = a256 = a257 = a258 = a259 = 1;
            int a260, a261, a262, a263, a264, a265, a266, a267, a268, a269;
            a260 = a261 = a262 = a263 = a264 = a265 = a266 = a267 = a268 = a269 = 1;
            int a270, a271, a272, a273, a274, a275, a276, a277, a278, a279;
            a270 = a271 = a272 = a273 = a274 = a275 = a276 = a277 = a278 = a279 = 1;
            int a280, a281, a282, a283, a284, a285, a286, a287, a288, a289;
            a280 = a281 = a282 = a283 = a284 = a285 = a286 = a287 = a288 = a289 = 1;
            int a290, a291, a292, a293, a294, a295, a296, a297, a298, a299;
            a290 = a291 = a292 = a293 = a294 = a295 = a296 = a297 = a298 = a299 = 1;
#pragma warning restore CS0219 // Variable is assigned but its value is never used
            return a257;
        }

        [TestMethod()]
        public void TestReadInlineVar()
        {
            var method = this.
                        GetType().
                        GetMethod(nameof(TestReadInlineVarMethod), BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.InlineVar);
        }


        public static int TestReadShortInlineBrTargetMethod()
        {
            int a = int.Parse("1");
            switch (a)
            {
                case 2: return 2;
                default: return a;
            }
        }

        [TestMethod()]
        public void TestReadShortInlineBrTarget()
        {
            var method = this.
                        GetType().
                        GetMethod(nameof(TestReadShortInlineBrTargetMethod), BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.ShortInlineBrTarget);
        }

        public static int TestReadShortInlineIMethod()
        {
            //byte a = 1;
            //unsafe { return *(int*)&a; }
            return 100;
        }

        [TestMethod()]
        public void TestReadShortInlineI()
        {
            var method = this.
                        GetType().
                        GetMethod(nameof(TestReadShortInlineIMethod), BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.ShortInlineI);
        }


        public static float TestReadShortInlineRMethod() => float.MinValue;
        [TestMethod()]
        public void TestReadShortInlineR()
        {

            var method = this.
                         GetType().
                         GetMethod(nameof(TestReadShortInlineRMethod), BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.ShortInlineR);
        }

        public static float TestReadShortInlineVarMethod()
        {
            //emit Ldloc_S 
            int a0, a1, a2, a3, a4, a5, a6, a7;
            a0 = a1 = a2 = a3 = a4 = a5 = a6 = a7 = 1;
            int a8 = a0 + a1 + a2 + a3 + a4 + a5 + a6 + a7;
            return a8;


        }
        [TestMethod()]
        public void TestReadShortInlineVar()
        {
            var method = this.
                         GetType().
                         GetMethod(nameof(TestReadShortInlineVarMethod), BindingFlags.Public | BindingFlags.Static);
            TestFromMethod(() => method, OperandType.ShortInlineVar);
        }
    }
}