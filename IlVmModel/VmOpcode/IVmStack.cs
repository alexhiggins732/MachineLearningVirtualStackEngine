using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IlVmModel.VmOpcode
{
    public interface IVmStack
    {
        MethodInfo Pop { get; set; }
        MethodInfo Push { get; set; }
    }

    //Need to start definining these manually, but is multiple dayss of work so will get distracted and not knowing where to continue
    //      will most likely give up and leaving it inpcomplete like numerous code generators, switch statements, etc
    //      that have already been coded so far.
    // Lets take a different approach:
    //      Scope out actual implementenation:
    //          Road map with incremential units of work.
    //          Incorporate TDD and mark off todos and milestones.


    public class VMOpcodeAdd
    {
        IVmStack stack;
        public VMOpcodeAdd(IVmStack vmStack) => this.stack = vmStack;
        public void GenerateIl(ILGenerator il)
        {
            il.Emit(OpCodes.Call, stack.Pop);
            il.Emit(OpCodes.Call, stack.Pop);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Call, stack.Push);
        }
    }



}
