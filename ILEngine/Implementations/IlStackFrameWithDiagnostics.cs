using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ILEngine.Implementations
{
    public class IlStackFrameWithDiagnostics : IIlInstructionResolver
    {
        public List<IlInstruction> Stream { get; set; }
        public dynamic ReturnResult { get; set; }
        public Exception Exception { get; set; }
        public ILOperandStack Stack { get; set; }

        public object[] Args { get; set; }
        public ILVariable[] Locals { get; set; }
        public IlInstruction Current { get; set; }
        public OpCode Code { get; set; }
        public int Position { get; set; }
        public Dictionary<int, int> JumpTable { get; set; }


        public IIlInstructionResolver Resolver { get; set; }

        public Func<int, FieldInfo> ResolveFieldToken => Resolver.ResolveFieldToken;

        public Module Module => Resolver.Module;

        public Func<int, MemberInfo> ResolveMemberToken => Resolver.ResolveMemberToken;

        public Func<int, MethodBase> ResolveMethodToken => Resolver.ResolveMethodToken;

        public Func<int, byte[]> ResolveSignatureToken => Resolver.ResolveSignatureToken;

        public Func<int, string> ResolveStringToken => Resolver.ResolveStringToken;

        public Func<int, Type> ResolveTypeToken => Resolver.ResolveTypeToken;

        public bool TriggerBreak { get; set; }
        public void Reset()
        {
            ReturnResult = Exception = null;
            Stack = Stack ?? new ILOperandStack();
            Resolver = Resolver ?? IlInstructionResolver.ExecutingAssemblyResolver;
            Current = default(IlInstruction);
            Code = default(OpCode);
            Position = -1;
            ExecutedInstructions = 0;
        }
        public void ReadNext()
        {
            Current = Stream[Position];
            Code = Current.OpCode;
            ExecutedInstructions++;
        }
        public void Inc()
        {
            Position++;
        }
        public bool MoveNext()
        {
            return Position < Stream.Count;
        }
        public int ExecutedInstructions { get; set; }

        public void Execute(int timeout = 0)
        {
            var engine = new IlEngineWithDiagnostics();
            try
            {
                Action action = () => engine.ExecuteFrame(this);
                if (timeout == 0)
                    action();
                else
                    action.WithTimeout(timeout);
            }
            catch (Exception ex)
            {
                this.Exception = ex;
            }

        }
    }
}
