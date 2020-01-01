using System.Collections.Generic;
using System.Reflection.Emit;

namespace ILEngine
{
    public interface IILStackFrameBuilder<TStackFrame>
        where TStackFrame : IILStackFrame, new()
    {
        TStackFrame Build(List<ILInstruction> stream, IILInstructionResolver resolver = null, object[] args = null, ILVariable[] locals = null);
        TStackFrame Build(List<OpCode> opCodes, IILInstructionResolver resolver = null, object[] args = null, ILVariable[] locals = null);
        TStackFrame BuildAndExecute(List<ILInstruction> stream, IILInstructionResolver resolver = null, object[] args = null, ILVariable[] locals = null);
        TStackFrame BuildAndExecute(List<ILInstruction> stream, int timeout, IILInstructionResolver resolver = null, object[] args = null, ILVariable[] locals = null);
        TStackFrame BuildAndExecute(List<OpCode> opCodes, IILInstructionResolver resolver = null, object[] args = null, ILVariable[] locals = null);
        TStackFrame BuildAndExecute(List<OpCode> opCodes, int timeout, IILInstructionResolver resolver = null, object[] args = null, ILVariable[] locals = null);
        void Execute(TStackFrame frame, int timeout);
    }
}