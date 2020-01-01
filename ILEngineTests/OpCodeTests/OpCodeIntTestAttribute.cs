using System;
using System.Linq;

namespace ILEngine.Tests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OpCodeIntTestAttribute : OpCodeTestAttribute
    {
        public int[] Parameters;

        public OpCodeIntTestAttribute(params int[] args)
        {
            Expected = args[0];
            Parameters = args.Skip(1).ToArray();
        }


        public int Expected { get; set; }
        public bool Equals(int a, int b) => a == b;
        public bool LessThan(int a, int b) => a < b;
        public bool LessThanOrEqual(int a, int b) => a <= b;
        public bool GreaterThanOrEqual(int a, int b) => a >= b;
        public bool GreaterThan(int a, int b) => a > b;
    }
}
