using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MLTestDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            GeneratePostiveIntData();
        }
        static void GeneratePostiveIntData()
        {
            var rangeOneTo10 = Enumerable.Range(1, 9).ToArray();
            var rangeOneTo20 = Enumerable.Range(1, 19).ToArray();
            var ranges = rangeOneTo10.Select(i => rangeOneTo10.Select(x => (ulong)x * (ulong)(Math.Pow(10, i))).ToArray()).ToArray();
          
            var rangeTens = rangeOneTo10.Select(x => x * 10).ToArray();
            var rangeHundreds = rangeOneTo10.Select(x => x * 100).ToArray();
            var rangeZeroTo100 = Enumerable.Range(0, 100).ToArray();
            var values = rangeZeroTo100.ToArray();
            var ulongMaxValue = ulong.MaxValue;//18,446,744,073,709,551,615
            var digitNames = "zero,one,two,three,four,five,six,seven,eight,nine".FromCsv();
            var digitsOneToNine = digitNames.Skip(1).ToArray();
            var digitValues = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var digit_tens = "ten,eleven,twelve,thrirteen,fourteen,fifteen,sixteen,seventeen,eightteen,nineteen"
                .FromCsv();
            var tensDigitNames = "ten,twenty,thrirty,fourty,fifty,sixty,seventy,eighty,ninety".FromCsv();
            var hundredDigitNames = digitsOneToNine.Select(x => $"{x} hundred").ToArray();
            var thousandDigitNames= digitsOneToNine.Select(x => $"{x} thousand").ToArray();
            var places = "hundred,thousand,million,billion,trillion,quadrillion,quintillion,sextillion,Septillion".FromCsv();
            var illionPrefixs = "m,b,tr,quadr,quint,sext,sept,oct,non,dec,undec,duodec,tredec,quattrodec,quindec,sexdec,septendec,octodec,novembdec,vigint,cent".FromCsv().Select(x => $"{x}illion").ToArray();


            var tests = new List<string>();
            //add 0-100;
            //add 100-120, then add (121,122,129),(tens[i]+ {1,2,9} or rand(3)) up two 199;
            // add (200-900, stepping 100);

            
          
        }
    }
    public static class StringExtensions
    {
        public static string[] FromCsv(this string value) => value.Split(',');
        public static T[] FromCsv<T>(this string value)
        {
            var type = typeof(T);
            switch (type.Name)
            {
                case "Int32":
                    return value.Split(',').Select(x => int.Parse(x)).Cast<T>().ToArray();
                default:
                    throw new NotImplementedException();
            }
          
        }
    }
}
