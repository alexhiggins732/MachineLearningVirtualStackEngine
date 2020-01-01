using System.Reflection;
using System.Reflection.Emit;

namespace ILEngine.CodeGenerator
{
    /// <summary>
    /// Pops two values off the caller's stack, executes the <see cref="ILEmitUnaryOpCode.OpCode"/> and pushes the result back onto the callers stack
    /// </summary>
    public class ILEmitUnaryOpCode : ILEmitOpCodeInfo
    {

        public ILEmitUnaryOpCode(OpCode opCode) : base(opCode)
        {
        }

        public ILEmitUnaryOpCode(OpCode opCode, string csExpression) : base(opCode)
        {
            this.CsExpression = csExpression;
        }
        public ILEmitUnaryOpCode(OpCode opCode, Label label) : base(opCode, label)
        {
        }

        public override void EmitMethodCallInstructions(ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(this.OpCode);
            il.Emit(OpCodes.Ret);

        }

        /// <summary>
        /// Emits il to the <paramref name="ilGenerator"/> that to pops one value from the caller's stack using the <paramref name="popMethod"/> and then executes the <see cref="ILEmitUnaryOpCode.Opcode"/>.
        /// The result is then pushed onto the caller's stack using the <paramref name="pushMethod"/>.
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="pushMethod"></param>
        /// <param name="popMethod"></param>
        public override void EmitInstructions(ILGenerator ilGenerator, MethodInfo pushMethod, MethodInfo popMethod)
        {

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, popMethod);
            ilGenerator.Emit(this.OpCode);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, pushMethod);
            ilGenerator.Emit(OpCodes.Ret);
        }
    }

}
