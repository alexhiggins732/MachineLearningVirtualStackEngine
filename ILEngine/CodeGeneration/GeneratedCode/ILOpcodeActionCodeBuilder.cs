using System;
using System.Collections.Generic;

namespace ILEngine
{
    public interface IILOpcodeActionCodeMemberNameProvider
    {
        string InterfaceName { get; }
        string BaseClassName { get; }
    }
    public class ILOpcodeActionCodeMemberNameProvider : IILOpcodeActionCodeMemberNameProvider
    {
        public string InterfaceName => "IILOpcodeActionGenerator";

        public string BaseClassName => "ILOpcodeActionGeneratorBase";
    }

    public class ILOpcodeActionCodeBuilder
    {

        public static string GetILOpcodeInterfaceCode(IILOpcodeActionCodeMemberNameProvider memberNameProvider = null)
        {
            memberNameProvider = memberNameProvider ?? new ILOpcodeActionCodeMemberNameProvider();
            var sb = new System.Text.StringBuilder();
            var abstractBaseClassBuilder = new System.Text.StringBuilder();
            var abstractMethodBuilder = new System.Text.StringBuilder();


            var names = Enum.GetNames(typeof(ILOpCodeValues));
            var values = Enum.GetValues(typeof(ILOpCodeValues));
            var lookup = new Dictionary<int, string>();
            sb.AppendLine($"\tpublic interface {memberNameProvider.InterfaceName}");
            sb.AppendLine("\t{");

            abstractBaseClassBuilder.AppendLine($"\tpublic abstract class {memberNameProvider.BaseClassName}: { memberNameProvider.InterfaceName}");
            abstractBaseClassBuilder.AppendLine("\t{");

            sb.AppendLine("\t\tvoid GenerateAll();");
            abstractBaseClassBuilder.AppendLine("\t\tpublic void GenerateAll()");
            abstractBaseClassBuilder.AppendLine("\t\t{");

            for (var i = 0; i < values.Length; i++)
            {
                var value = (ILOpCodeValues)values.GetValue(i);
                var intValue = (int)value;
                var name = names[i];
                //var argSize = GetOpCodeArgByteSize(value);
                sb.AppendLine("\t\tvoid Generate" + name + "();");

                abstractMethodBuilder.AppendLine("[OpcodeValue(" + intValue + ")]");
                abstractMethodBuilder.AppendLine("\t\tpublic abstract void Generate" + name + "();");

                abstractBaseClassBuilder.AppendLine("\t\t\tGenerate" + name + "();");
            }
            abstractBaseClassBuilder.AppendLine("\t\t}");
            abstractBaseClassBuilder.AppendLine(abstractMethodBuilder.ToString());
            abstractBaseClassBuilder.AppendLine("\t}");
            sb.AppendLine("\t}");
            var result = sb.ToString();

            var resultab = abstractBaseClassBuilder.ToString();

            return result;
        }

    }


}
