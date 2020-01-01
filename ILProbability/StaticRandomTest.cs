using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ILProbability
{
    public class PopCountSampler
    {
        public int Bits;

        public int[] PopCounts;
        public int[] ColumnCounts;
        public int[][] History;
        private int sampleCount;
        public PopCountSampler(int bits)
        {
            this.Bits = bits;
            this.PopCounts = new int[bits+1];
            this.ColumnCounts = new int[bits+1];
            this.History = new int[bits][];
            for (var i = 0; i < bits; i++)
            {
                History[i] = new int[bits];
            }
        }

        public void Sample(uint n)
        {
            sampleCount++;
            int max = Bits - 1;
            for (var row = 0; row < max; row++)
            {
                Array.Copy(History[row + 1], History[row], Bits);
            }
            int popCount = 0;
            var lastRow = History[max];
            for (var column = 0; column < Bits; column++)
            {
                popCount += (lastRow[column] = (int)(n & 1));
                n >>= 1;
            }
            PopCounts[popCount]++;
            if (sampleCount >= Bits)
            {
                for (var column = 0; column < Bits; column++)
                {
                    popCount = 0;
                    for (var row = 0; row < Bits; row++)
                    {
                        popCount += History[row][column];
                    }
                    if (popCount == 0 || popCount==Bits)
                    {
                        string bp = "";
                    }
                    ColumnCounts[popCount]++;
                }
            }




        }
        public int NumSamples => PopCounts.Sum();

        public static void RunTest()
        {
            var sampler = new PopCountSampler(32);
            for (var i = 0; i < 1000000; i++)
            {
                var sample = StaticRandom.NextUInt();
                sampler.Sample(sample);
            }
            var rowSamples = sampler.PopCounts;
            var columnSamples = sampler.ColumnCounts;
            var rowJson = JsonConvert.SerializeObject(rowSamples);
            var columnJson = JsonConvert.SerializeObject(columnSamples);
            var rowCsv = string.Join("\r\n", rowSamples);
            var columnCsv = string.Join("\r\n", columnSamples);

        }
    }
    public class PopcountProbabilities
    {
        public PopcountProbabilities()
        {
            //given 1 bit [0] {true:1/2, false:1/2}
            //given 1 bit [0] {true:1/2, false:1/2}
            // var pop32 = new Dictionary<>
            var counts = new List<Dictionary<int, int>>();
            for (var bits = 1; bits <= 32; bits++)
            {

                ulong maxValue = uint.MaxValue;
                ulong limit = (1ul << bits) - 1;
                var data = Enumerable.Range(0, bits + 1).ToDictionary(x => x, x => 0);

                for (ulong i = 0; i <= maxValue && i <= limit; i++)
                {
                    var count = PopCount(i);
                    data[count]++;
                }
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                counts.Add(data);
                var json2 = JsonConvert.SerializeObject(counts, Formatting.Indented);
                var json3 = JsonConvert.SerializeObject(counts);

            }

            foreach (var dict in counts)
            {

            }

        }
        static BigInteger Factorial(int n)
        {
            BigInteger result = BigInteger.One;
            for (var i = 1; i <= n; i++)
                result *= i;
            return result;
        }
        /// <summary>
        /// A000245
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        static int MidProbability(int bits)
        {
            var n = bits;
            var res = (3 * Factorial(2 * n)) / (Factorial(n + 2) * Factorial(n - 1));
            return (int)res;
        }

        /// <summary>
        /// Catalan numbers: C(n) = binomial(2n,n)/(n+1) = (2n)!/(n!(n+1)!). Also called Segner numbers. 
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static int Catalan(int bits)
        {
            var f2n = Factorial(bits * 2);
            var fn = Factorial(bits);
            var fnp1 = Factorial(bits + 1);
            return (int)(f2n / (fn * fnp1));
        }
        public static void CatalanTests()
        {
            var cats = Enumerable.Range(0, 15).ToDictionary(x => x, x => Catalan(x));
            var catsJson = JsonConvert.SerializeObject(cats, Formatting.Indented);
            Console.WriteLine(catsJson);
        }

        /// <summary>
        /// a(n-1) = binomial(2*n, n)/2 = (2*n)!/(2*n!*n!).
        /// </summary>
        /// <param name="bits"></param>
        static int FibMiddle(int bits)
        {
            var f2n = Factorial(bits * 2);
            var fn = Factorial(bits);
            var fnSq = fn * fn;
            var fnSqX2 = 2 * fnSq;
            var q = f2n / fnSqX2;
            return (int)q;
        }
        public static void FibMiddleTests()
        {
            var coeffs = Enumerable.Range(0, 15).ToDictionary(x => x, x => FibMiddle(x));
            var coeffsJson = JsonConvert.SerializeObject(coeffs, Formatting.Indented);
            Console.WriteLine(coeffsJson);
        }

        public static void MidDeltaTests()
        {
            var mids = Enumerable.Range(0, 15).ToDictionary(x => x, x => MidProbability(x));
            var mid1 = MidProbability(1);

            var mid3 = MidProbability(3);
            var mid5 = MidProbability(5);
            var mid7 = MidProbability(7);
            var mid9 = MidProbability(9);
            var mid11 = MidProbability(11);
            var mid13 = MidProbability(13);
            var mid15 = MidProbability(15);
        }

        private int PopCount(ulong i)
        {
            int pop = 0;
            while (i > 0)
            {
                if (1 == (i & 1)) pop++;
                i >>= 1;
            }
            return pop;
        }
    }
    public class ProbabilityAverageSample
    {
        public int NumVariables;
        public int NumTests;
        public int SamplesPerTest;
        public double AverageAnyTrue;
        public double AverageAnyFalse;
        public double AverageAllTrue;
        public double AverageAllFalse;

        public ProbabilityAverageSample(int numVariables, int numTests, int samplesPerTest, double averageAnyTrue, double averageAnyFalse, double averageAllTrue, double averageAllFalse)
        {
            this.NumVariables = numVariables;
            this.NumTests = numTests;
            this.SamplesPerTest = samplesPerTest;
            this.AverageAnyTrue = averageAnyTrue;
            this.AverageAnyFalse = averageAnyFalse;
            this.AverageAllTrue = averageAllTrue;
            this.AverageAllFalse = averageAllFalse;
        }

        public override string ToString()
        {
            return $@"With variables = {NumVariables}, As num samples => infinity AnyTrueProbability =>  {AverageAnyTrue.ToString("P")} and AnyFalse probablity => . {AverageAnyTrue.ToString("P")}.
With variables = {NumVariables}, As num samples => infinity AllTrueProbability =>  {AverageAllTrue.ToString("P")} and AllFalse probablity => .{AverageAllFalse.ToString("P")}";
        }
    }
    public class BooleanProbabilitySample
    {
        public int NumSamples;
        public int TrueCount;
        public int FalseCount;

        public BooleanProbabilitySample(int numSamples, int trueCount, int falseCount)
        {
            NumSamples = numSamples;
            TrueCount = trueCount;
            FalseCount = falseCount;
        }

        public double TrueProbability => (double)TrueCount / NumSamples;
        public double FalseProbability => (double)FalseCount / NumSamples;
    }
    public class MultiBooleanProbabilitySample
    {
        public int NumSamples;
        public int AnyTrueCount;
        public int AnyFalseCount;
        public int AllTrueCount;
        public int AllFalseCount;

        public MultiBooleanProbabilitySample(int numSamples, int anyTrueCount, int anyFalseCount, int allTrueCount, int allFalseCount)
        {
            NumSamples = numSamples;
            AnyTrueCount = anyTrueCount;
            AnyFalseCount = anyFalseCount;
            AllTrueCount = allTrueCount;
            AllFalseCount = allFalseCount;
        }


        public double AnyTrueProbability => (double)AnyTrueCount / NumSamples;
        public double AnyFalseProbability => (double)AnyFalseCount / NumSamples;

        public double AllTrueProbability => (double)AllTrueCount / NumSamples;
        public double AllFalseProbability => (double)AllFalseCount / NumSamples;


    }
    public class StaticRandomTest
    {
        public static void RunTest()
        {
            int _boolMask = 32;
            uint _boolBits = StaticRandom.NextUInt();
            List<bool> samples = new List<bool>();
            for (var i = 0; i < 32; i++)
            {
                _boolMask -= 1;
                samples.Add((_boolBits & 1) == 0);
            }
        }


        public static void ProbablitiesOneVarTest()
        {
            int numTests = 1000;
            var results = new List<BooleanProbabilitySample>();
            for (var i = 0; i < numTests; i++)
            {
                int numSamples = 10000;
                List<bool> samples = Enumerable.Range(0, numSamples).Select(x => StaticRandom.NextBool()).ToList();
                int trueCount = samples.Where(x => x).Count();
                int falseCount = samples.Where(x => !x).Count();

                double truthProbability = (double)trueCount / numSamples;
                double falseProbability = (double)falseCount / numSamples;
                results.Add(new BooleanProbabilitySample(numSamples, trueCount, falseCount));
            }
            var averageTruth = results.Average(x => x.TrueProbability);
            var averageFalse = results.Average(x => x.FalseProbability);
            //As num samples => infinity x.TrueProbability => .5 and False probablity => .5.

        }




        public static void ProbablitiesTwoVarTest()
        {
            int numTests = 1000;
            var results = new List<MultiBooleanProbabilitySample>();
            for (var i = 0; i < numTests; i++)
            {
                int numSamples = 10000;
                int variables = 2;

                var variableSamples = Enumerable
                    .Range(0, variables)
                    .Select(var => Enumerable.Range(0, numSamples).Select(x => StaticRandom.NextBool()).ToList()).ToList();


                int anyTrueCount = 0;
                int anyFalseCount = 0;
                int allTrueCount = 0;
                int allFalseCount = 0;

                for (var sample = 0; sample < numSamples; sample++)
                {
                    bool anyTrue = false;
                    bool anyFalse = false;
                    bool allTrue = true;
                    bool allFalse = true;
                    for (var s = 0; s < variableSamples.Count; s++)
                    {
                        anyTrue = anyTrue | variableSamples[s][sample];
                        anyFalse = anyFalse | variableSamples[s][sample] == false;
                        allTrue = allTrue & variableSamples[s][sample];
                        allFalse = allFalse & variableSamples[s][sample] == false;
                    }

                    if (anyTrue) anyTrueCount++;
                    if (anyFalse) anyFalseCount++;
                    if (allTrue) allTrueCount++;
                    if (allFalse) allFalseCount++;
                }




                results.Add(new MultiBooleanProbabilitySample(numSamples, anyTrueCount, anyFalseCount, allTrueCount, allFalseCount));
            }
            var averageAnyTrue = results.Average(x => x.AnyTrueProbability);
            var averageAnyFalse = results.Average(x => x.AnyFalseProbability);

            var averageAllTrue = results.Average(x => x.AllTrueProbability);
            var averageAllFalse = results.Average(x => x.AllFalseProbability);
            //With one variables variable, As num samples => infinity TrueProbability => .5 and False probablity => .5.

            //With two variable, As num samples => infinity AnyTrueProbability => .75 and AnyFalse probablity => .75.
            //With two variables variable, As num samples => infinity AllTrueProbability => .25 and AllFalse probablity => .75.

        }


        public static void ProbablitiesThreeVarTest()
        {
            int numTests = 1000;
            var results = new List<MultiBooleanProbabilitySample>();
            for (var i = 0; i < numTests; i++)
            {
                int numSamples = 10000;
                int variables = 3;

                var variableSamples = Enumerable
                    .Range(0, variables)
                    .Select(var => Enumerable.Range(0, numSamples).Select(x => StaticRandom.NextBool()).ToList()).ToList();


                int anyTrueCount = 0;
                int anyFalseCount = 0;
                int allTrueCount = 0;
                int allFalseCount = 0;

                for (var sample = 0; sample < numSamples; sample++)
                {
                    bool anyTrue = false;
                    bool anyFalse = false;
                    bool allTrue = true;
                    bool allFalse = true;
                    for (var s = 0; s < variableSamples.Count; s++)
                    {
                        anyTrue = anyTrue | variableSamples[s][sample];
                        anyFalse = anyFalse | variableSamples[s][sample] == false;
                        allTrue = allTrue & variableSamples[s][sample];
                        allFalse = allFalse & variableSamples[s][sample] == false;
                    }

                    if (anyTrue) anyTrueCount++;
                    if (anyFalse) anyFalseCount++;
                    if (allTrue) allTrueCount++;
                    if (allFalse) allFalseCount++;
                }




                results.Add(new MultiBooleanProbabilitySample(numSamples, anyTrueCount, anyFalseCount, allTrueCount, allFalseCount));
            }
            var averageAnyTrue = results.Average(x => x.AnyTrueProbability);
            var averageAnyFalse = results.Average(x => x.AnyFalseProbability);

            var averageAllTrue = results.Average(x => x.AllTrueProbability);
            var averageAllFalse = results.Average(x => x.AllFalseProbability);
            //With one variables variable, As num samples => infinity TrueProbability => .5 and False probablity => .5.

            //With two variable, As num samples => infinity AnyTrueProbability => .75 and AnyFalse probablity => .75.
            //With two variables variable, As num samples => infinity AllTrueProbability => .25 and AllFalse probablity => .75.


            //With three variable, As num samples => infinity AnyTrueProbability => .875 and AnyFalse probablity => .875.
            //With three variables variable, As num samples => infinity AllTrueProbability => .125 and AllFalse probablity => .125.

        }



        public static void ProbablitiesThreeVarInlineTest()
        {

            int numTests = 1000;
            var results = new List<MultiBooleanProbabilitySample>();
            int numVariables = 3;
            var allTrueMask = (1u << numVariables) - 1;
            int samplesPerTest = 10000;
            uint value = 0;
            uint rnd = 0;
            for (var i = 0; i < numTests; i++)
            {
                int anyTrueCount = 0;
                int anyFalseCount = 0;
                int allTrueCount = 0;
                int allFalseCount = 0;
                for (var sample = 0; sample < samplesPerTest; sample++)
                {
                    rnd = StaticRandom.NextUInt();
                    value = rnd & allTrueMask;

                    if (value == 0)
                    {
                        allFalseCount++;
                        anyFalseCount++;
                    }
                    else if (value == allTrueMask)
                    {
                        allTrueCount++;
                        anyTrueCount++;
                    }
                    else
                    {
                        anyTrueCount++;
                        anyFalseCount++;
                    }
                }

                results.Add(new MultiBooleanProbabilitySample(samplesPerTest, anyTrueCount, anyFalseCount, allTrueCount, allFalseCount));
            }

            var averageAnyTrue = results.Average(x => x.AnyTrueProbability);
            var averageAnyFalse = results.Average(x => x.AnyFalseProbability);

            var averageAllTrue = results.Average(x => x.AllTrueProbability);
            var averageAllFalse = results.Average(x => x.AllFalseProbability);
            //With one variables variable, As num samples => infinity TrueProbability => .5 and False probablity => .5.

            //With two variable, As num samples => infinity AnyTrueProbability => .75 and AnyFalse probablity => .75.
            //With two variables variable, As num samples => infinity AllTrueProbability => .25 and AllFalse probablity => .75.
        }



        public static ProbabilityAverageSample ProbablitiesInlineVarTest(int numVariables)
        {

            if (numVariables < 1 || numVariables > 32)
            {
                throw new ArgumentOutOfRangeException("Num variables must be between 1 and 32");
            }
            int numTests = 1000;
            var results = new List<MultiBooleanProbabilitySample>();

            var allTrueMask = (uint)((1ul << numVariables) - 1);
            int samplesPerTest = 10000;
            uint value = 0;
            uint rnd = 0;
            for (var i = 0; i < numTests; i++)
            {
                int anyTrueCount = 0;
                int anyFalseCount = 0;
                int allTrueCount = 0;
                int allFalseCount = 0;
                for (var sample = 0; sample < samplesPerTest; sample++)
                {
                    rnd = StaticRandom.NextUInt();
                    value = rnd & allTrueMask;

                    if (value == 0)
                    {
                        allFalseCount++;
                        anyFalseCount++;
                    }
                    else if (value == allTrueMask)
                    {
                        allTrueCount++;
                        anyTrueCount++;
                    }
                    else
                    {
                        anyTrueCount++;
                        anyFalseCount++;
                    }
                }

                results.Add(new MultiBooleanProbabilitySample(samplesPerTest, anyTrueCount, anyFalseCount, allTrueCount, allFalseCount));
            }

            var averageAnyTrue = results.Average(x => x.AnyTrueProbability);
            var averageAnyFalse = results.Average(x => x.AnyFalseProbability);

            var averageAllTrue = results.Average(x => x.AllTrueProbability);
            var averageAllFalse = results.Average(x => x.AllFalseProbability);

            return new ProbabilityAverageSample(numVariables, numTests, samplesPerTest, averageAnyTrue, averageAnyFalse, averageAllTrue, averageAllFalse);
            //With one variables variable, As num samples => infinity TrueProbability => .5 and False probablity => .5.

            //With two variable, As num samples => infinity AnyTrueProbability => .75 and AnyFalse probablity => .75.
            //With two variables variable, As num samples => infinity AllTrueProbability => .25 and AllFalse probablity => .75.
        }
    }
}
