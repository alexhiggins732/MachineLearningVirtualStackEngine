using System.Collections.Generic;
using System.Reflection.Emit;

namespace ILEngine.Implementations
{
    public class IlStackFrameBuilder
    {
        public static IlStackFrameWithDiagnostics Build(List<OpCode> opCodes,
            IIlInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            var streamWriter = new IlInstructionWriter(opCodes);
            var stream = streamWriter.GetInstructionStream();
            return Build(stream, resolver, args, locals);
        }
        public static IlStackFrameWithDiagnostics Build(List<IlInstruction> stream,
            IIlInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            var result = new IlStackFrameWithDiagnostics
            {
                Stream = stream,
                Resolver = resolver,
                Args = args,
                Locals = locals
            };
            return result;
        }


        public static IlStackFrameWithDiagnostics BuildAndExecute(List<OpCode> opCodes,
            IIlInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            return BuildAndExecute(opCodes, 0, resolver, args, locals);
        }

        public static IlStackFrameWithDiagnostics BuildAndExecute(List<OpCode> opCodes, int timeout,
            IIlInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            var frame = Build(opCodes, resolver, args, locals);
            Execute(frame, timeout);
            return frame;
        }


        public static IlStackFrameWithDiagnostics BuildAndExecute(List<IlInstruction> stream,
            IIlInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            return BuildAndExecute(stream, 0, resolver, args, locals);
        }

        public static IlStackFrameWithDiagnostics BuildAndExecute(List<IlInstruction> stream,
            int timeout,
            IIlInstructionResolver resolver = null,
            object[] args = null,
            ILVariable[] locals = null)
        {
            var frame = Build(stream, resolver, args, locals);
            Execute(frame, timeout);
            return frame;
        }

        public static void Execute(IlStackFrameWithDiagnostics frame, int timeout)
        {
            frame.Execute(timeout);
        }
    }
}
