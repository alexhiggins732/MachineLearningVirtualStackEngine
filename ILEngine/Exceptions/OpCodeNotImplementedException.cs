using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{
    public class OpCodeNotImplementedException : NotImplementedException
    {
        private short opCodeValue;

        public OpCodeNotImplementedException(short opCodeValue) : base($"Opcode {opCodeValue} is not implemented")
        {
            this.opCodeValue = opCodeValue;
        }

        public OpCodeNotImplementedException(OpCode opCode) : base($"Opcode {opCode} is not implemented")
        {
            this.opCodeValue = opCode.Value;
        }

        public OpCodeNotImplementedException(ILOpCodeValues opCodeValue) : base($"Opcode {opCodeValue} is not implemented")
        {
            this.opCodeValue = unchecked((short)opCodeValue);
        }
    }
}
