using ILEngine.Models;
using System.Text;

namespace ILEngine.CodeGenerator
{
    public class ILOpCodeValuesGenerator
    {
    
        public static string GenerateIlOpCodeValuesEnum()
        {

            var builder = new StringBuilder();

            builder.AppendLine(@"using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;");
            builder.AppendLine();
            builder.AppendLine(@"namespace ILEngine
{
");
            int indentation = 1;
            builder.AppendLineIndented("public enum ILOpCodeValues", indentation);
            builder.AppendLineIndented("{", indentation);


            var models = OpCodeMetaModel.OpCodeMetas;
            indentation++;
            int modelCount = models.Count;
            for (var i = 0; i < modelCount; i++)
            {
                var model = models[i];
                /* -- template
                         /// <summary>
        /// Fills space if opcodes are patched
        /// </summary>
        Nop = 0x00,
               
                  
                 
                 */
                builder.AppendLineIndented(@"/// <summary>", indentation);
                builder.AppendLineIndented($"/// {model.OpCodeDescription}", indentation);
                builder.AppendLineIndented(@"/// </summary>", indentation);
                builder.AppendLineIndented($@"{model.ClrName} = 0x{model.OpCodeValue.ToString("x2")}{(i == modelCount - 1 ? "" : ",")}", indentation);
                builder.AppendLine();
            }


            indentation--;

            builder.AppendLineIndented("} //enum ILOpCodeValues", indentation);

            builder.AppendLine("} //end namespace");

            var code = builder.ToString();
            return code;

        }
    }
}
