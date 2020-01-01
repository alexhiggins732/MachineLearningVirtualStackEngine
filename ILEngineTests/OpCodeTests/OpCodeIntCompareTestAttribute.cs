using System;
using System.Linq;

namespace ILEngine.Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OpCodeIntCompareTestAttribute : OpCodeTestAttribute
    {
        public int[] Parameters;

        public OpCodeIntCompareTestAttribute(bool expected, params int[] args)
        {
            Expected = expected;
            Parameters = args.ToArray();
        }


        public bool Expected { get; set; }
        public bool Equals(bool a, bool b) => a == b;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OpCodeUIntCompareTestAttribute : OpCodeTestAttribute
    {
        public uint[] Parameters;

        public OpCodeUIntCompareTestAttribute(bool expected, params uint[] args)
        {
            Expected = expected;
            Parameters = args.ToArray();
        }


        public bool Expected { get; set; }
        public bool Equals(bool a, bool b) => a == b;
    }
}
