using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;

namespace ILEngine
{
    /// <summary>
    /// Attribute used to bind Methods to <see cref="System.Reflection.Emit.OpCode"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [ExcludeFromCodeCoverage]
    public class OpCodeValueAttribute : Attribute
    {
        public int OpcodeValue { get; private set; }
        public ILOpCodeValues ILOpcode => (ILOpCodeValues)OpcodeValue;
        public OpCode OpCode => OpCodeLookup.GetILOpcode(OpcodeValue);
        public OpCodeValueAttribute(int value)
        {
            this.OpcodeValue = (short)value;
        }
    }


}
