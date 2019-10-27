using System.Text;
using System.Threading.Tasks;
namespace ILEngine
{
    class Program
    {
        static void Main(string[] args)
        {

            var stackBehaviorSwitch = EnumSwitchGenerator.GenerateCsEnumSwitch<System.Reflection.Emit.StackBehaviour>();


      


            IlVmModel.DefaultVmBuilder.BuildVm();
            var gen = new ILOpcodeInterperterSwitchActionGenerator();
            gen.HandleOpcode(0);
            var genswitch = gen.GeneratorSwitchStatement();

            var vmSwitchStatement = ILOpcodeActionCodeBuilder.GetILOpcodeInterfaceCode();

   

        }
    }
}
