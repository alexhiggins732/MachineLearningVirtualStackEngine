using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILVmModel.Models
{
    public class VmMethod
    {
        public string MethodName { get; set; }
        public VariableCollection Arguments { get; set; }
        public VariableCollection Locals { get; set; }
        public List<VmInstruction> Instructions { get; set; }
        public VmMethod(string name)
        {

        }
    }
}
