using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIR
{
    class SetEnumerator
    {
        public SetEnumerator(int range, dynamic[] symbols)
        {
            var startEnumerator = symbols.Take(0).GetEnumerator();
            var endEnumerator = symbols.Take(0).GetEnumerator();
            var digits = Enumerable.Range(0, range+2);
            var max = range + 1;
            var enumerators = digits.Select(i => i == 0 || i == max ? symbols.Take(0).GetEnumerator() : symbols.Take(symbols.Length).GetEnumerator());

        }
    }
}
