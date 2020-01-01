using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ILVmModel
{
    public class VmILInterpreter
    {
        private Stack<object> stack;
        private Func<bool>[] ilInstructionHandlers;
        public VmILInterpreter()
        {
            this.stack = new Stack<object>();
            ilInstructionHandlers = new Func<bool>[]
            {
                Add,BLT
            };
        }

        public void ExecuteMethodInfo(MethodInfo method)
        {
            var body = method.GetMethodBody();
            var bodyIL = body.GetILAsByteArray();
            var methodLocals = body.LocalVariables;
            var methodParameters = method.GetParameters();

            var args = new object[methodParameters.Length];
            var locals = new object[methodLocals.Count];

            var loc_0 = locals[0];
            loc_0 = 1;
            locals[0] = loc_0;
       
            var arg_0 = args[0];
            arg_0 = 1;
            args[0] = arg_0;


            
        }
        public void ExecuteFull(int[] instructions, object[] locals, object[] args)
        {
            var opcode = 0;
            var pos = -1;
            var max = instructions.Length - 1;
            bool hasjump = false;
        }
        public void Execute(int[] instructions)
        {
            var opcode = 0;
            var pos = -1;
            var max = instructions.Length - 1;
            bool hasjump = false;

            JMPTABLE:
            opcode = instructions[pos]; //ldloc.0 /*pos*/ ldloc [instructions], ldelem stloc.1 /*pos*/
            hasjump = HandleOpcode(opcode); // branch/conditional branch will push a jmptarget on the stack and jump to CUSTOMJMP;
            pos++;
            if (hasjump) pos = instructions[pos];
            if (pos < max) goto JMPTABLE;  // br JMPTABLE
        }

        public void Execute2(int[] instructions)
        {
            var opcode = 0;
            var pos = -1;
            var max = instructions.Length - 1;


            JMPTABLE:
            opcode = instructions[pos]; //ldloc.0 /*pos*/ ldloc [instructions], ldelem stloc.1 /*pos*/
            if (HandleOpcode(opcode)) // branch/conditional branch will push a jmptarget on the stack and jump to CUSTOMJMP;
                pos = instructions[++pos];
            else
                pos++;
            if (pos < max) goto JMPTABLE;  // br JMPTABLE
        }


        private bool Add()
        {
            var a = (int)stack.Pop();
            var b = (int)stack.Pop();
            stack.Push(a + b);
            return false;
        }
        private bool BLT()
        {
            var a = (int)stack.Pop();
            var b = (int)stack.Pop();
            return a < b;
        }
        private bool HandleOpcode(int opcodeIndex) => ilInstructionHandlers[opcodeIndex]();

    }
}
