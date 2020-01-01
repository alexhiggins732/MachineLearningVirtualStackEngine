using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ILEngine.Tests
{
    [TestClass()]
    public class ILStackFrameWithDiagnosticsGenericTests
    : ILStackFrameWithDiagnosticsTestBase
        <ILStackFrameWithDiagnostics<ILEngineCompiled>,
        ILStackFrameBuilder<ILStackFrameWithDiagnostics<ILEngineCompiled>>, 
        ILEngineCompiled>
    {

    }
}