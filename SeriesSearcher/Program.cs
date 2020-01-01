using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesSearcher
{
    class Program
    {
        static void Main(string[] args)
        {
            //SeriesIteratorTests.RunTests();

            AlphaFractals.RunTests();
        }
    }
    public class SeriesIteratorTests
    {
        public static void RunTests()
        {
        }
        public static void RunFractalTests()
        {
            var roots = new[] { "password", "secure" };

        }
        public static void RunBaseDigitTests()
        {
            var root = "aaaaaaaa".ToCharArray();
            var iSet = "0123456789".ToCharArray();


            var it = new SeriesIteratorOfChar(root, iSet, 4);
            var sw = Stopwatch.StartNew();
            int lastDepth = 0;
            ulong count = 0;
            foreach (var item in it)
            {
                count++;
                if (item.Length != lastDepth)
                {
                    lastDepth = item.Length;
                    Console.WriteLine($"Depth {lastDepth}: {count} items in {sw.Elapsed}");
                    sw = Stopwatch.StartNew();
                }
            }
        }

    }

    public class SeriesIteratorOfChar : SeriesIterator<char>
    {
        public SeriesIteratorOfChar(char[] baseSeries, char[] iterationSet, int depth) : base(baseSeries, iterationSet, depth)
        {

        }
    }
    public class SeriesIterator<T> : IEnumerable<T[]>
    {
        public T[] BaseSeries;
        public T[] IterationSet;

        public int Depth;



        public SeriesIterator(T[] baseSeries, T[] iterationSet, int depth)
        {
            this.BaseSeries = baseSeries;
            this.IterationSet = iterationSet;
            this.Depth = depth;
        }



        public IEnumerator<T[]> GetEnumerator()
        {
            return new SeriesEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    public class SeriesEnumerator<T> : IEnumerator<T[]>
    {

        private SeriesMaskEnumerator maskEnumerator;
        EnumerableJaggedArray<T> jaggedArray;
        private IEnumerator<T[]> jaggedArrayEnumerator;
        private T[] BaseSeries;
        private T[] IterationSet;
        private T[][] BaseSeriesElements;
        private T[][] jaggedElements;
        public T[] Current => jaggedArrayEnumerator.Current;

        object IEnumerator.Current => Current;

        public SeriesEnumerator(SeriesIterator<T> seriesIterator)
        {
            this.BaseSeries = seriesIterator.BaseSeries;
            this.IterationSet = seriesIterator.IterationSet;

            this.maskEnumerator = new SeriesMaskEnumerator(seriesIterator.BaseSeries.Length, seriesIterator.Depth);
            maskEnumerator.MoveNext();
            BaseSeriesElements = BaseSeries.Select(x => new[] { x }).ToArray();
            jaggedElements = (T[][])BaseSeriesElements.Clone();
            this.jaggedArray = new EnumerableJaggedArray<T>(jaggedElements);
            this.jaggedArrayEnumerator = jaggedArray.GetEnumerator();

        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        public bool MoveNext()
        {

            bool result = jaggedArrayEnumerator.MoveNext();
            if (!result)
            {
                result = maskEnumerator.MoveNext();
                if (result)
                {
                    if (jaggedElements.Length != maskEnumerator.Current.Length)
                    {
                        jaggedElements = maskEnumerator.Current.Select(x => new T[] { }).ToArray();
                    }
                    //var jaggedElements = new List<T[]>();
                    int baseIndex = 0;
                    for (var i = 0; i < maskEnumerator.Current.Length; i++)
                    {
                        if (maskEnumerator.Current[i])
                        {
                            //jaggedElements.Add(BaseSeriesElements[baseIndex++]);
                            jaggedElements[i] = BaseSeriesElements[baseIndex++];
                        }
                        else
                        {
                            //jaggedElements.Add((T[])IterationSet.Clone());
                            jaggedElements[i] = (T[])IterationSet.Clone();
                        }
                    }
                    this.jaggedArray = new EnumerableJaggedArray<T>(jaggedElements.ToArray());
                    this.jaggedArrayEnumerator = jaggedArray.GetEnumerator();
                    result = jaggedArrayEnumerator.MoveNext();
                }

            }

            return result;
        }

        public void Reset()
        {
            //SeriesIterator.MaskEnumerator.Reset();
        }
    }

    public class EnumerableCollectionEnumeratorTest
    {
        public static void RunTests()
        {
            var chars = "0123456789".ToCharArray();

            var enumerable = new EnumerableCollectionEnumerable<char>(chars, chars, chars);
            var l = enumerable.Select(x => new string(x)).OrderBy(x => x).ToList();
            foreach (var item in enumerable)
            {
                Console.WriteLine(string.Join("", item));
            }
        }
    }
    public class EnumerableCollectionEnumerable<T> : IEnumerable<T[]>
    {
        public IEnumerator<T>[] Enumerators { get; private set; }
        public EnumerableCollectionEnumerable(params IEnumerable<T>[] enumerables)
        {
            this.Enumerators = enumerables.Select(x => x.GetEnumerator()).ToArray();
        }

        public IEnumerator<T[]> GetEnumerator()
        {
            return new EnumerableCollectionEnumerator<T>(Enumerators);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
    public class EnumerableCollectionEnumerator<T> : IEnumerator<T[]>
    {
        private IEnumerator<T>[] enumerators;
        bool initialized = false;
        public EnumerableCollectionEnumerator(IEnumerator<T>[] enumerators)
        {
            this.enumerators = enumerators;
            Current = enumerators.Select(x => default(T)).ToArray();
        }

        public T[] Current { get; }

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {

            enumerators = null;

        }

        private void setCurrent(int i)
        {
            for (var k = 0; k <= i; k++)
            {
                if (k < i) enumerators[k].Reset();
                else Current[k] = enumerators[k].Current;
            }
        }
        public bool MoveNext()
        {
            bool result = false;
            if (!initialized)
            {
                initialized = true;
                for (var i = 0; i < enumerators.Length && (result = enumerators[i].MoveNext()); i++)
                {
                    Current[i] = enumerators[i].Current;
                }
            }
            else
            {
                int i;
                for (i = 0; i < enumerators.Length && !(result = enumerators[i].MoveNext()); i++)
                {

                }
                if (result)
                    setCurrent(i);

            }
            return result;
        }

        public void Reset()
        {
            this.initialized = false;
            for (var k = 0; k < enumerators.Length; k++)
            {
                enumerators[k].Reset();
                Current[k] = default(T);
            }

        }
    }

}
