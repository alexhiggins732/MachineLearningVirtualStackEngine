using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILDisassembler
{
    class Program
    {
        static void Main(string[] args)
        {
            for (var i = 0; i < 10; i++)
            {
                AddIt(i);
            }
            for (var k = 0; k < 10; k++)
            {
                AddIt(k);
            }

            for (var i = 0; i < 10; i++)
            {
                for (var k = 0; k < 10; k++)
                {
                    AddIt(i, k);
                }
            }

        }

        private static void AddIt(int i, int k)
        {
            int j = i + k;
        }

        private static void AddIt(int i)
        {
            int k = i + 1;
        }

        private static void Unary()
        {
            bool i = false;
            var j = true;
            var k = !j;
            var n = ~1;
        }
    }
}
