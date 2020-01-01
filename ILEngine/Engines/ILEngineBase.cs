using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{
    public abstract class ILEngineBase : IILEngine
    {
        protected ILStackFrameFlowControlTarget flowControlTarget;
        protected IILStackFrame frame;
        public ILStackFrameFlowControlTarget FlowControlTarget { get => flowControlTarget; set => flowControlTarget = value; }
        public IILStackFrame StackFrame { get => frame; set => frame = value; }
        public bool BreakOnDebug { get; set; } = false;
        public bool ThrowOnException { get; set; } = false;

        public abstract void ExecuteFrame(IILStackFrame frame);

    }
}
