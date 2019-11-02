using ILEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MLEnvironment
{
    public class VirtualOpCode : IComparable<VirtualOpCode>, IComparable
    {
        public OpCode OpCode { get; private set; }
        public VirtualOpCode(OpCode opCode)
        {
            this.OpCode = opCode;
        }

        public int CompareTo(VirtualOpCode other)
        {
            return unchecked((ushort) OpCode.Value).CompareTo(unchecked((ushort)other.OpCode.Value));
        }

        public int CompareTo(object obj)
        {
            var other = obj as VirtualOpCode;
            if (other == null) return -1;
            return CompareTo(other);
        }
        public override string ToString()
        {
            return OpCode.ToString();

        }
    }
    public class MSILActionSpace
    {
        public static MLActionSpace BuildMSILActionSpace()
        {

            Dictionary<string, OpCode> OpCodes = OpCodeLookup.OpCodesByName;

            List<MLAction> actions = null;
            actions = new List<MLAction>();
            foreach (var opcode in OpCodes)
            {
                var action = new MLAction(opcode.Key, new VirtualOpCode(opcode.Value));
                actions.Add(action);
            }
            actions= actions.OrderBy(x=> x.Value).ToList();
            var result = new MLActionSpace(nameof(MSILActionSpace), actions);
            return result;
        }
    }
}
