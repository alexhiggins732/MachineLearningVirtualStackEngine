using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ILEngine
{
    public class IlInstructionReader
    {
        public static List<IlInstruction> FromMethod(MethodInfo method) => FromByteCode(method.GetMethodBody().GetILAsByteArray());
       
        public static List<IlInstruction> FromByteCode(byte[] byteCode)
        {
            var result = new List<IlInstruction>();
            var ms = new MemoryStream(byteCode);
            var br = new BinaryReader(ms);
            long idx = 0;
            while (ms.Position < ms.Length)
            {
                idx = ms.Position;
                byte first = br.ReadByte(); //todo; opcodes are short, need to determine  0xfe00 && idx <= 0xfe1e

                var code = OpCodeLookup.GetILOpcode(first);
                if (code == OpCodes.Prefix1)
                {
                    var second = br.ReadByte();

                    var shortVal = (short)((((ushort)first) << 8) + (second));
                    code = OpCodeLookup.GetILOpcode(shortVal);
                }
                var instruction = new IlInstruction { OpCode = code, ByteIndex = idx };
                switch (code.OperandType)
                {

                    //     The operand is a 32-bit integer branch target.
                    case OperandType.InlineBrTarget: // = 0,
                        instruction.Arg = br.ReadInt32();
                        break;
                    //     The operand is a 32-bit metadata token.
                    case OperandType.InlineField: // = 1,
                        instruction.Arg = br.ReadInt32();
                        break;
                    //     The operand is a 32-bit integer.
                    case OperandType.InlineI: // = 2,
                        instruction.Arg = br.ReadInt32();
                        break;
                    //     The operand is a 64-bit integer.
                    case OperandType.InlineI8: // = 3,
                        instruction.Arg = br.ReadInt64();
                        break;
                    //     The operand is a 32-bit metadata token.
                    case OperandType.InlineMethod: // = 4,
                        instruction.Arg = br.ReadInt32();
                        break;
                    //     No operand.
                    case OperandType.InlineNone: // = 5,
                        break;
                    //     The operand is reserved and should not be used.
                    case OperandType.InlinePhi: // = 6,
                        throw new NotImplementedException();
                        break;
                    //     The operand is a 64-bit IEEE floating point number.
                    case OperandType.InlineR: // = 7,
                        instruction.Arg = br.ReadDouble();
                        break;
                    //     The operand is a 32-bit metadata signature token.
                    case OperandType.InlineSig: // = 9,
                        instruction.Arg = br.ReadInt32();
                        break;
                    //     The operand is a 32-bit metadata string token.
                    case OperandType.InlineString: // = 10,
                        instruction.Arg = br.ReadInt32();
                        //throw new NotImplementedException();
                        break;
                    //     The operand is the 32-bit integer argument to a switch instruction.
                    case OperandType.InlineSwitch: // = 11,
                        instruction.Arg = br.ReadInt32();
                        break;
                    //     The operand is a FieldRef, MethodRef, or TypeRef token.
                    case OperandType.InlineTok: // = 12,
                        instruction.Arg = br.ReadInt32();
                        //throw new NotImplementedException();
                        break;
                    //     The operand is a 32-bit metadata token.
                    case OperandType.InlineType: // = 13,
                        instruction.Arg = br.ReadInt32();
                        break;
                    //     The operand is 16-bit integer containing the ordinal of a local variable or an
                    //     argument.
                    case OperandType.InlineVar: // = 14,
                        instruction.Arg = br.ReadInt16();
                        break;
                    //     The operand is an 8-bit integer branch target.
                    case OperandType.ShortInlineBrTarget: // = 15,
                        instruction.Arg = br.ReadByte();
                        break;
                    //     The operand is an 8-bit integer.
                    case OperandType.ShortInlineI: // = 16,
                        instruction.Arg = br.ReadByte();
                        break;
                    //     The operand is a 32-bit IEEE floating point number.
                    case OperandType.ShortInlineR: // = 17,
                        instruction.Arg = br.ReadSingle();
                        break;
                    //     The operand is an 8-bit integer containing the ordinal of a local variable or
                    //     an argumenta.
                    case OperandType.ShortInlineVar: // = 18
                        instruction.Arg = br.ReadByte();
                        break;
                    default: break;
                }

                result.Add(instruction);

            }

            return result;
        }

        public static string ToString(List<IlInstruction> ilStream) => string.Join("\r\n", Enumerable.Range(0, ilStream.Count).Select(i=> $"{i}: {ilStream[i]}"));

    }
}
