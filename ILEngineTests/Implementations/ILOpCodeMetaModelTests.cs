using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using ILEngine.Models;

namespace ILEngine.Implementations.Tests
{
    [TestClass]
    public class ILOpCodeMetaModelTests
    {
        [TestMethod()]
        public void TestMeta()
        {
            var metaCodes = OpCodeMetaModel.OpCodeMetas;


            var expected = new[]
            {
                new MetaExpectation{BitSize=0, PopCount=0,NumRows=35},
                new MetaExpectation{BitSize=0, PopCount=1,NumRows=59},
                new MetaExpectation{BitSize=0, PopCount=2,NumRows=43},
                new MetaExpectation{BitSize=0, PopCount=3,NumRows=10},

                new MetaExpectation{BitSize=8, PopCount=0,NumRows=8},
                new MetaExpectation{BitSize=8, PopCount=1,NumRows=4},
                new MetaExpectation{BitSize=8, PopCount=2,NumRows=10},
                new MetaExpectation{BitSize=8, PopCount=3,NumRows=0},

                new MetaExpectation{BitSize=16, PopCount=0,NumRows=4},
                new MetaExpectation{BitSize=16, PopCount=1,NumRows=2},
                new MetaExpectation{BitSize=16, PopCount=2,NumRows=0},
                new MetaExpectation{BitSize=16, PopCount=3,NumRows=0},

                new MetaExpectation{BitSize=32, PopCount=0,NumRows=12},
                new MetaExpectation{BitSize=32, PopCount=1,NumRows=23},
                new MetaExpectation{BitSize=32, PopCount=2,NumRows=15},
                new MetaExpectation{BitSize=32, PopCount=3,NumRows=1},

                new MetaExpectation{BitSize=64, PopCount=0,NumRows=2},
                new MetaExpectation{BitSize=64, PopCount=1,NumRows=0},
                new MetaExpectation{BitSize=64, PopCount=2,NumRows=0},
                new MetaExpectation{BitSize=64, PopCount=3,NumRows=0},
            };



            var expectedSum = expected.Sum(x => x.NumRows);

            Assert.IsTrue(expectedSum == metaCodes.Count, $"Meta filter aggregate count mismatch:  Expected {metaCodes.Count}, Summed {expectedSum}");
            var filters = new Dictionary<MetaExpectation, Func<OpCodeMetaModel, bool>>();

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

        private void SetFilters(MetaExpectation expectation)
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

        private void Set64BitFilters(MetaExpectation expectation)
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

        private void Set32BitFilters(MetaExpectation expectation)
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


        private void Set16BitFilters(MetaExpectation expectation)
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

        private void Set8BitFilters(MetaExpectation expectation)
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

        private void Set0BitFilters(MetaExpectation expectation)
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

        public class MetaExpectation
        {
            public int BitSize;
            public int PopCount;
            public int NumRows;
            public int FiltersCount;
            public List<Func<OpCodeMetaModel, bool>> ExclusionFilters;
            public MetaExpectation()
            {
                this.ExclusionFilters = new List<Func<OpCodeMetaModel, bool>>();
            }
        }
    }
}