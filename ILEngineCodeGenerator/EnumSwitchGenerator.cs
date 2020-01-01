using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.CodeGenerator
{
    public class EnumSwitchGenerator
    {
        public static string GenerateCsEnumSwitch<T>() where T : struct, Enum
        {
            var enumType = typeof(T);
            var names = Enum.GetNames(enumType);


            var values = Enum.GetValues(enumType);
            Func<T, int> getIntValue = (val) => (int)(dynamic)val;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"\tswitch({Char.ToLower(enumType.Name[0])}{enumType.Name.Substring(1)})");
            sb.AppendLine("\t{");
            for (var i = 0; i < values.Length; i++)
            {
                var value = (T)values.GetValue(i);
                var intValue = getIntValue(value);
                var name = names[i];
                sb.AppendLine($"\t\tcase {enumType.FullName}{name}: //{intValue}");
                sb.AppendLine($"\t\t\t//{intValue}");
                sb.AppendLine($"\t\t\tbreak;");
            }
            sb.AppendLine("\t\tcase default: throw new NotImplementedException()");
            sb.AppendLine("\t}");
            return sb.ToString();


        }
    }
}
