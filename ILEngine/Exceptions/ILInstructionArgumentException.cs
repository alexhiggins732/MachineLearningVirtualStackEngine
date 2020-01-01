using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{
    public class ILInstructionArgumentException : ArgumentException
    {
        public ILInstructionArgumentException(string message) : base(message)
        {
        }
    }
}
