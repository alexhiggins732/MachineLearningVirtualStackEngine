using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{
    public class InvalidOpCodeException : Exception
    {
        private short opCodeValue;

        public InvalidOpCodeException(short opCodeValue) : base($"Invalid Opcode {opCodeValue}.")
        {
            this.opCodeValue = opCodeValue;
        }

        public InvalidOpCodeException(OpCode opCode) : base($"Invalid Opcode {opCode}.")
        {
            this.opCodeValue = opCode.Value;
        }

        public InvalidOpCodeException(ILOpCodeValues opCodeValue) : base($"Invalid Opcode {opCodeValue}.")
        {
            this.opCodeValue = unchecked((short)opCodeValue);
        }
    }
}
