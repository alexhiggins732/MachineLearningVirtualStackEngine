using ILEngine.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngineTests
{
    public class Program
    {


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
        public static void Main(string[] args)
        {

            var test_Add = IlMethodBuilder.Compile_Add(typeof(uint), typeof(uint), typeof(uint));
            var result_Add = test_Add.Invoke(null, new object[] { uint.MaxValue, 2u });

            var test_clt = IlMethodBuilder.CompileBinary(ILEngine.ILOpCodeValues.Clt, typeof(bool), typeof(uint), typeof(uint));
            var result_test_clt = test_clt.Invoke(null, new object[] { uint.MaxValue, 2u });
            var result_test_clt2 = test_clt.Invoke(null, new object[] { 2u, uint.MaxValue });

            var test_cgt = IlMethodBuilder.CompileBinary(ILEngine.ILOpCodeValues.Cgt, typeof(bool), typeof(uint), typeof(uint));
            var result_test_cgt = test_cgt.Invoke(null, new object[] { uint.MaxValue, 2u });
            var result_test_cgt2 = test_cgt.Invoke(null, new object[] { 2u, uint.MaxValue });


            var test_cgt_un = IlMethodBuilder.CompileBinary(ILEngine.ILOpCodeValues.Cgt_Un, typeof(bool), typeof(uint), typeof(uint));
            var result_test_cgt_un = test_cgt_un.Invoke(null, new object[] { uint.MaxValue, 2u });
            var result_test_cgt2_un = test_cgt_un.Invoke(null, new object[] { 2u, uint.MaxValue });

            var test_Add_Ovf = IlMethodBuilder.Compile_Add_Ovf(typeof(uint), typeof(uint), typeof(uint));
            var result_Add_Ovf = test_Add_Ovf.Invoke(null, new object[] { uint.MaxValue, 2u });

            var test_Add_Ovf_Un = IlMethodBuilder.Compile_Add_Ovf_Un(typeof(uint), typeof(uint), typeof(uint));
            var result_Add_Ovf_Un = test_Add_Ovf_Un.Invoke(null, new object[] { uint.MaxValue, 2u });

            var test = new ILEngineTests.IlinstructionEngineOpcodeTests();
            test.CompiledOpCodeTests();

            var lv = new ILEngine.ILVariable { Index = 0, Value = 2, Type = typeof(int) };
            var lv2 = new ILEngine.ILVariable { Index = 0, Value = 2, Type = typeof(int) };
            var eq = lv.Value == lv2.Value;
            var irTests = new ILEngine.Tests.IlInstructionReaderTests();
            irTests.ConvertIConvertibleByteCodeTest();

            //var t= ArgIterator




            ILEngine.ILOpcodeInterperterSwitchActionGenerator.GenerateOpCodeJmpTable();




            // generate code to implement native MSIL methods using c# dynamics (DLR);
            var dynCs = ILEngine.ILEngineNativeInstructionGenerator.GenerateDynamicCs();



            //var r = ILEngine.ILEngineNativeInstructionGenerator.Generate();

            //var m_add = r.GetMethod("Add", new[] { typeof(object), typeof(object) });
            //var dn = m_add.Invoke(null, new object[] { 1, 2, });

            //var m_il = m_add.GetMethodBody().GetILAsByteArray();
            //var ins= ILEngine.IlInstructionReader.FromByteCode(m_il);



            var tests = new IlInstructionEngineTests();
            tests.ExecuteTest();

            tests.ExecuteTypedTest();

            tests.ExecuteTestInline();

            tests.ExecuteNativeTests();
            tests.ExecuteTypedNetIO();
        }
    }
}
