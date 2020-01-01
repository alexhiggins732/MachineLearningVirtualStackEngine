using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine.CodeGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.CodeGeneratorTests
{
    [TestClass()]
    public class CsExpressionBuilderTests
    {
        [TestMethod()]
        public void AddPreBodyExpressionTest()
        {
            var builder = new CsExpressionBuilder();
            var scope = builder.CreateScope();

            scope.AddUsing("System");
            scope.AddPreBodyExpression("\r\n");
            scope.AddPreBodyExpression("namespace TestNamespace\r\n");
            scope.AddPreBodyExpression("{\r\n");

            var actual = builder.ToString();
            var expected = @"using System;

namespace TestNamespace
{
";
            Assert.IsTrue(actual == expected, $"Builder failed: Expected {expected}, Actual: {actual}");
        }

        [TestMethod()]
        public void AddPostBodyExpressionTest()
        {
            var builder = new CsExpressionBuilder();
            var scope = builder.CreateScope();

            scope.AddUsing("System");
            scope.AddPreBodyExpression("\r\n");
            scope.AddPreBodyExpression("namespace TestNamespace\r\n");
            scope.AddPreBodyExpression("{\r\n");

            scope.AddPostBodyExpression("}\r\n");


            var actual = builder.ToString();
            var expected = @"using System;

namespace TestNamespace
{
}
";
            Assert.IsTrue(actual == expected, $"Builder failed: Expected {expected}, Actual: {actual}");
        }

        [TestMethod()]
        public void AddChildExpressionTest()
        {
            var builder = new CsExpressionBuilder();
            var scope = builder.CreateScope();

            scope.AddUsing("System");
            scope.AddPreBodyExpression("\r\n");
            scope.AddPreBodyExpression("namespace TestNamespace\r\n");
            scope.AddPreBodyExpression(new TextExpression("{\r\n"));

            scope.AddPostBodyExpression("}\r\n");

            var classNode = scope.CreateChildExpression();
            var child = classNode.CreateScope();

            child.AddPreBodyExpression("public class TestClass\r\n");
            child.AddPreBodyExpression("{\r\n");
            child.AddPostBodyExpression("}\r\n");

            var methodExpression = child.CreateChildExpression();
            var methodScope = methodExpression.CreateScope();
            methodScope.AddPreBodyExpression("public void SomeMethod()\r\n");
            methodScope.AddPreBodyExpression(new TextExpression("{\r\n"));
            methodScope.AddPostBodyExpression(new TextExpression("}\r\n"));

            var methodBody = methodScope.CreateChildExpression();


            Action<CsExpressionBuilder> createSwitch = (x) =>
            {
                var switchScope = x.CreateScope();
                switchScope.AddPreBodyExpression("switch (somestatement)\r\n");
                switchScope.AddPreBodyExpression("{\r\n");

                var case1 = switchScope.CreateChildExpression().CreateScope();
                case1.AddPreBodyExpression("case value1:\r\n");
                var case1Body = case1.CreateChildExpression().CreateScope();
                case1Body.AddPreBodyExpression(new TextExpression("return case1;\r\n"));

                var case2 = switchScope.CreateChildExpression().CreateScope();
                case2.AddPreBodyExpression("case value2:\r\n");
                var case2Body = case2.CreateChildExpression().CreateScope();
                case2Body.AddPreBodyExpression(new TextExpression("return case2;\r\n"));

                switchScope.AddPostBodyExpression("}\r\n");
            };

            createSwitch(methodBody);
            methodBody.CreateScope().AddPreBodyExpression("\r\n");
            createSwitch(methodBody);
            methodBody.CreateScope().AddPreBodyExpression("\r\n");

            var methodIf = methodBody.CreateScope();
            methodIf.AddPreBodyExpression("if(1==1)\r\n");
            methodIf.AddPreBodyExpression("{\r\n");
            methodIf.AddPostBodyExpression("}\r\n");
            var ifBodyScope = methodIf.CreateChildExpression().CreateScope();
            ifBodyScope.AddPreBodyExpression("return 1;\r\n");
            var methodElse = methodBody.CreateScope();
            methodElse.AddPreBodyExpression("else\r\n");
            methodElse.AddPreBodyExpression("{\r\n");
            methodElse.AddPostBodyExpression("}\r\n");
            var elseBodyScope = methodElse.CreateChildExpression().CreateScope();
            elseBodyScope.AddPreBodyExpression("return 2;\r\n");
            //child.AddChildExpression(methodExpression);





            var actual = builder.ToString();
            var expected = @"using System;

namespace TestNamespace
{
    public class TestClass
    {
        public void SomeMethod()
        {
            switch (somestatement)
            {
                case value1:
                    return case1;
                case value2:
                    return case2;
            }
            
            switch (somestatement)
            {
                case value1:
                    return case1;
                case value2:
                    return case2;
            }
            
            if(1==1)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }
}
";
            Assert.IsTrue(actual == expected, $"Builder failed: Expected {expected}, Actual: {actual}");
        }

        [TestMethod()]
        public void CreateChildExpressionTest()
        {
            var builder = new CsExpressionBuilder();
            var scope = builder.CreateScope();

            scope.AddUsing("System");
            scope.AddPreBodyExpression("\r\n");
            scope.AddPreBodyExpression("namespace TestNamespace\r\n");
            scope.AddPreBodyExpression(new TextExpression("{\r\n"));

            scope.AddPostBodyExpression("}\r\n");

            var classNode = scope.CreateChildExpression();
            var child = classNode.CreateScope();

            child.AddPreBodyExpression("public class TestClass\r\n");
            child.AddPreBodyExpression("{\r\n");
            child.AddPreBodyExpression("}\r\n");

            var class2 = classNode.CreateScope();
            class2.AddPreBodyExpression("public class TestClass2\r\n");
            class2.AddPreBodyExpression("{\r\n");
            class2.AddPreBodyExpression("}\r\n");



            var actual = builder.ToString();
            var expected = @"using System;

namespace TestNamespace
{
    public class TestClass
    {
    }
    public class TestClass2
    {
    }
}
";
            Assert.IsTrue(actual == expected, $"Builder failed:\r\nExpected:\r\n{expected}, Actual:\r\n{actual}");
        }

        [TestMethod()]
        public void AddPreBodyExpressionTest1()
        {
            var builder = new CsExpressionBuilder();
            var scope = builder.CreateScope();

            scope.AddUsing("System");
            scope.AddPreBodyExpression("\r\n");
            scope.AddPreBodyExpression("namespace TestNamespace\r\n");
            scope.AddPreBodyExpression(new TextExpression("{\r\n"));

            scope.AddPostBodyExpression("}\r\n");


            var actual = builder.ToString();
            var expected = @"using System;

namespace TestNamespace
{
}
";
            Assert.IsTrue(actual == expected, $"Builder failed: Expected {expected}, Actual: {actual}");
        }

        [TestMethod()]
        public void AddNamespaceDeclarationTest()
        {
            var builder = new CsExpressionBuilder();
            var scope = builder.CreateScope();

            scope.AddUsing("System");
            scope.AddNamespaceDeclaration("TestNameSpace");
            scope.AddPreBodyExpression("{\r\n");
            scope.AddPostBodyExpression("}\r\n");
            var actual = builder.ToString();

            var expected = @"using System;
namespace TestNameSpace
{
}
";
            Assert.IsTrue(actual == expected, $"Builder failed: Expected {expected}, Actual: {actual}");

        }

        [TestMethod()]
        public void AddPostBodyExpressionTest1()
        {
            var builder = new CsExpressionBuilder();
            var scope = builder.CreateScope();

            scope.AddUsing("System");
            scope.AddPreBodyExpression("\r\n");
            scope.AddPreBodyExpression("namespace TestNamespace\r\n");
            scope.AddPreBodyExpression(new TextExpression("{\r\n"));
            scope.AddPostBodyExpression("}\r\n");

            var scope2 = builder.CreateScope();
            scope2.AddPreBodyExpression("\r\n");
            scope2.AddPreBodyExpression("namespace TestNamespace2\r\n");
            scope2.AddPreBodyExpression("{\r\n");

            scope2.AddPostBodyExpression(new TextExpression("}\r\n"));
            var actual = builder.ToString();
            var expected = @"using System;

namespace TestNamespace
{
}

namespace TestNamespace2
{
}
";
            Assert.IsTrue(actual == expected, $"Builder failed: Expected {expected}, Actual: {actual}");

        }

        [TestMethod()]
        public void AddUsingTest()
        {
            var builder = new CsExpressionBuilder();
            var scope = builder.CreateScope();
            scope.AddUsing("System");
            var actual = builder.ToString();
            var expected = @"using System;
";
            Assert.IsTrue(actual == expected, $"Builder failed: Expected {expected}, Actual: {actual}");

        }

        [TestMethod()]
        public void AddUsingsTest()
        {
            var builder = new CsExpressionBuilder();
            var scope = builder.CreateScope();
            scope.AddUsings("System", "System.Collections.Generic");
            var expected = @"using System;
using System.Collections.Generic;
";
            var actual = builder.ToString();
            Assert.IsTrue(actual == expected, $"Builder failed: Expected {expected}, Actual: {actual}");
        }

        [TestMethod()]
        public void ToStringTest()
        {
            var builder = new CsExpressionBuilder();
            var actual = builder.ToString();
            var expected = "";
            Assert.IsTrue(actual == expected, $"Builder failed: Expected {expected}, Actual: {actual}");
        }
    }
}