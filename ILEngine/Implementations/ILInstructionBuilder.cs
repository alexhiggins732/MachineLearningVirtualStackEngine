using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{
    public class ILInstructionBuilder
    {
        public List<ILInstruction> Instructions = new List<ILInstruction>();

        public void Write(ILInstruction instruction) => Instructions.Add(instruction);
        public void Write(params ILInstruction[] instructions) => Instructions.AddRange(instructions);
        public void Write(OpCode opCode) => Instructions.Add(ILInstruction.Create(opCode));
        public void Write(params OpCode[] opCodes) => Instructions.AddRange(opCodes.Select(opCode => ILInstruction.Create(opCode)));
        public void Write(OpCode opCode, object arg) => Instructions.Add(ILInstruction.Create(opCode, arg));

        public void Write(ILOpCodeValues opCodeValue) => Instructions.Add(ILInstruction.Create(opCodeValue));
        public void Write(params ILOpCodeValues[] opCodeValues) => Instructions.AddRange(opCodeValues.Select(x => ILInstruction.Create(x)));
        public void Write(ILOpCodeValues opCodeValue, object arg) => Instructions.Add(ILInstruction.Create(opCodeValue, arg));

        public void Clear()
        {
            Instructions.Clear();
        }

    
    }
}
