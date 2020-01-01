namespace ILEngine
{
    public class ILEngineWithActionDiagnostics : IILEngine, IOpCodeEngine
    {
        IOpCodeActionProvider opCodeActionProvider;
        public ILEngineWithActionDiagnostics(IOpCodeActionProvider opCodeActionProvider = null)
        {
            if (opCodeActionProvider == null)
            {
                opCodeActionProvider = new DefaultOpCodeActionCodeProvider();
            }
            this.opCodeActionProvider = opCodeActionProvider;
        }

        public ILStackFrameFlowControlTarget FlowControlTarget { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IILStackFrame StackFrame { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool BreakOnDebug { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool ThrowOnException { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void ExecuteFrame(ILStackFrameWithDiagnostics frame)
        {
            frame.Reset();
            goto Inc;

            ReadNext:
            frame.ReadNext();

            short opCodeValue = frame.Code.Value;
            var action = opCodeActionProvider.GetOpCodeAction(opCodeValue);
            var flowControlTarget = action(frame);
            switch (flowControlTarget)
            {
                case ILStackFrameFlowControlTarget.ReadNext:
                    goto ReadNext;
                case ILStackFrameFlowControlTarget.Ignore:
                case ILStackFrameFlowControlTarget.Inc:
                    goto Inc;
                case ILStackFrameFlowControlTarget.MoveNext:
                    goto MoveNext;
                case ILStackFrameFlowControlTarget.Ret:
                    goto Ret;

                case ILStackFrameFlowControlTarget.NotImplemented:
                default:
                    throw new OpCodeNotImplementedException(opCodeValue);
            }

            Inc:
            frame.Inc();

            MoveNext:
            if (frame.MoveNext())
                goto ReadNext;


            Ret:
            frame.ReturnResult = (frame.Stack.Count > 0 && frame.Exception == null) ? frame.Stack.Pop() : null;
        }

        public void ExecuteFrame(IILStackFrame frame)
        {
            throw new System.NotImplementedException();
        }
    }
}
