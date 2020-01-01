using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{
    public class ILInstructionWriter
    {
        private List<OpCode> l;

        public ILInstructionWriter(List<OpCode> l)
        {
            this.l = l;
        }

        public List<ILInstruction> GetInstructionStream()
        {
            var result = new List<ILInstruction>();
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    var idx = 0;
                    for (var i = 0; i < l.Count; i++, idx++)
                    {
                        var code = l[i];
                        if (code == OpCodes.Prefix1)
                        {
                            i++;
                            idx++;
                            code = l[i];
                        }
                        var instruction = new ILInstruction { OpCode = code, ByteIndex = idx };
                        result.Add(instruction);
                    }
                }

            }
            return result;
        }
    }
}
