
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ILEngine
{
    public class ILStackFrameWithDiagnostics<TEngine> :
        ILStackFrameWithDiagnostics,
        IILInstructionResolver,
        IILStackFrame<TEngine>
        where TEngine : IILEngine, new()
    {


        public override void Execute(int timeout = 0, bool throwOnException = false)
        {
            var engine = new TEngine();
            base.Execute(engine, timeout, throwOnException);
            engine.ThrowOnException = throwOnException;
        }
    }
}
