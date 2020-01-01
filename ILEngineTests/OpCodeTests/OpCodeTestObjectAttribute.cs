using System;

namespace ILEngine.Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OpCodeTestObjectAttribute : OpCodeTestAttribute
    {
        public object Arg1 { get; set; }
        public object Arg2 { get; set; }
        public object Expected { get; set; }
    }
}
