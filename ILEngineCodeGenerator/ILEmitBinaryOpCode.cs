using System.Reflection;
using System.Reflection.Emit;

namespace ILEngine.CodeGenerator
{
    /// <summary>
    /// Pops two values off the caller's stack, executes the <see cref="ILEmitBinaryOpCode.OpCode"/> and pushes the result back onto the callers stack
    /// </summary>
    public class ILEmitBinaryOpCode : ILEmitOpCodeInfo
    {

        public ILEmitBinaryOpCode(OpCode opCode) : base(opCode)
        {
        }
        public ILEmitBinaryOpCode(OpCode opCode, Label label) : base(opCode, label)
        {
        }
        public ILEmitBinaryOpCode(OpCode opCode, string csExpression) : base(opCode)
        {
            this.CsExpression = csExpression;
        }


        public override void EmitMethodCallInstructions(ILGenerator il)
        {

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Unbox_Any, typeof(int));
            //il.Emit(OpCodes.Ldarg_1);
            //il.Emit(OpCodes.Unbox_Any, typeof(int));

            il.Emit(OpCodes.Ret);

        }
        /// <summary>
        /// Emits il to the <paramref name="ilGenerator"/> that to pops two values from the caller's stack using the <paramref name="popMethod"/> and then executes the <see cref="ILEmitUnaryOpCode.Opcode"/>.
        /// The result is then pushed onto the caller's stack using the <paramref name="pushMethod"/>.
        public override void EmitInstructions(ILGenerator il, MethodInfo pushMethod, MethodInfo popMethod)
        {

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, popMethod);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, popMethod);
            il.Emit(this.OpCode);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, pushMethod);
            il.Emit(OpCodes.Ret);
        }
    }

}
