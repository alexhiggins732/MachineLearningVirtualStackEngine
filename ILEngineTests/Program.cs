using ILEngine.Compilers;
using ILEngine.Implementations.Tests;
using ILEngine.Tests;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ILEngineTests.ConvertWip;

namespace ILEngineTests
{
    [ExcludeFromCodeCoverage]
    public class Program
    {

        public static void Main(string[] args)
        {
            RunUnitTest();
        }

        static void RunUnitTest()
        {
          

            


            var test = new ILEngineCompiledTests();
            test.Cgt_Test();
            test.Clt_Test();
        }
        public struct ptr<T>
            where T : struct
        {
            public T Value;
            public ptr(T value) => this.Value = value;

            public object GetValue => Value;

        }
        public class GenericArray<T>
        {
            private object[] data;
            private Type type;

            public GenericArray(int length)
            {
                this.data = new object[length];
                type = typeof(T);



            }


            public uint GetValue(int index) => (uint)data[index];

            public void SetValue(object instance, int index) => data[index] = instance;
        }
        //public static T[] Cast<T>(Func<object> t) => (T[])t();
        public static T[] CastArray<T>(object t) => (T[])t;
        public static T Cast<T>(Func<T> resolver) => resolver();
        public static void SetElement<T>(object arr, object instance, int index)
        {
            var castedArr = CastArray<T>(arr);
            var castedInstance = Cast(() => instance);
            CastArray<T>(arr)[index] = (T)Convert.ChangeType(instance, typeof(T));
        }


        static sbyte fromU64(ulong val) { unsafe { return *((sbyte*)&val); } }

        static int ptrUInt16ToVoidToInt32()
        {
            ushort b = 1;
            unsafe
            {
                //int* p = (int*)&b;
                void* v = &b;
                //int pr = (int)*p;
                int* pr1 = (int*)v;
                return (int)*pr1;
            }
        }
        static int ptrU16Toi42Tests()
        {
            ushort b = 1;
            unsafe
            {
                int* p = (int*)&b;
                return *p;

            }
        }
        static int prtI1test()
        {
            I1 i1 = new I1 { value = 1 };
            unsafe
            {
                int* p = (int*)&i1;
                return *p;
            }
        }
        static uint prtI12U32test()
        {
            I1 i1 = new I1 { value = 1 };
            unsafe
            {
                uint* p = (uint*)&i1;
                return *p;
            }
        }



        public static sbyte ptrGenericShortTest()
        {
            var prti16 = new ptr<short>(2);
            unsafe
            {
                return *(SByte*)&(prti16.Value);
            }
        }

        public static sbyte genericPtrTest()
        {
            var ptr = new ptr<short>(2);
            unsafe
            {
                return *(SByte*)&(ptr.Value);
            }
        }

        public static ptr ptrTest()
        {

            ulong i81 = ulong.MaxValue;
            unsafe
            {
                ptr* p = (ptr*)&i81;
                return *p;
                //return new ptr { value = new IntPtr((void*)&i81) };
            }
        }

        public static ushort ToUInt16(ptr ptr)
        {
            unsafe
            {
                ulong dd = *(ulong*)&(ptr);
                //var padd = *(sbyte*)ptr.value.ToPointer();
                //var sb = (sbyte)ptr.value.ToPointer();
                return (ushort)dd;

            }
        }

        static void GenerateFrameCode()
        {
            var opCodeBodies = ILEngine.CodeGenerator.OpCodeBodyParser.ParseBodyFromIlEngineWithDiagnostics();
            var binder = ILEngine.CodeGenerator.OpCodeEngineGenerator.StackFrameMethodBinder(opCodeBodies);
            var frameCode = ILEngine.CodeGenerator.OpCodeEngineGenerator
                .GenerateFrameEngine("ILEngine.Implementations", "IlEngineCompiled", binder);
            //TODO:
            var engineSrceDirectoryPath = Path.GetFullPath("../../../IlEngine");
            var di = new DirectoryInfo(engineSrceDirectoryPath);
            if (di.Exists)
            {
                var GeneratedPath = Path.Combine(di.FullName, "CodeGeneration/GeneratedCode");
                if (Directory.Exists(GeneratedPath))
                {
                    var dest = Path.Combine(GeneratedPath, "IlEngineCompiled.cs");
                    File.WriteAllText(dest, frameCode);
                }
            }


            //File.WriteAllText(resultFile, frameCode);

        }
        public static void MainOld(string[] args)
        {

            GenerateFrameCode();


            var test = new ILInstructionEngineOpCodeTests();
            test.CompiledOpCodeTests();

            var lv = new ILEngine.ILVariable { Index = 0, Value = 2, Type = typeof(int) };
            var lv2 = new ILEngine.ILVariable { Index = 0, Value = 2, Type = typeof(int) };
            var eq = lv.Value == lv2.Value;
            var irTests = new ILEngine.Implementations.Tests.ILInstructionReaderTests();
            irTests.ConvertIConvertibleByteCodeTest();

            //var t= ArgIterator




            ILEngine.CodeGenerator.ILOpcodeInterperterSwitchActionGenerator.GenerateOpCodeJmpTable();




            // generate code to implement native MSIL methods using c# dynamics (DLR);
            var dynCs = ILEngine.CodeGenerator.ILEngineNativeInstructionGenerator.GenerateDynamicCs();



            //var r = ILEngine.ILEngineNativeInstructionGenerator.Generate();

            //var m_add = r.GetMethod("Add", new[] { typeof(object), typeof(object) });
            //var dn = m_add.Invoke(null, new object[] { 1, 2, });

            //var m_il = m_add.GetMethodBody().GetILAsByteArray();
            //var ins= ILEngine.IlInstructionReader.FromByteCode(m_il);



            var tests = new ILInstructionEngineTests();
            tests.ExecuteTest();

            tests.ExecuteTypedTest();

            tests.ExecuteTestInline();

            tests.ExecuteNativeTests();
            tests.ExecuteTypedNetIO();
        }
    }
}
