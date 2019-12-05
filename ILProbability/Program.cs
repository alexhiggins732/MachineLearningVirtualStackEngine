using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILProbability
{
    class Program
    {
        static void Main(string[] args)
        {

            MASamplerTests.RunTests();
            if (bool.Parse(bool.TrueString)) return;
            var probs = new PopcountProbabilities();
            var l = new List<ProbabilityAverageSample>();
            for(var i=1; i<=32; i++)
            {
                var sample = StaticRandomTest.ProbablitiesInlineVarTest(i);
                l.Add(sample);
                Console.WriteLine(sample);
            }
        }
    }
}
