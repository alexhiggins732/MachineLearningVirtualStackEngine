using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ILVmModel.VmOpCode
{
    public interface IVmStack
    {
        MethodInfo Pop { get; set; }
        MethodInfo Push { get; set; }
    }



}
