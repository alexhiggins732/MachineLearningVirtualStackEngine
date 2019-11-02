using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLEnvironment
{
    class Program
    {
        public static void Main(string[] args)
        {
            TestActionSpace();
        }

        private static void TestActionSpace()
        {
            var space = MSILActionSpace.BuildMSILActionSpace();
            Console.WriteLine(space.ValueRange.MaxValue);
            Console.WriteLine(space.ValueRange.MinValue);
        }
    }
}
