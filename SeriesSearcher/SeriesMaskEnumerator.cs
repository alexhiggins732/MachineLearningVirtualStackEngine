using System.Collections;
using System.Collections.Generic;

namespace SeriesSearcher
{
   
    public class SeriesMaskEnumerator : IEnumerator<bool[]>
    {
        private int requiredBits;
        private int maxBits;
        private int maxValue;
        private int powMaxValue;
        private int currentMask;
        private int initialMask;
        private int Length;
        private int popCount;
        public SeriesMaskEnumerator(int length, int depth)
        {

            this.Length = this.requiredBits = length; //seriesIterator.BaseSeries.Length;
            this.powMaxValue = powMaxValue = (1 << requiredBits) - 1;
            this.initialMask = this.currentMask = powMaxValue - 1;

            this.maxBits = length + depth; // seriesIterator.BaseSeries.Length + seriesIterator.Depth;
            this.maxValue = (1 << (maxBits)) - 1;
        }


        public bool[] Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            //
        }

        public bool MoveNext()
        {
            bool result = false;

            if (currentMask < maxValue)
            {
                if (currentMask == powMaxValue)
                {
                    Length++;
                    powMaxValue = (1 << Length) - 1;
                    this.currentMask = initialMask;
                }
                while (++currentMask <= powMaxValue && !(result = PopCount(currentMask) == requiredBits)) { }
                if (currentMask > powMaxValue && currentMask < maxValue)
                {
                    Length++;
                    powMaxValue = (1 << Length) - 1;
                    this.currentMask = initialMask;
                    return MoveNext();
                }
                if (result)
                {
                    Current = GetBoolMask();
                }
            }
            return result;
        }

        private bool[] GetBoolMask()
        {
            var l = new List<bool>();
            var currentValue = currentMask;
            for (var i = 0; i < Length; i++, currentValue >>= 1)
            {
                l.Add(1 == (currentValue & 1));
            }
            return l.ToArray();
        }

        private int PopCount(int currentValue)
        {

            popCount = 0;
            this.currentMask = currentValue;
            for (var i = 0; currentValue > 0 && i < 32; i++, currentValue >>= 1)
            {
                if (1 == (currentValue & 1))
                {
                    popCount++;
                }
            }
            return popCount;
        }

        public void Reset()
        {
            string bp = "WTF";
        }
    }
}
