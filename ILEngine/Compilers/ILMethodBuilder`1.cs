using ILEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.Compilers
{
    public class ILMethodBuilder<T>
    {
        public static ILMethod Create(string methodName) => new ILMethod(methodName, typeof(T));
    }
}
