#define testcustom
#undef testcustom
using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace ILEngine.Tests
{
    [TestClass()]
    public class ILEngineCompiledGenericTests : 
        ILEngineTestRunnerBase<ILEngineCompiled, ILStackFrameWithDiagnostics<ILEngineCompiled>>
    {
        [ExpectedException(typeof(OpCodeNotImplementedException))]
        [TestMethod()]
        public override void ExecuteOpCode_Test()
        {
            var engine = NewEngine();
            engine.StackFrame = NewFrame();
            engine.ExecuteOpCode(300);
            Assert.IsNotNull(engine.StackFrame.Exception, "Engine exception not set");
            throw engine.StackFrame.Exception;
        }
    }
}