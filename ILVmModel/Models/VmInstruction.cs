using System.Collections.Generic;

namespace ILVmModel.Models
{
    public class VmInstruction
    {
        public int InstructionIndex { get; private set; }
        public int ByteIndex { get; private set; }
        public short OpCode { get; private set; }
        public List<VmInstructionArgument> Arguments { get; private set; }// should this be argum
    }
}
