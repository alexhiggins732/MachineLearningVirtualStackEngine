using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.Tests
{
    [TestClass()]
    public class ILVariableTests
    {
        [TestMethod()]
        public void ToStringTest()
        {
            var var = new ILVariable { Name = "test", Index = 1, Type = typeof(string), Value = nameof(ToStringTest) };
            var actual = var.ToString();
            var expected = "Loc.test (String) {ToStringTest}";
            Assert.IsTrue(actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");

            var var2 = new ILVariable();
            actual = var2.ToString();
            expected = "Loc.0 () {null}";
            Assert.IsTrue(actual == expected, $"Actual:{actual}\r\nExpected:{expected}\r\n");
        }
        [TestMethod()]
        public void CopyFromMethodInfoLocalVariableInfo()
        {

            var thisType = this.GetType();
            var thisMethod = thisType.GetMethod(nameof(CopyFromMethodInfoLocalVariableInfo));
            Assert.IsNotNull(thisMethod, "Failed to resolve current method");
            var thisMethodBody = thisMethod.GetMethodBody();
            var thisLocals = thisMethodBody.LocalVariables;
            var src = thisLocals.First(x => x.LocalIndex == 0);
            var ilVariable = new ILVariable { Name = nameof(thisType) };
            ilVariable.CopyFrom(src);

            var actualIndex = ilVariable.Index;
            var expectedIndex = src.LocalIndex;
            Assert.IsTrue(actualIndex == expectedIndex, $"Local Index Mistmatch\r\nActual:{actualIndex}\r\nExpected:{expectedIndex}\r\n");

            var actualType = ilVariable.Type;
            Assert.IsNotNull(actualType, "Failed to copy local type");
            var expectedType = src.LocalType;
            Assert.IsTrue(actualType == expectedType, $"Local Index Mistmatch\r\nActual:{actualType.Name}\r\nExpected:{expectedType.Name}\r\n");


        }

        [TestMethod()]
        public void CopyFromMethodInfoLocalVariableInfoWithInitLocals()
        {
            ILVariable fieldTest;

            var thisType = this.GetType();
            var thisMethod = thisType.GetMethod(nameof(CopyFromMethodInfoLocalVariableInfoWithInitLocals));
            Assert.IsNotNull(thisMethod, "Failed to resolve current method");
            var thisMethodBody = thisMethod.GetMethodBody();
            var thisLocals = thisMethodBody.LocalVariables;
            var src = thisLocals.First(x => x.LocalIndex == 0);
            var ilVariable = new ILVariable { Name = nameof(fieldTest) };
            ilVariable.CopyFrom(src, true);

            var actualIndex = ilVariable.Index;
            var expectedIndex = src.LocalIndex;
            Assert.IsTrue(actualIndex == expectedIndex, $"Local Index Mistmatch\r\nActual:{actualIndex}\r\nExpected:{expectedIndex}\r\n");

            var actualType = ilVariable.Type;
            Assert.IsNotNull(actualType, "Failed to copy local type");
            var expectedType = src.LocalType;
            Assert.IsTrue(actualType == expectedType, $"Local Index Mistmatch\r\nActual:{actualType.Name}\r\nExpected:{expectedType.Name}\r\n");
            var actual = ilVariable.Value;
            Assert.IsNotNull(actual, "Copy failed to initialize variable");

        }
    }
}