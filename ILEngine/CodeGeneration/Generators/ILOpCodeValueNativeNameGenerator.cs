using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.CodeGeneration.Generators
{

    public class ILOpCodeValueNativeNameGenerator
    {
        public static string GetNativeNames()
        {

            var opCodes = OpCodeLookup.OpCodesByName;
            var map = opCodes.Select(x =>
            {
                var name = x.Value.ToString();
                var shortValue = x.Value.Value;
                var IlOpCodeValue = (ILOpCodeValues)shortValue;
                var ILOpCodeName = Enum.GetName(typeof(ILOpCodeValues), shortValue);
                if (string.IsNullOrEmpty(ILOpCodeName))
                {
                    ILOpCodeName = $"{x.Key} /*missing*/";
                }
                return $"public const string {ILOpCodeName} = \"{name}\";";
            });
            var lines = string.Join("\r\n", map);
            return lines;
        }

    }
}
