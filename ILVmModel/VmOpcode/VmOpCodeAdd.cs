using System.Reflection.Emit;

namespace ILVmModel.VmOpCode
{
    //Need to start definining these manually, but is multiple dayss of work so will get distracted and not knowing where to continue
    //      will most likely give up and leaving it inpcomplete like numerous code generators, switch statements, etc
    //      that have already been coded so far.
    // Lets take a different approach:
    //      Scope out actual implementenation:
    //          Road map with incremential units of work.
    //          Incorporate TDD and mark off todos and milestones.


    public class VmOpCodeAdd
    {
        IVmStack stack;
        public VmOpCodeAdd(IVmStack vmStack) => this.stack = vmStack;
        public void GenerateIl(ILGenerator il)
        {
            il.Emit(OpCodes.Call, stack.Pop);
            il.Emit(OpCodes.Call, stack.Pop);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Call, stack.Push);
        }
    }



}
