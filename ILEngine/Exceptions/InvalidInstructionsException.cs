using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{
    public class InvalidInstructionsException : Exception
    {
        public InvalidInstructionsException(string message) :base(message) { }
        
    }
}
