
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{
    public interface IILStackFrame<TEngine>
        : IILStackFrame
        where TEngine : IILEngine
    {

    }
    public interface IILStackFrame
    {
        ILOperandStack Stack { get; }
        object[] Args { get; set; }
        OpCode Code { get; set; }
        ILInstruction Current { get; set; }
        Exception Exception { get; set; }
        int ExecutedInstructions { get; set; }
        Dictionary<int, int> JumpTable { get; set; }
        ILVariable[] Locals { get; set; }
        Module Module { get; }
        int Position { get; set; }
        Func<int, FieldInfo> ResolveFieldToken { get; }
        Func<int, MemberInfo> ResolveMemberToken { get; }
        Func<int, MethodBase> ResolveMethodToken { get; }
        IILInstructionResolver Resolver { get; set; }
        Func<int, byte[]> ResolveSignatureToken { get; }
        Func<int, string> ResolveStringToken { get; }
        Func<int, Type> ResolveTypeToken { get; }
        dynamic ReturnResult { get; set; }

        List<ILInstruction> Stream { get; set; }
        bool TriggerBreak { get; set; }

        void SetResolver(Type type);
        void SetResolver(MethodInfo method);
        void SetResolver(Module module);
        void Execute(int timeout = 0, bool throwOnException = false);

        void Execute(IILEngine engine, int timeout = 0, bool throwOnException = false);
        void Inc();
        bool MoveNext();
        void ReadNext();
        void Reset();
        void CopyFrom(MethodInfo method);
    }
    public interface IILEngine
    {
        ILStackFrameFlowControlTarget FlowControlTarget { get; set; }
        IILStackFrame StackFrame { get; set; }

        bool BreakOnDebug { get; set; }
        bool ThrowOnException { get; set; }
        void ExecuteFrame(IILStackFrame frame);
    }
}
