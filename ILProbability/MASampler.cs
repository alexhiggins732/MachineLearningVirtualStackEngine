using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILProbability
{
    public class MASamplerTests
    {
        public static void RunTests()
        {
            RunMtfSmaTests();

        }

        private static void RunMtfSmaTests()
        {

            int maxMa = 256;
            var mtf = new MtfSmaIntDecimal(maxMa);

            var vectorSampler = new VectorSampler<decimal>(256, maxMa);

            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 65536; i++)
            {
                var rnd = StaticRandom.Next();
                mtf.AddSample(rnd);
                vectorSampler.AddSample(mtf.Averages);
                //var avgs = string.Join(",", mtf.Averages);
                //Console.WriteLine($"{i}: {avgs}");
            }
            sw.Stop();
            Console.WriteLine($"Sampled {vectorSampler.TotalSamples} of [{vectorSampler.HistorySize}][{vectorSampler.SampleLength}] in {sw.Elapsed}");
        }

        private static void RunTimedTests()
        {
            var timed = new Sma<int, decimal, IntDecimalProvider>(256);
            var timedWatch = Stopwatch.StartNew();
            for (var i = 0; i < 100000; i++)
            {
                var rnd = StaticRandom.Next();
                timed.AddSample(rnd);
                var avg = timed.Average;
            }
            timedWatch.Stop();
            Console.WriteLine($"Timed: {timedWatch.Elapsed}");

            var timedControl = new List<Decimal>();
            var timedControlWatch = Stopwatch.StartNew();
            for (var i = 0; i < 100000; i++)
            {
                var rnd = StaticRandom.Next();
                timedControl.Add(rnd);
                if (timedControl.Count > 256)
                {
                    timedControl.RemoveAt(0);
                }
                var avg = timedControl.Average();
            }
            timedControlWatch.Stop();
            Console.WriteLine($"Timed Control: {timedControlWatch.Elapsed}");

            var factor = timedControlWatch.ElapsedMilliseconds / (double)timedWatch.ElapsedMilliseconds;
            Console.WriteLine($"Factor: {factor}");
        }

        private static void RunAssertAverages()
        {
            var ma10 = new Sma<int, decimal, IntDecimalProvider>(10);
            for (var i = 0; i < 100; i++)
            {
                var sample = 1;
                ma10.AddSample(sample);
                Console.WriteLine($"{i}: {ma10.Average}");
                if (ma10.Count == 10)
                {
                    System.Diagnostics.Debug.Assert(ma10.Average == 1);
                }
            }

        }
        private static void RunAssertControlAverages()
        {

            var ma256 = new Sma<int, decimal, IntDecimalProvider>(256);
            var ma256Control = new List<Decimal>();


            for (var i = 0; i < 1000; i++)
            {
                var rnd = StaticRandom.Next();
                ma256Control.Add(rnd);
                ma256.AddSample(rnd);
                Console.WriteLine($"{i}", ma256.Average);
                if (ma256Control.Count > 256)
                {
                    ma256Control.RemoveAt(0);
                    var avg = ma256Control.Average();
                    System.Diagnostics.Debug.Assert(avg == ma256.Average);
                }
            }

        }
    }

    public class VectorSampler<T>
    {
        public T[][] History;
        public int HistorySize { get; private set; }
        public int SampleLength { get; private set; }
        public int TotalSamples;
        public VectorSampler(int historySize, int length)
        {

            this.History = new T[HistorySize = historySize][];
            this.SampleLength = length;
            for (var i = 0; i < historySize; i++)
            {
                this.History[i] = new T[SampleLength];
            }
        }

        public void AddSample(T[] sample)
        {
            for (var i = 0; i < HistorySize - 1; i++)
            {
                Array.Copy(History[i], History[i + 1], SampleLength);
            }
            Array.Copy(sample, History[0], SampleLength);
            TotalSamples++;
        }

    }
    
    public class IntDecimalProvider : IFloatProvider<int, decimal>
    {


        public decimal Divide(int dividend, int divisor)
        {
            return (decimal)dividend / divisor;
        }
        public decimal Divide(decimal dividend, int divisor)
        {
            return dividend / divisor;
        }

        public decimal Divide(int dividend, decimal divisor)
        {
            return dividend / divisor;
        }
        public decimal Divide(decimal dividend, decimal divisor)
        {
            return dividend / divisor;
        }

        public decimal Multiply(int multiplicand, int multiplier)
        {
            return (decimal)multiplicand * multiplier;
        }
        public decimal Multiply(decimal multiplicand, int multiplier)
        {
            return multiplicand * multiplier;
        }

        public decimal Multiply(int multiplicand, decimal multiplier)
        {
            return multiplicand * multiplier;
        }
        public decimal Multiply(decimal multiplicand, decimal multiplier)
        {
            return multiplicand * multiplier;
        }

        public decimal Add(decimal a, decimal b) => a + b;
        public decimal Substract(decimal a, decimal b) => a - b;

        public int ToNumeric(object value) => Convert.ToInt32(value);
        public Decimal ToFloat(object value) => Convert.ToDecimal(value);


        public int ToNumeric(string value) => int.Parse(value);
        public decimal ToFloat(string value) => decimal.Parse(value);
    }

    public interface IFloatProvider<TNumeric, TFloat>
    {
        //TFloat Divide<Tconvertable>(Tconvertable dividend, Tconvertable divisor);
        //TFloat Divide(TNumeric dividend, TNumeric divisor);

        //TFloat Divide(TNumeric dividend, TFloat divisor);
        //TFloat Divide(TFloat dividend, TNumeric divisor);


        //TFloat Multiply(TNumeric multiplicand, TNumeric multiplier);
        //TFloat Multiply(TNumeric multiplicand, TFloat multiplier);
        //TFloat Multiply(TFloat multiplicand, TNumeric multiplier);

        TFloat Divide(TFloat dividend, TFloat divisor);
        TFloat Multiply(TFloat multiplicand, TFloat multiplier);

        TFloat Add(TFloat a, TFloat b);
        TFloat Substract(TFloat a, TFloat b);

        TFloat ToFloat(object value);
        TNumeric ToNumeric(object value);
        TFloat ToFloat(string value);
        TNumeric ToNumeric(string value);
    }


    public class MtfSmaIntDecimal : MtfSma<int, decimal, IntDecimalProvider>
    {
        public MtfSmaIntDecimal(int smaMax) : base(smaMax)
        {
        }
    }
    public class MtfSma<TNumeric, TFloat, TFloatProvider>
        where TFloatProvider : IFloatProvider<TNumeric, TFloat>, new()
    {
        public Dictionary<int, ISma<TNumeric, TFloat>> SMAs;
        private Dictionary<int, int> indexMap;
        private List<Action<TNumeric>> SampleActions;
        public TFloat[] Averages;
        public int TotalSamples = 0;
        public MtfSma(int smaMax) : this(Enumerable.Range(1, smaMax))
        {
            //SMAs = new Dictionary<int, ISMA<TNumeric, TFloat>>();
            //this.SampleActions = new List<Action<TNumeric>>();
            //Averages = new TFloat[smaMax];
            //for (int i = 0, smaSize = 1; i < smaMax; i++, smaSize++)
            //{
            //    var sma = new SMA<TNumeric, TFloat, TFloatProvider>(smaSize);
            //    SampleActions.Add((x) => { sma.AddSample(x); Averages[indexMap[sma.MaSize]] = sma.Average; });
            //    SMAs.Add(smaSize, sma);
            //    indexMap.Add(smaSize, i);

            //}
        }

        public MtfSma(IEnumerable<int> maSizes)
        {
            SMAs = new Dictionary<int, ISma<TNumeric, TFloat>>();
            this.SampleActions = new List<Action<TNumeric>>();
            this.indexMap = new Dictionary<int, int>();
            //int smaCount = msaSizes.Count;
            //Averages = new TFloat[smaMax];
            var averages = new List<TFloat>();
            int i = 0;
            foreach (var smaSize in maSizes)
            {
                var sma = new Sma<TNumeric, TFloat, TFloatProvider>(smaSize);
                SampleActions.Add((x) => { sma.AddSample(x); Averages[indexMap[sma.MaSize]] = sma.Average; });
                SMAs.Add(smaSize, sma);
                indexMap.Add(smaSize, i++);
                averages.Add(sma.Average);
            }
            this.Averages = averages.ToArray();
        }

        public void AddSample(TNumeric value)
        {

            SampleActions.ForEach(action => action(value));
            TotalSamples++;
        }
    }

    public interface ISma<TNumeric, TFloat>
    {
        int Count { get; }
        void AddSample(TNumeric rnd);
        TFloat Average { get; }
    }
    public class Sma<TNumeric, TFloat, TFloatProvider> : ISma<TNumeric, TFloat>
        where TFloatProvider : IFloatProvider<TNumeric, TFloat>, new()
    {
        public int MaSize;
        public TFloat sampleRatio;
        private TFloatProvider provider;
        public TFloat Average { get; private set; }
        private List<TFloat> history;
        public int Count => history.Count;
        public Sma(int maSize)
        {
            this.MaSize = maSize;
            this.provider = new TFloatProvider();
            this.sampleRatio = provider.Divide(provider.ToFloat(1), provider.ToFloat(MaSize));
            this.history = new List<TFloat>();
        }

        public void AddSample(TNumeric rnd)
        {
            TFloat sampleAsFloat = provider.ToFloat(rnd);
            TFloat sampleValue = provider.Multiply(sampleAsFloat, sampleRatio);
            if (history.Count == 0)
            {
                history.Add(sampleValue);
                Average = sampleValue;
            }
            else if (history.Count < MaSize)
            {
                history.Add(sampleValue);
                Average = provider.Add(Average, sampleValue);
            }
            else
            {
                Average = provider.Substract(Average, history[0]);
                if (history.Count == MaSize)
                    history.RemoveAt(0);

                history.Add(sampleValue);
                Average = provider.Add(Average, sampleValue);
            }

        }
    }
}
