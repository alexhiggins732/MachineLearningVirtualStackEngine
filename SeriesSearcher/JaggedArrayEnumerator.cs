using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeriesSearcher.AlphabetHelper;
namespace SeriesSearcher
{
    public class AlphabetHelper
    {

        public const string AlphaLowerString = "abcdefghijklmnopqrstuvwxyz";
        public const string AlphaUpperString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string DigitsString = "0123456789";
        public const string DigitsSpecialString = ")!@#$%^&*(";
        public const string HexCharsLowerString = "abcdef";
        public const string HexCharsUpperString = "ABCDEF";
        public const string HexLowerString = DigitsString + HexCharsLowerString;
        public const string HexUpperString = DigitsString + HexCharsUpperString;
        public const string AlphaNumericLowerString = AlphaLowerString + DigitsString;
        public const string AlphaNumericUpperString = AlphaUpperString + DigitsString;
        public const string AlphaNumericString = AlphaLowerString + AlphaUpperString + DigitsString;
        public const string AlphaNumericSpecialString = AlphaLowerString + AlphaUpperString + DigitsString + DigitsSpecialString;
        public static char[] LoweredAlpha { get; } = AlphaLowerString.ToCharArray();
        public static char[] UpperAlpha { get; } = AlphaUpperString.ToCharArray();
        public static char[] Digits { get; } = DigitsString.ToCharArray();
        public static char[] HexCharsLower { get; } = HexCharsLowerString.ToCharArray();
        public static char[] HexCharsUpper { get; } = HexCharsUpperString.ToCharArray();
        public static char[] HexUpper { get; } = HexUpperString.ToCharArray();
        public static char[] HexLower { get; } = HexLowerString.ToCharArray();

        public static char[] AlphaNumericLower { get; } = AlphaNumericLowerString.ToCharArray();
        public static char[] AlphaNumericUpper { get; } = AlphaNumericUpperString.ToCharArray();
        public static char[] AlphaNumeric { get; } = AlphaNumericString.ToCharArray();
        public static char[] AlphaNumericSpecial { get; } = AlphaNumericSpecialString.ToCharArray();

        public static Dictionary<char, char[]> AlphaFractalBases { get; } = LoweredAlpha.ToDictionary(x => x, x => GetCharFractals(x));
        public static Dictionary<char, char[]> DigitFractalBases { get; } = Digits.ToDictionary(x => x, x => GetDigitFractals(x));

        public static char[] DigitFractals { get; } = Digits.SelectMany(c => GetDigitFractals(c)).ToArray();
        public static char[] GetCharFractals(char x)
        {
            var result = new List<char> { x, char.ToUpper(x) };
            switch (x)
            {
                case 'a':
                    result.Add('2');
                    result.Add('@');
                    break;
                case 'e':
                    result.Add('3');
                    result.Add('#');
                    break;
                case 'i':
                    result.Add('1');
                    result.Add('!');
                    break;
                case 'o':
                    result.Add('0');
                    result.Add(')');
                    break;
            }
            return result.ToArray();
        }
        public static char[] GetDigitFractals(char x)
        {
            char[] result = new[] { x, x };
            switch (x)
            {
                case '0':
                    result[0] = ')';
                    break;
                case '1':
                    result[0] = '!';
                    break;
                case '2':
                    result[0] = '@';
                    break;
                case '3':
                    result[0] = '#';
                    break;
                case '4':
                    result[0] = '$';
                    break;
                case '5':
                    result[0] = '%';
                    break;
                case '6':
                    result[0] = '^';
                    break;
                case '7':
                    result[0] = '&';
                    break;
                case '8':
                    result[0] = '*';
                    break;
                case '9':
                    result[0] = '(';
                    break;
                default:
                    throw new ArgumentException("Argument must be in range [0:9]");
            }
            return result;
        }

    }
    public class AlphaFractals
    {
        public static void RunTests()
        {
            RunWordListFractalsWithMaskedFractalsTest();
        }
        public static void RunWordListFractalsTest()
        {
            var roots = new[] { "password", "secure", };
            var wordFractals = GetWordListFractals(roots);
            foreach (var fractal in wordFractals)
            {
                Console.WriteLine(new string(fractal));
            }
        }
        public static void RunWordListFractalsWithNumericSuffixTest()
        {
            var roots = new[] { "password", "secure", };
            var suffixList = Digits;
            var wordFractals = GetWordListFractalsWithSuffix(roots, suffixList, 4);
            foreach (var fractal in wordFractals)
            {
                Console.WriteLine(new string(fractal));
            }
        }

