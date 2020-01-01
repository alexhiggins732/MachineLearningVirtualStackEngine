using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILProbability
{
    public class StaticRandom
    {
        public static Random rnd = new Random();
        //
        // Summary:
        //     Initializes a new instance of the System.Random class, using the specified seed
        //     value.
        //
        // Parameters:
        //   Seed:
        //     A number used to calculate a starting value for the pseudo-random number sequence.
        //     If a negative number is specified, the absolute value of the number is used.
        public static void Seed(int seed) => rnd = new Random(seed);

        //
        // Summary:
        //     Returns a non-negative random integer.
        //
        // Returns:
        //     A 32-bit signed integer that is greater than or equal to 0 and less than System.Int32.MaxValue.
        public static int Next() => rnd.Next(int.MinValue, int.MaxValue);
        //
        // Summary:
        //     Returns a random integer that is within a specified range.
        //
        // Parameters:
        //   minValue:
        //     The inclusive lower bound of the random number returned.
        //
        //   maxValue:
        //     The exclusive upper bound of the random number returned. maxValue must be greater
        //     than or equal to minValue.
        //
        // Returns:
        //     A 32-bit signed integer greater than or equal to minValue and less than maxValue;
        //     that is, the range of return values includes minValue but not maxValue. If minValue
        //     equals maxValue, minValue is returned.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     minValue is greater than maxValue.
        public static int Next(int minValue, int maxValue) => rnd.Next(minValue, maxValue);
        //
        // Summary:
        //     Returns a non-negative random integer that is less than the specified maximum.
        //
        // Parameters:
        //   maxValue:
        //     The exclusive upper bound of the random number to be generated. maxValue must
        //     be greater than or equal to 0.
        //
        // Returns:
        //     A 32-bit signed integer that is greater than or equal to 0, and less than maxValue;
        //     that is, the range of return values ordinarily includes 0 but not maxValue. However,
        //     if maxValue equals 0, maxValue is returned.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     maxValue is less than 0.
        public static int Next(int maxValue) => rnd.Next(maxValue);
        //
        // Summary:
        //     Fills the elements of a specified array of bytes with random numbers.
        //
        // Parameters:
        //   buffer:
        //     An array of bytes to contain random numbers.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     buffer is null.
        public static void NextBytes(byte[] buffer) => rnd.NextBytes(buffer);
        //
        // Summary:
        //     Returns a random floating-point number that is greater than or equal to 0.0,
        //     and less than 1.0.
        //
        // Returns:
        //     A double-precision floating point number that is greater than or equal to 0.0,
        //     and less than 1.0.
        public static double NextDouble() => rnd.NextDouble();

        public static uint NextUInt() => (uint)Next();
        //
        // Summary:
        //     Returns a random floating-point number between 0.0 and 1.0.
        //
        // Returns:
        //     A double-precision floating point number that is greater than or equal to 0.0,
        //     and less than 1.0.
        protected static double Sample() => rnd.NextDouble();

        private static uint _boolBits;
        private static int _boolMask = 32;
        public static bool NextBool()
        {
            _boolMask -= 1;
            if (_boolMask == 0) { _boolMask = 32; _boolBits = (uint)~Next(); }
            return (_boolBits & 1) == 0;
        }

    }
}
