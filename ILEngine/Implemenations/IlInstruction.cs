using System;
using System.Reflection;
using System.Reflection.Emit;
namespace ILEngine
{
    public struct IlInstruction
    {
        public static IlInstruction NoOp = new IlInstruction() { OpCode = OpCodes.Nop };
        public static IlInstruction Next;
        public static IlInstruction EOF;
        public object Arg;
        public OpCode OpCode;
        public long ByteIndex;

        public static IlInstruction Create(ILOpCodeValues ilOpCodeValue)
        {
            var opCode = OpCodeLookup.GetILOpcode((int)ilOpCodeValue);
            var result = new IlInstruction { OpCode = opCode };
            return result;
        }

        public static IlInstruction Create(ILOpCodeValues ilOpCodeValue, object arg)
        {
            var opCode = OpCodeLookup.GetILOpcode((int)ilOpCodeValue);
            var result = new IlInstruction { OpCode = opCode, Arg= arg };
            return result;
        }

        //private IlFlowControl? flowControl;
        //public IlFlowControl FlowControl => flowControl.HasValue ? flowControl.Value : (flowControl = IlFlowControl.Next).Value;
        public override string ToString() => $"IL_{ByteIndex.ToString().PadLeft(4, '0')} {OpCode.Name}{(!(Arg is null) ? $" {Arg}" : "")}";

        public void Emit(ILGenerator gen, IIlInstructionResolver resolver)
        {
            if (Arg is null) gen.Emit(OpCode);
            else
            {
                switch (OpCode.OperandType)
                {

                    //     The operand is a 32-bit integer branch target.
                    case OperandType.InlineBrTarget: // = 0,
                        gen.Emit(OpCode, (int)Arg);
                        //instruction.Arg = br.ReadInt32();
                        break;
                    //     The operand is a 32-bit metadata token.
                    case OperandType.InlineField: // = 1,                                        
                        gen.Emit(OpCode, (int)Arg);
                        //instruction.Arg = br.ReadInt32();
                        break;
                    //     The operand is a 32-bit integer.
                    case OperandType.InlineI: // = 2,
                        gen.Emit(OpCode, (int)Arg);

                        //instruction.Arg = br.ReadInt32();
                        break;
                    //     The operand is a 64-bit integer.
                    case OperandType.InlineI8: // = 3,                                            
                        gen.Emit(OpCode, (long)Arg);
                        //instruction.Arg = br.ReadInt64();
                        break;
                    //     The operand is a 32-bit metadata token.
                    case OperandType.InlineMethod: // = 4,
                        gen.Emit(OpCode, (int)Arg);
                        //instruction.Arg = br.ReadInt32();
                        break;
                    //     No operand.
                    case OperandType.InlineNone: // = 5,
                        gen.Emit(OpCode);
                        break;
                    //     The operand is reserved and should not be used.
                    case OperandType.InlinePhi: // = 6,
                        throw new NotImplementedException();
                        break;
                    //     The operand is a 64-bit IEEE floating point number.
                    case OperandType.InlineR: // = 7,
                        gen.Emit(OpCode, (double)Arg);
                        //instruction.Arg = br.ReadDouble();
                        break;
                    //     The operand is a 32-bit metadata signature token.
                    case OperandType.InlineSig: // = 9,
                        //var signature = resolver.ResolveSignatureToken((int)Arg);
                        //.Empty();
                        gen.Emit(OpCode, (int)Arg);
                        throw new NotImplementedException();
                        //gen.Emit(OpCode, (int)Arg);
                        //instruction.Arg = br.ReadInt32();
                        break;
                    //     The operand is a 32-bit metadata string token.
                    case OperandType.InlineString: // = 10,
                        gen.Emit(OpCode, resolver.ResolveStringToken((int)Arg));
                        //gen.Emit(OpCode, (int)Arg);
                        //instruction.Arg = br.ReadInt32();
                        //throw new NotImplementedException();
                        break;
                    //     The operand is the 32-bit integer argument to a switch instruction.
                    case OperandType.InlineSwitch: // = 11,
                        gen.Emit(OpCode, (int)Arg);
                        //instruction.Arg = br.ReadInt32();
                        break;
                    //     The operand is a FieldRef, MethodRef, or TypeRef token.
                    case OperandType.InlineTok: // = 12,
                        var member = resolver.ResolveMemberToken((int)Arg);

                        if (member is FieldInfo fieldInfo)
                        {
                            gen.Emit(OpCode, fieldInfo);
                        }
                        else if (member is MethodInfo methodInfo)
                        {
                            gen.Emit(OpCode, methodInfo);
                        }
                        if (member is Type runtimeTypeHandle)
                            gen.Emit(OpCode, resolver.ResolveTypeToken((int)Arg));



                        //gen.Emit(OpCode, resolver.ResolveMemberToken((int)Arg));
                        //gen.Emit(OpCode, (int)Arg);
                        //instruction.Arg = br.ReadInt32();
                        //throw new NotImplementedException();
                        break;
                    //     The operand is a 32-bit metadata token.
                    case OperandType.InlineType: // = 13,
                        gen.Emit(OpCode, resolver.ResolveTypeToken((int)Arg));
                        //gen.Emit(OpCode, (int)Arg);
                        //instruction.Arg = br.ReadInt32();
                        break;
                    //     The operand is 16-bit integer containing the ordinal of a local variable or an
                    //     argument.
                    case OperandType.InlineVar: // = 14,
                        gen.Emit(OpCode, (short)Arg);
                        //instruction.Arg = br.ReadInt16();
                        break;
                    //     The operand is an 8-bit integer branch target.
                    case OperandType.ShortInlineBrTarget: // = 15,
                        gen.Emit(OpCode, (byte)Arg);
                        //instruction.Arg = br.ReadByte();
                        break;
                    //     The operand is an 8-bit integer.
                    case OperandType.ShortInlineI: // = 16,
                        gen.Emit(OpCode, (byte)Arg);
                        //instruction.Arg = br.ReadByte();
                        break;
                    //     The operand is a 32-bit IEEE floating point number.
                    case OperandType.ShortInlineR: // = 17,
                        gen.Emit(OpCode, (float)Arg);
                        //instruction.Arg = br.ReadSingle();
                        break;
                    //     The operand is an 8-bit integer containing the ordinal of a local variable or
                    //     an argumenta.
                    case OperandType.ShortInlineVar: // = 18
                        gen.Emit(OpCode, (byte)Arg);
                        //instruction.Arg = br.ReadByte();
                        break;
                    default:
                        gen.Emit(OpCode);
                        break;
                        //throw new NotImplementedException();
                }
            }
        }
    }
}
