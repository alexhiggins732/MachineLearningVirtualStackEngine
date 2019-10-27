using System;
using System.Reflection.Emit;

namespace ILEngine
{
    /// <summary>
    /// Attribute used to bind Methods to <see cref="System.Reflection.Emit.OpCode"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class OpcodeValueAttribute : Attribute
    {
        public int OpcodeValue { get; private set; }
        public ILOpCodeValues ILOpcode => (ILOpCodeValues)OpcodeValue;
        public OpCode OpCode => OpCodeLookup.GetILOpcode(OpcodeValue);
        public OpcodeValueAttribute(int value)
        {
            this.OpcodeValue = (short)value;
        }
    }


}
