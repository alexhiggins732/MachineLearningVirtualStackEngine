using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.CodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            FindSignatureMethods();
        }

        private static void FindSignatureMethods()
        {
            ILSearcher.FindILByOperandType(System.Reflection.Emit.OperandType.InlineSig);
        }
    }
}
