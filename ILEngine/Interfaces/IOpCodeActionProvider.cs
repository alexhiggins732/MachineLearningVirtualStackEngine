using System;

namespace ILEngine
{
    public interface IOpCodeActionProvider
    {
        Func<ILStackFrameWithDiagnostics, ILStackFrameFlowControlTarget> GetOpCodeAction(short opCodeValue);
    }
}
