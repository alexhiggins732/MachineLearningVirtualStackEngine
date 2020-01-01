using ILEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{
    public class DefaultILInstructionGenerator
    {
        public static ILInstruction GenerateRandom(OpCodeMetaModel model)
        {
            var flowcontrol = model.FlowControl;
            var operandBitSize = model.OperandTypeBitSize;
            Enum.TryParse<StackBehaviour>(model.StackBehaviourPop, out StackBehaviour pop);

            //    /*


            //Pop0 = 0,
            //Pop1 = 1,
            //Pop1_pop1 = 2,
            //Popi = 3,
            //Popi_pop1 = 4,
            //Popi_popi = 5,
            //Popi_popi8 = 6,
            //Popi_popi_popi = 7,
            //Popi_popr4 = 8,
            //Popi_popr8 = 9,
            //Popref = 10,
            //Popref_pop1 = 11,
            //Popref_popi = 12,
            //Popref_popi_popi = 13,
            //Popref_popi_popi8 = 14,
            //Popref_popi_popr4 = 15,
            //Popref_popi_popr8 = 16,
            //Popref_popi_popref = 17
            //Varpop = 26
            //Popref_popi_pop1 = 28
            //     */
            //    switch (pop)
            //    {
            //        case StackBehaviour.Pop0: //0
            //            popCount = 0;
            //            break;
            //        case StackBehaviour.Pop1: //1
            //        case StackBehaviour.Popi: //3
            //        case StackBehaviour.Popref: //10
            //        case StackBehaviour.Varpop: //26
            //            popCount = 1;
            //            break;
            //        case StackBehaviour.Pop1_pop1: //2
            //        case StackBehaviour.Popi_pop1: //4
            //        case StackBehaviour.Popi_popi: //5
            //        case StackBehaviour.Popi_popi8: //6
            //        case StackBehaviour.Popi_popr4: //8
            //        case StackBehaviour.Popi_popr8: //9
            //        case StackBehaviour.Popref_pop1: //11;
            //        case StackBehaviour.Popref_popi: //12;
            //            popCount = 2;
            //            break;
            //        case StackBehaviour.Popi_popi_popi: //7
            //        case StackBehaviour.Popref_popi_popi: //13
            //        case StackBehaviour.Popref_popi_popi8: //14
            //        case StackBehaviour.Popref_popi_popr4: //15
            //        case StackBehaviour.Popref_popi_popr8: //16
            //        case StackBehaviour.Popref_popi_popref: //17
            //        case StackBehaviour.Popref_popi_pop1: //28
            //            popCount = 3;
            //            break;

            //    }

            return new ILInstruction();
            
            
        }
    }
}