        public static void RunWordListFractalsWithNumericSuffixFractalsTest()
        {
            var roots = new[] { "password", "secure", };
            var suffixList = DigitFractals;
            var wordFractals = GetWordListFractalsWithSuffix(roots, suffixList, 4);
            foreach (var fractal in wordFractals)
            {
                Console.WriteLine(new string(fractal));
            }
        }
        public static void RunWordListFractalsWithNumericPrefixFractalsTest()
        {
            var roots = new[] { "password", "secure", };
            var prefixList = DigitFractals;
            var wordFractals = GetWordListFractalsWithPrefix(roots, prefixList, 4);
            foreach (var fractal in wordFractals)
            {
                Console.WriteLine(new string(fractal));
            }
        }

        public static void RunWordListFractalsWithMaskedFractalsTest()
        {
            var roots = new[] { "password", "secure", };
            var fractalList = DigitFractals;
            var wordFractals = GetWordListFractalsWithMaskedFractals(roots, fractalList, 4);
            ulong count = 0;
            int increment = 0;
            int showMask = 1 << 26;
            var sw = Stopwatch.StartNew();
            foreach (var fractal in wordFractals)
            {
                //Console.WriteLine(new string(fractal));
                count++;
                increment++;
                if (increment == showMask)
                {
                    Console.WriteLine($"{count}: {new string(fractal)} - {sw.Elapsed}");
                    increment = 0;
                }
            }
        }



        public static IEnumerable<char[]> GetWordListFractals(IEnumerable<string> wordList)
        {
            var fractalbases = AlphaFractalBases;
            foreach (var word in wordList)
            {
                var wordPermuations = word.Select(x => fractalbases[x]).ToList();
                var jaggedSingle = new EnumerableJaggedArray<char>(wordPermuations.ToArray());
                foreach (var perm in jaggedSingle)
                {
                    yield return perm;
                }
            }
        }

        public static IEnumerable<char[]> GetWordListFractalsWithSuffix(IEnumerable<string> wordList, char[] suffixList, int maxSuffixLength)
        {
            var fractalbases = AlphaFractalBases;
            foreach (var word in wordList)
            {
                var wordPermuations = word.Select(x => fractalbases[x]).ToList();
                for (var i = 0; i < maxSuffixLength; i++)
                {
                    var copy = wordPermuations.ToList();
                    for (var k = 0; k < i; k++)
                    {
                        copy.Add(suffixList);
                    }
                    var jaggedSingle = new EnumerableJaggedArray<char>(copy.ToArray());
                    foreach (var perm in jaggedSingle)
                    {
                        yield return perm;
                    }
                }
            }
        }

        public static IEnumerable<char[]> GetWordListFractalsWithPrefix(IEnumerable<string> wordList, char[] prefixList, int maxSuffixLength)
        {
            var fractalbases = AlphaFractalBases;
            foreach (var word in wordList)
            {
                var wordPermuations = word.Select(x => fractalbases[x]).ToList();
                for (var i = 0; i < maxSuffixLength; i++)
                {
                    var copy = wordPermuations.ToList();
                    for (var k = 0; k < i; k++)
                    {
                        copy.Insert(0, prefixList);
                    }
                    var jaggedSingle = new EnumerableJaggedArray<char>(copy.ToArray());
                    foreach (var perm in jaggedSingle)
                    {
                        yield return perm;
                    }
                }
            }
        }

