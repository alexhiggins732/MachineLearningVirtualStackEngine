using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.CodeGenerator
{
    public class ILSearchResult
    {
        public MethodInfo Method;
        public List<ILInstruction> Instructions;

        public ILSearchResult(MethodInfo method)
        {
            Method = method;
            this.Instructions = ILInstructionReader.FromMethod(method);
        }
    }
    public class ILSearcher
    {
        public static List<ILSearchResult> FindIL(Func<IEnumerable<ILInstruction>, bool> filter,
            Assembly assembly = null,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance
            )
        {
            assembly = assembly ?? typeof(int).Assembly;
            var result = assembly
                .GetExportedTypes()
                .SelectMany(type => type
                    .GetMethods(bindingFlags)
                    .Select(Method => new ILSearchResult(Method))
                    .Where(x=> filter(x.Instructions))
                    .ToList()
                ).ToList();
            return result;
        }
        public static void FindILByOperandType(OperandType operandType)
        {
            Func<IEnumerable<ILInstruction>, bool> filter = (x)
                  => x.Any(il => il.OpCode.OperandType == operandType);

            var instructions = FindIL(filter);

        }
    }
}
