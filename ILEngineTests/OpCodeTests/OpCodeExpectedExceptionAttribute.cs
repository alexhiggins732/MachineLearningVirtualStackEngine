using System;
using System.Linq;

namespace ILEngine.Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OpCodeExpectedExceptionAttribute : OpCodeTestAttribute
    {
        public Type ExpectedExceptionType;
        public object[] Parameters;
        public object Expected;
        public OpCodeExpectedExceptionAttribute(Type expectedExceptionType, object expected, params object[] parameters)
        {
            this.ExpectedExceptionType = expectedExceptionType;
            this.Expected = expected;
            this.Parameters = parameters.ToArray();
        }
    }
}
