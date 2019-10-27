using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace NativeOnlyStackMachine
{
    public class VirtualAssemblyBuilder
    {
        public static void BuildFromClr(string name)
        {
            var t = typeof(string);
            var tAssem = t.Assembly;
            var assembly = tAssem;
        }
    }

    public class VirtualAssembly
    {
       
    }
    public class Namespaces: INamespaces
    {

    }
}
