using System;
using System.Linq;

namespace ILEngine.Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OpCodeUIntTestAttribute : OpCodeTestAttribute
    {
        public uint[] Parameters;

        public OpCodeUIntTestAttribute(params uint[] args)
        {
            Expected = args[0];
            Parameters = args.Skip(1).ToArray();
        }


        public uint Expected { get; set; }
        public bool Equals(uint a, uint b) => a == b;
        public bool LessThan(uint a, uint b) => a < b;
        public bool LessThanOrEqual(uint a, uint b) => a <= b;
        public bool GreaterThanOrEqual(uint a, uint b) => a >= b;
        public bool GreaterThan(uint a, uint b) => a > b;
    }
}
