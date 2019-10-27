using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IlVmModel
{
    public class IlVmEngineModel
    {

        public static void Execute(byte[] instructions, IlVmMeta metaModel)
        {
            var pos = -1;
            var max = instructions.Length - 2;
            var strings = metaModel.strings;
            var ints = metaModel.ints;
            var locals = metaModel.locals;
            var args = metaModel.args;
            var methods = metaModel.methods;
            int opcode;
            int oparg32;
            long oparg64;
            var stack = new Stack<object>();

            READINSTRUCTION:

            opcode = instructions[pos++];
            opcode <<= 1;
            opcode += instructions[pos++];

            switch (opcode)
            {
                case 0x00: //NOOP
                    goto NEXTINSTRUCTION;
                case 0x01: //Break
                    System.Diagnostics.Debugger.Break();
                    goto NEXTINSTRUCTION;
                case 0x02: //Ldarg_0 = 0x02,
                    stack.Push(metaModel.args[0]);
                    goto NEXTINSTRUCTION;
                case 0x03: //Ldarg_1 = 0x03,
                    stack.Push(metaModel.args[1]);
                    goto NEXTINSTRUCTION;
                case 0x04: //Ldarg_2 = 0x04,
                    stack.Push(metaModel.args[2]);
                    goto NEXTINSTRUCTION;
                case 0x05:
                    stack.Push(metaModel.args[2]);
                    goto NEXTINSTRUCTION;
                case 0x06: //Ldarg_3 = 0x04,
                    stack.Push(metaModel.args[3]);
                    goto NEXTINSTRUCTION;


                /// <summary>
                /// Load arg at index s (short) onto the stack
                /// </summary>
                case 0x0e: //     Ldarg_S = 0x0e,
                    oparg32 = instructions[pos++];
                    stack.Push(metaModel.args[oparg32]);
                    goto NEXTINSTRUCTION;

                /// <summary>
                /// Load arg address s (short) onto the stack
                /// </summary>
                case 0x0f: //  Ldarga_S = 0x0f,
                    oparg32 = instructions[pos++];
                    stack.Push(metaModel.args[oparg32]);
                    goto NEXTINSTRUCTION;

                /// <summary>
                /// Stor arg at index s (short) onto the stack
                /// </summary>
                case 0x10: // Starg_S = 0x10,
                    oparg32 = instructions[pos++];
                    metaModel.args[oparg32] = stack.Pop();
                    goto NEXTINSTRUCTION;

                /// <summary>
                /// Load local at index s (short) onto the stack
                /// </summary>
                case 0x11:// Ldloc_S = 0x11,
                    oparg32 = instructions[pos++];
                    stack.Push(metaModel.locals[oparg32]);
                    goto NEXTINSTRUCTION;

                /// <summary>
                /// Load local address s (short) onto the stack
                /// </summary>
                case 0x12: // Ldloca_S = 0x12,
                    oparg32 = instructions[pos++];
                    stack.Push(metaModel.locals[oparg32]);
                    goto NEXTINSTRUCTION;

                /// <summary>
                /// Pop value off stack and store at local address s (short)
                /// </summary>
                case 0x13: // Stloc_S = 0x13,
                    oparg32 = instructions[pos++];
                    metaModel.locals[oparg32] = stack.Pop();
                    goto NEXTINSTRUCTION;

                case 0xfe09: //   Ldarg = 0xfe09,
                    oparg32 = instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    stack.Push(metaModel.args[oparg32]);
                    goto NEXTINSTRUCTION;
                case 0xfe0a: // Ldarga = 0xfe0a,
                    oparg32 = instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    stack.Push(metaModel.args[oparg32]);
                    goto NEXTINSTRUCTION;
                case 0xfe0b: // Starg = 0xfe0b,
                    oparg32 = instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    metaModel.args[oparg32] = stack.Pop();
                    goto NEXTINSTRUCTION;
                case 0xfe0c: //Ldloc = 0xfe0c,
                    oparg32 = instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    stack.Push(metaModel.locals[oparg32]);
                    goto NEXTINSTRUCTION;
                case 0xfe0d: //Ldloca = 0xfe0d,
                    oparg32 = instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    stack.Push(metaModel.locals[oparg32]);
                    goto NEXTINSTRUCTION;
                case 0xfe0e: // Stloc = 0xfe0e,
                    oparg32 = instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    oparg32 <<= 8;
                    oparg32 += instructions[pos++];
                    metaModel.locals[oparg32] = stack.Pop();
                    goto NEXTINSTRUCTION;
                default: throw new NotImplementedException();
            }

            NEXTINSTRUCTION:
            pos++;
            if (pos < max) goto READINSTRUCTION;

        }


    }
}

