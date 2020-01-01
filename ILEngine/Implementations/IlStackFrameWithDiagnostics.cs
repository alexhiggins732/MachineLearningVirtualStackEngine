
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ILEngine
{

    public class ILStackFrameWithDiagnostics : IILInstructionResolver, IILStackFrame
    {
        public List<ILInstruction> Stream { get; set; }
        public dynamic ReturnResult { get; set; }
        public Exception Exception { get; set; }
        public ILOperandStack Stack { get; set; }

        public object[] Args { get; set; }
        public ILVariable[] Locals { get; set; }
        public ILInstruction Current { get; set; }
        public OpCode Code { get; set; }
        public int Position { get; set; }
        public Dictionary<int, int> JumpTable { get; set; }


        public IILInstructionResolver Resolver { get; set; }

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
            while (Stack.Count > 0)
                Stack.Pop();
            Resolver = Resolver ?? ILInstructionResolver.ExecutingAssemblyResolver;
            Current = default(ILInstruction);
            Code = default(OpCode);

            ExecutedInstructions = 0;
            Position = -1;
            if (Stream.Count > 0)
            {
                for (var i = 1; i < Stream.Count; i++)
                {
                    var instruction = Stream[i];
                    var previous = Stream[i - 1];
                    instruction.ByteIndex = previous.ByteIndex + previous.ByteSize;
                    Stream[i] = instruction;
                }
                var labelTargets = Stream.Where(x => x.Label.HasValue).ToDictionary(x => x.Label.Value, x => x);
                var labeledInstuctions = Stream.Where(x => x.Arg != null
                     && (x.Arg is ILInstruction) || (x.Arg is ILInstruction[])

                ).ToDictionary(x => x.ByteIndex, x => x);
                JumpTable = Stream.ToDictionary(x => (int)x.ByteIndex, x => ++Position);
                if (labelTargets.Count > 0)
                {
                    foreach (var kvp in labeledInstuctions)
                    {
                        var instruction = kvp.Value;
                        var endPos = instruction.ByteIndex + instruction.ByteSize;
                        if (instruction.JumpTargets != null)
                        {
                            if (instruction.JumpTargets.Length == 1)
                                instruction.Arg = instruction.JumpTargets[0];
                            else
                                instruction.Arg = instruction.JumpTargets;
                        }

                        if (instruction.Arg is ILInstruction jumpTarget)
                        {
                            instruction.JumpTargets = new[] { jumpTarget };
                            if (!jumpTarget.Label.HasValue)
                                throw new InvalidOperationException($"Label has not been marked for {instruction}.");
                            var lbl = (int)jumpTarget.Label;
                            if (!labelTargets.ContainsKey(lbl))
                                throw new InvalidOperationException($"Label points to an invalid label {lbl} for {instruction}.");
                            var dest = labelTargets[lbl];
                            instruction.Arg = (int)(dest.ByteIndex - endPos);
                        }
                        else
                        {

                            var targets = (ILInstruction[])instruction.Arg;
                            instruction.JumpTargets = targets;//store the target instructions for reuse if the instructions get rearranged.
                            int[] offsets = new int[targets.Length];
                            for (var k = 0; k < targets.Length; k++)
                            {
                                var target = targets[k];
                                if (!target.Label.HasValue)
                                    throw new InvalidOperationException($"Label[{k}] has not been marked for {instruction}.");
                                var lbl = (int)target.Label;
                                if (!labelTargets.ContainsKey(lbl))
                                    throw new InvalidOperationException($"Label[{k}] points to an invalid label {lbl} for {instruction}.");
                                var dest = labelTargets[lbl];
                                offsets[k] = (int)(dest.ByteIndex - endPos);
                            }
                            instruction.Arg = offsets;
                        }
                        int instructionIndex = JumpTable[(int)instruction.ByteIndex];
                        Stream[instructionIndex] = instruction;
                    }
                    
                }



                Position = -1;
            }




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

        public virtual void Execute(int timeout = 0, bool throwOnException = false)
        {
            var engine = new ILEngineCompiled();
            Execute(engine, timeout, throwOnException);
        }
        public void Execute(IILEngine engine, int timeout = 0, bool throwOnException = false)
        {
            engine.ThrowOnException = throwOnException;
            try
            {
                Action action = () => engine.ExecuteFrame(this);
                if (timeout == 0)
                    action();
                else
                {
                    var result = action.WithTimeout(timeout);
                    if (result.Exception != null)
                        this.Exception = result.Exception;
                }
            }
            catch (Exception ex)
            {
                this.Exception = ex;
            }

        }

        public void SetResolver(MethodInfo method)
        {
            Resolver = new ILInstructionResolver(method);
        }
        public void SetResolver(Type type) => SetResolver(type.Module);

        public void SetResolver(Module module)
        {
            Resolver = new ILInstructionResolver(module);
        }

        public void CopyFrom(MethodInfo method)
        {
            var body = method.GetMethodBody();
            var il = ILInstructionReader.FromByteCode(body.GetILAsByteArray());
            Stream = il;
            Locals = body.LocalVariables.Count ==0 ? null : body.LocalVariables.Select(lcl => new ILVariable(lcl)).ToArray();
            SetResolver(method);
        }
    }
}
