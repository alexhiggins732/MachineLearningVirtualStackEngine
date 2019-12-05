using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILEngine.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using ILEngine.Models;

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
                args.Add(argCount - 1);
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
                var result = IlStackFrameBuilder.BuildAndExecute(opCodes, args: args.ToArray(), locals: iLVariables.ToArray());
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

        public class MetaExpecation
        {
            public int BitSize;
            public int PopCount;
            public int NumRows;
            public int FiltersCount;
            public List<Func<OpCodeMetaModel, bool>> ExclusionFilters;
            public MetaExpecation()
            {
                this.ExclusionFilters = new List<Func<OpCodeMetaModel, bool>>();
            }
        }

        [TestMethod()]
        public void TestMeta()
        {
            var metaCodes = OpCodeMetaModel.OpCodeMetas;


            var expected = new[]
            {
                new MetaExpecation{BitSize=0, PopCount=0,NumRows=35},
                new MetaExpecation{BitSize=0, PopCount=1,NumRows=59},
                new MetaExpecation{BitSize=0, PopCount=2,NumRows=43},
                new MetaExpecation{BitSize=0, PopCount=3,NumRows=10},

                new MetaExpecation{BitSize=8, PopCount=0,NumRows=8},
                new MetaExpecation{BitSize=8, PopCount=1,NumRows=4},
                new MetaExpecation{BitSize=8, PopCount=2,NumRows=10},
                new MetaExpecation{BitSize=8, PopCount=3,NumRows=0},

                new MetaExpecation{BitSize=16, PopCount=0,NumRows=4},
                new MetaExpecation{BitSize=16, PopCount=1,NumRows=2},
                new MetaExpecation{BitSize=16, PopCount=2,NumRows=0},
                new MetaExpecation{BitSize=16, PopCount=3,NumRows=0},

                new MetaExpecation{BitSize=32, PopCount=0,NumRows=12},
                new MetaExpecation{BitSize=32, PopCount=1,NumRows=23},
                new MetaExpecation{BitSize=32, PopCount=2,NumRows=15},
                new MetaExpecation{BitSize=32, PopCount=3,NumRows=1},

                new MetaExpecation{BitSize=64, PopCount=0,NumRows=2},
                new MetaExpecation{BitSize=64, PopCount=1,NumRows=0},
                new MetaExpecation{BitSize=64, PopCount=2,NumRows=0},
                new MetaExpecation{BitSize=64, PopCount=3,NumRows=0},
            };



            var expectedSum = expected.Sum(x => x.NumRows);

            Assert.IsTrue(expectedSum == metaCodes.Count, $"Meta filter aggregate count mismatch:  Expected {metaCodes.Count}, Summed {expectedSum}");
            var filters = new Dictionary<MetaExpecation, Func<OpCodeMetaModel, bool>>();

            foreach (var expectation in expected)
            {
                int bs = expectation.BitSize;
                int pc = expectation.PopCount;
                filters.Add(expectation, x => x.OperandTypeBitSize == bs && x.StackBehaviourPopCount == pc);
            }

            foreach (var filter in filters)
            {
                var expectation = filter.Key;
                var filterFunc = filter.Value;
                var filteredMeta = metaCodes.Where(filterFunc).ToList();

                var count = filteredMeta.Count;
                Assert.IsTrue(expectation.NumRows == filteredMeta.Count, $"Filter count mismatch: BitSize: {expectation.BitSize}, Popcount: {expectation.PopCount}, NumRows: {expectation.NumRows} - Actual: {filteredMeta.Count}");

                SetFilters(expectation);

                var filtered = filteredMeta.Where(meta => !expectation.ExclusionFilters.Any(fn => fn(meta))).ToList();
                Assert.IsTrue(filtered.Count + expectation.FiltersCount == filteredMeta.Count, $"Failed to filter {expectation.FiltersCount} from {filteredMeta.Count}: Actual {filtered.Count}");

            }
            // Build Filters:
            /* BitSize=0, PopCount=0, -> value not in("arglist", "volatile.","tail.", "rethrow", "readonly.", "endfinally", "prefix7
            ,"prefix6"
            ,"prefix5"
,"prefix4"
,"prefix3"
,"prefix2" 
,"prefixref")*/


        }

        private void SetFilters(MetaExpecation expectation)
        {
            switch (expectation.BitSize)
            {
                case 0:
                    Set0BitFilters(expectation);
                    break;
                case 8:
                    Set8BitFilters(expectation);
                    break;
                case 16:
                    Set16BitFilters(expectation);
                    break;
                case 32:
                    Set32BitFilters(expectation);
                    break;
                case 64:
                    Set64BitFilters(expectation);
                    break;
            }
        }

        private void Set64BitFilters(MetaExpecation expectation)
        {
            switch (expectation.PopCount)
            {
                case 0:
                    //no exclusions
                    var filters_0_0 = new string[] { };
                    expectation.FiltersCount = filters_0_0.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_0.Contains(x.OpCode));
                    break;
                case 1:
                    //no opcodes, no exclusions
                    var filters_0_1 = new string[] { };
                    expectation.FiltersCount = filters_0_1.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_1.Contains(x.OpCode));
                    break;

                case 2:
                    //no opcodes, no exclusions
                    var filters_0_2 = new string[] { };
                    expectation.FiltersCount = filters_0_2.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_2.Contains(x.OpCode));
                    break;
                case 3:
                    //no opcodes, no exclusions
                    var filters_0_3 = new string[] { };
                    expectation.FiltersCount = filters_0_3.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_3.Contains(x.OpCode));
                    break;
            }
        }

        private void Set32BitFilters(MetaExpecation expectation)
        {
            switch (expectation.PopCount)
            {
                case 0:
                    // 2 exclusions
                    var filters_0_0 = new[] { "constrained.", "sizeof" };
                    expectation.FiltersCount = filters_0_0.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_0.Contains(x.OpCode));
                    break;
                case 1:
                    //TODO: review ref opcodes
                    var filters_0_1 = new[] { "exec.msil.i", "exec.msil.s", "refanyval", "mkrefany" };
                    expectation.FiltersCount = filters_0_1.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_1.Contains(x.OpCode));
                    break;

                case 2:
                    //TODO: review if cpobj, stobj for possible exclusions
                    var filters_0_2 = new string[] { };
                    expectation.FiltersCount = filters_0_2.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_2.Contains(x.OpCode));
                    break;
                case 3:
                    //no exclusion
                    var filters_0_3 = new string[] { };
                    expectation.FiltersCount = filters_0_3.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_3.Contains(x.OpCode));
                    break;
            }
        }


        private void Set16BitFilters(MetaExpecation expectation)
        {
            switch (expectation.PopCount)
            {
                case 0:
                    //no exclusions
                    var filters_0_0 = new string[] { };
                    expectation.FiltersCount = filters_0_0.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_0.Contains(x.OpCode));
                    break;
                case 1:
                    //no opcode, no exclusions
                    var filters_0_1 = new string[] { }; ;
                    expectation.FiltersCount = filters_0_1.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_1.Contains(x.OpCode));
                    break;

                case 2:
                    ///no opcodes, no exclusions
                    var filters_0_2 = new string[] { };
                    expectation.FiltersCount = filters_0_2.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_2.Contains(x.OpCode));
                    break;
                case 3:
                    //no opcodes, no exclusions
                    var filters_0_3 = new string[] { };
                    expectation.FiltersCount = filters_0_3.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_3.Contains(x.OpCode));
                    break;
            }
        }

        private void Set8BitFilters(MetaExpecation expectation)
        {
            switch (expectation.PopCount)
            {
                case 0:
                    // one exclusions
                    var filters_0_0 = new[] { "unaligned." };
                    expectation.FiltersCount = filters_0_0.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_0.Contains(x.OpCode));
                    break;
                case 1:
                    //no exclusions
                    var filters_0_1 = new string[] { };
                    expectation.FiltersCount = filters_0_1.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_1.Contains(x.OpCode));
                    break;

                case 2:
                    //no exclusions
                    var filters_0_2 = new string[] { };
                    expectation.FiltersCount = filters_0_2.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_2.Contains(x.OpCode));
                    break;
                case 3:
                    //no opcodes, no exclusions
                    var filters_0_3 = new string[] { };
                    expectation.FiltersCount = filters_0_3.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_3.Contains(x.OpCode));
                    break;
            }
        }

        private void Set0BitFilters(MetaExpecation expectation)
        {
            switch (expectation.PopCount)
            {
                case 0:
                    var filters_0_0 = new[] {"arglist", "volatile.","tail.", "rethrow", "readonly.", "endfinally", "prefix7"
                                ,"prefix6"
                                ,"prefix5"
                                        ,"prefix4"
                                        ,"prefix3"
                                        ,"prefix2"
                                        ,"prefixref"};
                    expectation.FiltersCount = filters_0_0.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_0.Contains(x.OpCode));
                    break;
                case 1:
                    //TODO: review ind opcodes for exclusions
                    var filters_0_1 = new[] { "localloc", "endfilter", "refanytype", "throw" };
                    expectation.FiltersCount = filters_0_1.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_1.Contains(x.OpCode));
                    break;

                case 2:
                    //TODO: review ind opcodes for exclusions
                    var filters_0_2 = new string[] { };
                    expectation.FiltersCount = filters_0_2.Length;
                    break;
                case 3:
                    //TODO: review if blk opcodes can be implemented.
                    var filters_0_3 = new[] { "cpblk", "initblk" };
                    expectation.FiltersCount = filters_0_3.Length;
                    expectation.ExclusionFilters.Add(x => filters_0_3.Contains(x.OpCode));
                    break;
            }
        }
    }
}