using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine.Compilers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.Compilers.Tests
{
    [TestClass()]
    public class ILMethodBuilderTests
    {
        [TestMethod]
        public void Compile_LdNull_Test()
        {
            var method = ILMethodBuilder.Compile_LdNull();
            var actual = method.Invoke(null, Type.EmptyTypes);
            Assert.IsNull(actual);

        }

        [TestMethod]
        public void Compile_Ldloc_0_Test()
        {
            var method = ILMethodBuilder.Compile_Ldloc_0();
            var actual = method.Invoke(null, Type.EmptyTypes);
            var expected = 1;

            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\n:Expected: {expected}");

        }

        [TestMethod()]
        public void CompileBinaryTest()
        {
            var madd = ILMethodBuilder.Compile_Add(typeof(uint), typeof(uint), typeof(uint));
            Assert.IsNotNull(madd);
            var actual = madd.Invoke(null, new object[] { uint.MaxValue, 2u });
            var expected = 1u;
            Assert.IsTrue((uint)actual == expected, $"Actual: {actual}\r\n:Expected: {expected}");


            var test_Add = ILMethodBuilder.Compile_Add(typeof(uint), typeof(uint), typeof(uint));
            var result_Add = test_Add.Invoke(null, new object[] { uint.MaxValue, 2u });
            Assert.IsTrue((uint)result_Add == expected, $"Actual: {actual}\r\n:Expected: {expected}");

            var test_clt = ILMethodBuilder.CompileBinary(ILEngine.ILOpCodeValues.Clt, typeof(bool), typeof(uint), typeof(uint));
            var result_test_clt = test_clt.Invoke(null, new object[] { uint.MaxValue, 2u });
            Assert.IsTrue((bool)result_test_clt);
            var result_test_clt2 = test_clt.Invoke(null, new object[] { 2u, uint.MaxValue });
            Assert.IsFalse((bool)result_test_clt2);


            var test_cgt = ILMethodBuilder.CompileBinary(ILEngine.ILOpCodeValues.Cgt, typeof(bool), typeof(uint), typeof(uint));
            var result_test_cgt = test_cgt.Invoke(null, new object[] { uint.MaxValue, 2u });
            Assert.IsFalse((bool)result_test_cgt);
            var result_test_cgt2 = test_cgt.Invoke(null, new object[] { 2u, uint.MaxValue });
            Assert.IsTrue((bool)result_test_cgt2);

            var test_cgt_un = ILMethodBuilder.CompileBinary(ILEngine.ILOpCodeValues.Cgt_Un, typeof(bool), typeof(uint), typeof(uint));
            var result_test_cgt_un = test_cgt_un.Invoke(null, new object[] { uint.MaxValue, 2u });
            Assert.IsTrue((bool)result_test_cgt_un);
            var result_test_cgt2_un = test_cgt_un.Invoke(null, new object[] { 2u, uint.MaxValue });
            Assert.IsFalse((bool)result_test_cgt2_un);

            var test_Add_Ovf = ILMethodBuilder.Compile_Add_Ovf(typeof(int), typeof(int), typeof(int));
            Assert.IsNotNull(test_Add_Ovf);
            Exception ex = null;
            try
            {
                var result_Add_Ovf = test_Add_Ovf.Invoke(null, new object[] { int.MaxValue, 1 });
            }
            catch (Exception c)
            {
                ex = c;
            }
            Assert.IsNotNull(ex);
            Assert.IsInstanceOfType(ex, typeof(System.Reflection.TargetInvocationException));
            Assert.IsNotNull(ex.InnerException);
            Assert.IsInstanceOfType(ex.InnerException, typeof(OverflowException));

            var test_Add_Ovf_Un = ILMethodBuilder.Compile_Add_Ovf_Un(typeof(uint), typeof(uint), typeof(uint));
            Assert.IsNotNull(test_Add_Ovf_Un);
            ex = null;

            try
            {
                var result_Add_Ovf_Un = test_Add_Ovf_Un.Invoke(null, new object[] { uint.MaxValue, 2u });
            }
            catch (Exception c)
            {
                ex = c;
            }
            Assert.IsNotNull(ex);
            Assert.IsInstanceOfType(ex, typeof(System.Reflection.TargetInvocationException));
            Assert.IsNotNull(ex.InnerException);
            Assert.IsInstanceOfType(ex.InnerException, typeof(OverflowException));

        }

        [TestMethod()]
        public void Compile_AddIntLocalsTest()
        {
            var defaultAdd = ILMethodBuilder.Compile_AddIntLocals();

            var actual = defaultAdd.Invoke(null, Type.EmptyTypes);
            var expected = 2;
            Assert.IsTrue((int)actual == expected, $"Actual: {actual}\r\n:Expected: {expected}");
        }

        [TestMethod()]
        public void Compile_AddTypedTest()
        {
            var madd = ILMethodBuilder.Compile_Add(typeof(long), new[] { typeof(long), typeof(long) });
            Assert.IsNotNull(madd);
            var actual = madd.Invoke(null, new object[] { 1L, 1L });
            var expected = 2L;
            Assert.IsTrue((long)actual == expected, $"Actual: {actual}\r\n:Expected: {expected}");
        }

        [ExpectedException(typeof(OverflowException))]
        [TestMethod()]
        public void Compile_Add_OvfTest()
        {
            var madd = ILMethodBuilder.Compile_Add_Ovf();
            Assert.IsNotNull(madd);
            try
            {
                var actual = madd.Invoke(null, Type.EmptyTypes);
            }
            catch (System.Reflection.TargetInvocationException refEx)
            {
                throw refEx.InnerException;
            }
        }

        [ExpectedException(typeof(OverflowException))]
        [TestMethod()]
        public void Compile_Add_OvfTest1()
        {
            var madd = ILMethodBuilder.Compile_Add_Ovf(typeof(long), new[] { typeof(long), typeof(long) });
            Assert.IsNotNull(madd);
            try
            {
                var actual = madd.Invoke(null, new object[] { 1L, 1L });
                var expected = 2L;
                Assert.IsTrue((long)actual == expected, $"Actual: {actual}\r\n:Expected: {expected}");

                actual = madd.Invoke(null, new object[] { long.MaxValue, long.MaxValue });
            }
            catch (System.Reflection.TargetInvocationException refEx)
            {
                throw refEx.InnerException;
            }
        }

        [TestMethod()]
        public void Compile_Add_Ovf_UnTest()
        {
            var madd = ILMethodBuilder.Compile_Add_Ovf_Un();
            Assert.IsNotNull(madd);
            var actual = madd.Invoke(null, Type.EmptyTypes);
            var expected = 2u;
            Assert.IsTrue((uint)actual == expected, $"Actual: {actual}\r\n:Expected: {expected}");
        }

        [ExpectedException(typeof(OverflowException))]
        [TestMethod()]
        public void Compile_Add_Ovf_UnTest1()
        {
            var madd = ILMethodBuilder.Compile_Add_Ovf_Un(typeof(uint), new[] { typeof(uint), typeof(uint) });
            Assert.IsNotNull(madd);
            var actual = madd.Invoke(null, new object[] { 1u, 1u });
            var expected = 2u;
            Assert.IsTrue((uint)actual == expected, $"Actual: {actual}\r\n:Expected: {expected}");
            try
            {
                actual = madd.Invoke(null, new object[] { uint.MaxValue, 1u });
            }
            catch (System.Reflection.TargetInvocationException refEx)
            {
                throw refEx.InnerException;
            }
        }
    }
}