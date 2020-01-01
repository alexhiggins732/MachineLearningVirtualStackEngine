using ILEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.CodeGenerator
{
    /// <summary>
    /// Auto-generates comprehensive list of unit tests to cover all opcode instructions
    /// </summary>
    public class TestsAutoGenerator
    {
        public static void Generate()
        {
            var opCodes = OpCodeMetaModel.OpCodeMetas;

            var compOpCodes = opCodes.Where(x => x.OpCodeDescription.IndexOf("compare", StringComparison.OrdinalIgnoreCase) > -1);

            //Ceq, Cgt, Cgt_Un, Clt, Clt_Un
            var sb = new System.Text.StringBuilder();

            //var allRangeValues = TypeRanges
            //    .All
            //    .SelectMany(x =>
            //       x.Select(
            //    ).ToList();
            //foreach (var comparisonOpCode in compOpCodes)
            //{
            //   foreach(TypeRange in TypeRanges.All)
            //    {

            //    }
            //}

        }
    }

}
