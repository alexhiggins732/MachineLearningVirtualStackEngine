using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace ILEngine.Implementations.Tests
{
    [TestClass()]
    public class IlStackFrameWithDiagnosticsTests
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
                var result = IlStackFrameBuilder.BuildAndExecute(opCodes);
                Assert.IsTrue(result.ExecutedInstructions == 1);
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
                args.Add(argCount-1);
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
                var result = IlStackFrameBuilder.BuildAndExecute(opCodes, args: args.ToArray(), locals:iLVariables.ToArray());
                Assert.IsTrue(result.ExecutedInstructions == 1);
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
            flowControlExclusions.Add(x => x == FlowControl.Phi);
            flowControlExclusions.Add(x => x == FlowControl.Break);



            allOpCodes = allOpCodes.Where(x => !flowControlExclusions.Any((exclude) => exclude(x.FlowControl)));


            var rem = allOpCodes.ToList();
            Assert.IsTrue(rem.Count == 214);


        }

        [TestMethod()]
        public void ResetTest()
        {
            //Assert.Fail();
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
            //Assert.Fail();
        }
    }
}