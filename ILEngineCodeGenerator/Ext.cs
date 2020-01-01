using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.CodeGenerator
{
    static class Ext
    {
        public static void AppendIndented(this StringBuilder builder, string message, int depth = 0)
        {
            builder.Append($"{GetIndent(depth)}{ message}");
        }
        public static void AppendLineIndented(this StringBuilder builder, string message, int depth = 0)
        {
            builder.AppendLine($"{GetIndent(depth)}{ message}");
        }

        const string tab = "    ";
        static string GetIndent(int depth)
        {
            var result = "";
            for (var i = 0; i < depth; i++)
            {
                result += tab;

            }
            return result;
        }
    }
}
