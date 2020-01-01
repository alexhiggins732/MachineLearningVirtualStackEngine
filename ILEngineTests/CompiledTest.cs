using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ILEngine.Tests
{
    /// <summary>
    /// Metadata for inline compiled tests defined in <see cref="ILEngineOpcodeTestMethodHelper.TestMethods"/>
    /// </summary>
    public class CompiledTest
    {
        public MethodInfo Method;
        public OpCodeTestAttribute TestAttribute;
        public ILOpCodeValues ILOpCodeValue;
        public OpCode OpCode;

        public CompiledTest(MethodInfo method, OpCodeTestAttribute testAttribute)
        {
            this.Method = method;
            var parsed = Enum.TryParse(method.Name, out ILOpCodeValues ILOpCodeValue);
            this.OpCode = OpCodeLookup.GetILOpcode((int)ILOpCodeValue);
            if (ILOpCodeValue.ToString() != Method.Name)
            {
                throw new NotImplementedException($"Iinvalid Method: '{Method.Name}'. Method names must have a matching IlOpCodeValue");
            }
            this.TestAttribute = testAttribute;
        }

        public override string ToString() => $"{Method}";

    }
}
