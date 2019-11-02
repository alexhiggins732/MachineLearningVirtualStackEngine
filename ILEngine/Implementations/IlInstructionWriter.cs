using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.Implementations
{
    public class IlInstructionWriter
    {
        private List<OpCode> l;

        public IlInstructionWriter(List<OpCode> l)
        {
            this.l = l;
        }

        public void Write()
        {
           
        }

        public List<IlInstruction> GetInstructionStream()
        {
            var result = new List<IlInstruction>();
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    var idx = 0;
                    for (var i = 0; i < l.Count; i++, idx++)
                    {
                        var code = l[i];
                        if (code == OpCodes.Prefix1)
                        {
                            i++;
                            idx++;
                            code = l[i];
                        }
                        var instruction = new IlInstruction { OpCode = code, ByteIndex = idx };
                        //switch (code.OperandType)
                        //{

                        //    //     The operand is a 32-bit integer branch target.
                        //    case OperandType.InlineBrTarget: // = 0,
                        //        instruction.Arg = br.ReadInt32();
                        //        break;
                        //    //     The operand is a 32-bit metadata token.
                        //    case OperandType.InlineField: // = 1,
                        //        instruction.Arg = br.ReadInt32();
                        //        break;
                        //    //     The operand is a 32-bit integer.
                        //    case OperandType.InlineI: // = 2,
                        //        instruction.Arg = br.ReadInt32();
                        //        break;
                        //    //     The operand is a 64-bit integer.
                        //    case OperandType.InlineI8: // = 3,
                        //        instruction.Arg = br.ReadInt64();
                        //        break;
                        //    //     The operand is a 32-bit metadata token.
                        //    case OperandType.InlineMethod: // = 4,
                        //        instruction.Arg = br.ReadInt32();
                        //        break;
                        //    //     No operand.
                        //    case OperandType.InlineNone: // = 5,
                        //        break;
                        //    //     The operand is reserved and should not be used.
                        //    case OperandType.InlinePhi: // = 6,
                        //        throw new NotImplementedException();
                        //        break;
                        //    //     The operand is a 64-bit IEEE floating point number.
                        //    case OperandType.InlineR: // = 7,
                        //        instruction.Arg = br.ReadDouble();
                        //        break;
                        //    //     The operand is a 32-bit metadata signature token.
                        //    case OperandType.InlineSig: // = 9,
                        //        instruction.Arg = br.ReadInt32();
                        //        break;
                        //    //     The operand is a 32-bit metadata string token.
                        //    case OperandType.InlineString: // = 10,
                        //        instruction.Arg = br.ReadInt32();
                        //        //throw new NotImplementedException();
                        //        break;
                        //    //     The operand is the 32-bit integer argument to a switch instruction.
                        //    case OperandType.InlineSwitch: // = 11,
                        //        instruction.Arg = br.ReadInt32();
                        //        break;
                        //    //     The operand is a FieldRef, MethodRef, or TypeRef token.
                        //    case OperandType.InlineTok: // = 12,
                        //        instruction.Arg = br.ReadInt32();
                        //        //throw new NotImplementedException();
                        //        break;
                        //    //     The operand is a 32-bit metadata token.
                        //    case OperandType.InlineType: // = 13,
                        //        instruction.Arg = br.ReadInt32();
                        //        break;
                        //    //     The operand is 16-bit integer containing the ordinal of a local variable or an
                        //    //     argument.
                        //    case OperandType.InlineVar: // = 14,
                        //        instruction.Arg = br.ReadInt16();
                        //        break;
                        //    //     The operand is an 8-bit integer branch target.
                        //    case OperandType.ShortInlineBrTarget: // = 15,
                        //        instruction.Arg = br.ReadByte();
                        //        break;
                        //    //     The operand is an 8-bit integer.
                        //    case OperandType.ShortInlineI: // = 16,
                        //        instruction.Arg = br.ReadByte();
                        //        break;
                        //    //     The operand is a 32-bit IEEE floating point number.
                        //    case OperandType.ShortInlineR: // = 17,
                        //        instruction.Arg = br.ReadSingle();
                        //        break;
                        //    //     The operand is an 8-bit integer containing the ordinal of a local variable or
                        //    //     an argumenta.
                        //    case OperandType.ShortInlineVar: // = 18
                        //        instruction.Arg = br.ReadByte();
                        //        break;
                        //    default: break;
                        //}

                        result.Add(instruction);
                    }
                }

            }
            return result;
        }
    }
}