        public static IEnumerable<char[]> GetWordListFractalsWithMaskedFractals(IEnumerable<string> wordList, char[] fractalList, int maxDepth)
        {
            var fractalbases = AlphaFractalBases;
            foreach (var word in wordList)
            {
                var wordPermuations = word.Select(x => fractalbases[x]).ToList();

                var jaggedSingle = new EnumerableJaggedArray<char>(wordPermuations.ToArray());
                foreach (var jagged in jaggedSingle)
                {
                    var seriesIterator = new SeriesIteratorOfChar(jagged, fractalList, maxDepth);
                    foreach (var iter in seriesIterator)
                    {
                        yield return iter;
                    }
                }


            }
        }

    }
    public class EnumerableJaggedArrayTests
    {
        public static void RunTests()
        {
            var decimalDigits = "0123456789";
            var hexDigits = decimalDigits + "abcdef";
            var digits = decimalDigits.ToArray();
            var hex = hexDigits.ToArray();
            var t1 = "123".ToCharArray();
            var t2 = "12".ToCharArray();
            var jagged = new EnumerableJaggedArray<char>(hex, hex);

            foreach (var item in jagged)
            {
                var s = new string(item);
                Console.WriteLine(s);
            }



            var jagged8 = new EnumerableJaggedArray<char>(hex, 8);


            var sw = Stopwatch.StartNew();
            //var jagged8Count = jagged8.Count();
            //Console.WriteLine($"jagged8Count {jagged8Count} in {sw.Elapsed}");

            sw = Stopwatch.StartNew();
            uint counted = 0;
            int phase = 0;
            int phaseMax = 1 << 28;
            foreach (var item in jagged8)
            {
                counted++;
                //phase++;
                //if (phase == phaseMax)
                //{
                //    phase = 0;
                //    Console.WriteLine($"Counted {counted} in {sw.Elapsed}");
                //}
            }
            Console.WriteLine($"Counted {counted} in {sw.Elapsed}");
        }

        public static void RunAlphaNumericTests()
        {
            var decimalDigits = "0123456789";
            var decimals = decimalDigits.ToCharArray();
            var alphaNumeric = new List<char>();
            for (var c = 'A'; c <= 'Z'; c++)
            {
                alphaNumeric.Add(c);
            }
            for (var c = 'a'; c <= 'z'; c++)
            {
                alphaNumeric.Add(c);
            }
            alphaNumeric.AddRange(decimals);

            for (var depth = 4; depth <= 8; depth++)
            {
                var sw = Stopwatch.StartNew();
                ulong counted = 0;
                var jagged = new EnumerableJaggedArray<char>(alphaNumeric, depth);
                foreach (var item in jagged)
                {
                    counted++;
                }
                Console.WriteLine($"Depth {depth}: Counted {counted} in {sw.Elapsed}");
            }


        }
    }
    public class EnumerableJaggedArray<T> : IEnumerable<T[]>
    {
        public T[][] JaggedArrays { get; private set; }
        public EnumerableJaggedArray(params IEnumerable<T>[] enumerables)
        {
            this.JaggedArrays = enumerables.Select(x => x.ToArray()).ToArray();
        }

        public EnumerableJaggedArray(IEnumerable<T> enumerable, int count)
        {
            var baseArray = enumerable.ToArray();
            this.JaggedArrays = Enumerable.Range(0, count).Select(i => (T[])baseArray.Clone()).ToArray();
        }

        public IEnumerator<T[]> GetEnumerator()
        {
            return new JaggedArrayEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
    public class JaggedArrayEnumerator<T> : IEnumerator<T[]>
    {
        private T[][] arrays;
        private int[] indexes;
        private int[] lengths;
        public JaggedArrayEnumerator(EnumerableJaggedArray<T> enumerableJaggedArray)
        {
            this.arrays = enumerableJaggedArray.JaggedArrays.Select(x => x.ToArray()).ToArray();
            lengths = arrays.Select(x => x.Length).ToArray();
            indexes = lengths.Select(x => 0).ToArray();
            indexes[indexes.Length - 1] = -1;
            Current = arrays.Select(x => x.First()).ToArray();
        }

        public T[] Current { get; private set; }

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            arrays = null;
            indexes = lengths = null;
        }

        public bool MoveNext()
        {

            bool result = false;
            for (var k = indexes.Length - 1; k > -1 && !result; k--)
            {
                indexes[k]++;
                result = indexes[k] < lengths[k];
                if (!result)
                {
                    indexes[k] = 0;
                }
                Current[k] = arrays[k][indexes[k]];
            }
            return result;

        }

        public void Reset()
        {
            int i = 0;
            {
                indexes[i] = 0;
                Current[i] = arrays[i][0];
            }
            indexes[i] = -1;
        }
    }

    public class MaskedJaggedArray<T>
    {

    }

    public static class ArrayExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> array, Action<T> action)
        {
            foreach (T element in array) action(element);
        }
    }
}
