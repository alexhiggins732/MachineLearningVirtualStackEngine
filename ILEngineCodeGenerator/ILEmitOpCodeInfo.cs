using System.Reflection;
using System.Reflection.Emit;

namespace ILEngine.CodeGenerator
{
    public abstract class ILEmitOpCodeInfo
    {
        public Label Label;
        public OpCode OpCode;
        public string CsExpression;
        public ILEmitOpCodeInfo(OpCode opCode)
        {
            this.OpCode = opCode;
        }
        public ILEmitOpCodeInfo(OpCode opCode, Label label)
        {
            this.OpCode = opCode;
            this.Label = label;

        }

        public abstract void EmitMethodCallInstructions(ILGenerator il);
        public abstract void EmitInstructions(ILGenerator il, MethodInfo pushMethod, MethodInfo popMethod);


    }

}
