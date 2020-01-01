using System.Collections.Generic;
using System.Reflection.Emit;

namespace ILEngine
{
    public class ILStackFrameBuilder<TStackFrame> 
        : IILStackFrameBuilder<TStackFrame> where TStackFrame : IILStackFrame, new()
    {
        public TStackFrame Build(List<OpCode> opCodes,
            IILInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            var streamWriter = new ILInstructionWriter(opCodes);
            var stream = streamWriter.GetInstructionStream();
            return Build(stream, resolver, args, locals);
        }
        public TStackFrame Build(List<ILInstruction> stream,
            IILInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            var result = new TStackFrame
            {
                Stream = stream,
                Resolver = resolver,
                Args = args,
                Locals = locals
            };
            return result;
        }


        public TStackFrame BuildAndExecute(List<OpCode> opCodes,
            IILInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            return BuildAndExecute(opCodes, 0, resolver, args, locals);
        }

        public TStackFrame BuildAndExecute(List<OpCode> opCodes, int timeout,
            IILInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            var frame = Build(opCodes, resolver, args, locals);
            Execute(frame, timeout);
            return frame;
        }


        public TStackFrame BuildAndExecute(List<ILInstruction> stream,
            IILInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            return BuildAndExecute(stream, 0, resolver, args, locals);
        }

        public TStackFrame BuildAndExecute(List<ILInstruction> stream,
            int timeout,
            IILInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            var frame = Build(stream, resolver, args, locals);
            Execute(frame, timeout);
            return frame;
        }

        public void Execute(TStackFrame frame, int timeout)
        {
            frame.Execute(timeout);
        }
    }
    
    public class ILStackFrameBuilder
    {
        public static ILStackFrameWithDiagnostics Build(List<OpCode> opCodes,
            IILInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            var streamWriter = new ILInstructionWriter(opCodes);
            var stream = streamWriter.GetInstructionStream();
            return Build(stream, resolver, args, locals);
        }
        public static ILStackFrameWithDiagnostics Build(List<ILInstruction> stream,
            IILInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            var result = new ILStackFrameWithDiagnostics
            {
                Stream = stream,
                Resolver = resolver,
                Args = args,
                Locals = locals
            };
            return result;
        }


        public static ILStackFrameWithDiagnostics BuildAndExecute(List<OpCode> opCodes,
            IILInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            return BuildAndExecute(opCodes, 0, resolver, args, locals);
        }

        public static ILStackFrameWithDiagnostics BuildAndExecute(List<OpCode> opCodes,
            int timeout,
            IILInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            var frame = Build(opCodes, resolver, args, locals);
            Execute(frame, timeout);
            return frame;
        }


        public static ILStackFrameWithDiagnostics BuildAndExecute(List<ILInstruction> stream,
            IILInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            return BuildAndExecute(stream, 0, resolver, args, locals);
        }

        public static ILStackFrameWithDiagnostics BuildAndExecute(List<ILInstruction> stream,
            int timeout,
            IILInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            var frame = Build(stream, resolver, args, locals);
            Execute(frame, timeout);
            return frame;
        }

        public static void Execute(ILStackFrameWithDiagnostics frame, int timeout)
        {
            frame.Execute(timeout);
        }
    }
}
