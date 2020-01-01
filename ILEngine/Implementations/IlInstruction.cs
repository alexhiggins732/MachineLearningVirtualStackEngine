using ILEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
namespace ILEngine
{

    public struct ILInstruction
    {

        public static ILInstruction NoOp = new ILInstruction() { OpCode = OpCodes.Nop };
        public static ILInstruction Ret = new ILInstruction() { OpCode = OpCodes.Ret };
        public static ILInstruction Next;
        public static ILInstruction EOF;
        public object Arg;
        public OpCode OpCode;
        public long ByteIndex;
        //TODO: Implement Labels
        public int? Label;
        public ILInstruction[] JumpTargets;
        public int ByteSize
        {
            get
            {
                if (this.OpCode != OpCodes.Switch)
                {
                    return OpCode.Size + OpCodeMetaModel.OpCodeMetaDict[OpCode.Value].OperandTypeByteSize;
                }
                else
                {
                    return OpCode.Size + (1 + ((Array)Arg).Length) * 4;
                }
            }

        }
        public static OpCodeMetaModel GetOpCodeMeta(short opCodeValue)
        {
            if (!OpCodeMetaModel.OpCodeMetaDict.ContainsKey(opCodeValue))
            {
                throw new InvalidOpCodeException(opCodeValue);
            }
            return OpCodeMetaModel.OpCodeMetaDict[opCodeValue];
        }
        public static void RequireNullOperand(short opCodeValue)
        {
            var meta = GetOpCodeMeta(opCodeValue);
            if (meta.OperandTypeByteSize != 0)
            {
                if (meta.OperandTypeByteSize == 1)
                {
                    throw new ILInstructionArgumentException($"{meta.OpCode} requires 1 byte operand argument.");
                }
                else
                {
                    throw new ILInstructionArgumentException($"{meta.OpCode} requires operand argument of {meta.OperandTypeByteSize} bytes");
                }

            }
        }

        public static void RequireOperand(short opCodeValue)
        {
            var meta = GetOpCodeMeta(opCodeValue);
            if (meta.OperandTypeByteSize == 0)
            {
                throw new ILInstructionArgumentException($"Specifying an operand argument for {meta.OpCode} is invalid.");

            }
        }

        public static ILInstruction Create(ILOpCodeValues ilOpCodeValue)
        {
            RequireNullOperand(unchecked((short)ilOpCodeValue));

            var opCode = OpCodeLookup.GetILOpcode((int)ilOpCodeValue);

            var result = new ILInstruction { OpCode = opCode };
            return result;
        }

        public static ILInstruction Create(ILOpCodeValues ilOpCodeValue, object arg)
        {
            RequireOperand(unchecked((short)ilOpCodeValue));
            var opCode = OpCodeLookup.GetILOpcode((int)ilOpCodeValue);
            var result = new ILInstruction { OpCode = opCode, Arg = arg };
            return result;
        }

        public static ILInstruction Create(OpCode opCode)
        {
            RequireNullOperand(unchecked((short)opCode.Value));
            var result = new ILInstruction { OpCode = opCode };
            return result;
        }

        public static ILInstruction Create(OpCode opCode, object arg)
        {
            RequireOperand(unchecked((short)opCode.Value));
            var result = new ILInstruction { OpCode = opCode, Arg = arg };
            return result;
        }

        //private IlFlowControl? flowControl;
        //public IlFlowControl FlowControl => flowControl.HasValue ? flowControl.Value : (flowControl = IlFlowControl.Next).Value;
        public override string ToString() => $"IL_{ByteIndex.ToString("x2").PadLeft(4, '0')} {OpCode.Name}{(!(Arg is null) ? $" {Arg}" : "")}";

        public void Emit(ILGenerator gen, IILInstructionResolver resolver, Dictionary<int, Label> jumpTargets = null)
        {
            if (Label.HasValue && jumpTargets != null)
            {
                gen.MarkLabel(jumpTargets[(int)Label]);
            }
            switch (OpCode.OperandType)
            {

                //     The operand is a 32-bit integer branch target.
                case OperandType.InlineBrTarget: // = 0,
                    gen.Emit(OpCode, (int)Arg);
                    //instruction.Arg = br.ReadInt32();
                    break;
                //     The operand is a 32-bit metadata token.
                case OperandType.InlineField: // = 1,                                        
                                              //gen.Emit(OpCode, (int)Arg);
                    gen.Emit(OpCode, resolver.ResolveFieldToken((int)Arg));
                    //instruction.Arg = br.ReadInt32();
                    break;
                //     The operand is a 32-bit integer.
                case OperandType.InlineI: // = 2,
                    gen.Emit(OpCode, (int)Arg);
                    //instruction.Arg = br.ReadInt32();
                    break;
                //     The operand is a 64-bit integer.
                case OperandType.InlineI8: // = 3,                                            
                    gen.Emit(OpCode, Convert.ToInt64(Arg));
                    //instruction.Arg = br.ReadInt64();
                    break;
                //     The operand is a 32-bit metadata token.
                case OperandType.InlineMethod: // = 4,

                    if (Arg is MethodInfo)
                    {
                        gen.Emit(OpCode, (MethodInfo)Arg);
                    }
                    else if (Arg is ConstructorInfo)
                    {
                        gen.Emit(OpCode, (ConstructorInfo)Arg);
                    }
                    else
                    {
                        var methodBase = resolver.ResolveMethodToken((int)Arg);
                        if (methodBase is MethodInfo method)
                        {
                            //gen.EmitCall(OpCode, method, method.GetParameters().Select(x=> x.ParameterType).ToArray()) ;
                            gen.Emit(OpCode, method);
                        }
                        else
                        {
                            gen.Emit(OpCode, (ConstructorInfo)methodBase);
                        }
                    }
                    //gen.Emit(OpCode, (int)Arg);
                    //instruction.Arg = br.ReadInt32();
                    break;
                //     No operand.
                case OperandType.InlineNone: // = 5,
                    gen.Emit(OpCode);
                    break;
                //     The operand is reserved and should not be used.
                //#pragma warning disable CS0618 // Type or member is obsolete
                //                    case OperandType.InlinePhi: // = 6,
                //#pragma warning restore CS0618 // Type or member is obsolete
                //                        throw new NotImplementedException();
                //     The operand is a 64-bit IEEE floating point number.
                case OperandType.InlineR: // = 7,
                    gen.Emit(OpCode, Convert.ToDouble(Arg));
                    //instruction.Arg = br.ReadDouble();
                    break;
                //     The operand is a 32-bit metadata signature token.

                //gen.Emit(OpCode, (int)Arg);
                //instruction.Arg = br.ReadInt32();
                //     The operand is a 32-bit metadata string token.
                case OperandType.InlineString: // = 10,
                    gen.Emit(OpCode, resolver.ResolveStringToken((int)Arg));
                    //gen.Emit(OpCode, (int)Arg);
                    //instruction.Arg = br.ReadInt32();

                    break;
                //     The operand is the 32-bit integer argument to a switch instruction.
                case OperandType.InlineSwitch: // = 11,
                    var switchTargets = JumpTargets;
                    var labels = switchTargets.Select(x => jumpTargets[(int)x.Label]).ToArray();
                    //var jmp = (int[])Arg;
                    //var labels = jmp.Select(x => gen.DefineLabel()).ToArray();
                    gen.Emit(OpCode, labels);
                    //gen.Emit()
                    //instruction.Arg = br.ReadInt32();
                    //break;
                    //throw new OpCodeNotImplementedException(ILOpCodeValues.Switch);
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
                    else if (member is Type runtimeTypeHandle)
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
                    gen.Emit(OpCode, Convert.ToInt16(Arg));
                    //instruction.Arg = br.ReadInt16();
                    break;
                //     The operand is an 8-bit integer branch target.
                case OperandType.ShortInlineBrTarget: // = 15,
                    gen.Emit(OpCode, Convert.ToByte(Arg));
                    //instruction.Arg = br.ReadByte();
                    break;
                //     The operand is an 8-bit integer.
                case OperandType.ShortInlineI: // = 16,
                    gen.Emit(OpCode, Convert.ToByte(Arg));
                    //instruction.Arg = br.ReadByte();
                    break;
                //     The operand is a 32-bit IEEE floating point number.
                case OperandType.ShortInlineR: // = 17,
                    gen.Emit(OpCode, Convert.ToSingle(Arg));
                    //instruction.Arg = br.ReadSingle();
                    break;
                //     The operand is an 8-bit integer containing the ordinal of a local variable or
                //     an argumenta.
                case OperandType.ShortInlineVar: // = 18
                    gen.Emit(OpCode, Convert.ToByte(Arg));
                    //instruction.Arg = br.ReadByte();
                    break;
                case OperandType.InlineSig: // = 9,
                    //need a calli for a unit test
                    gen.Emit(OpCode, (int)Arg);
                    break;
                default:
                    throw new OpCodeNotImplementedException(OpCode);
                    //gen.Emit(OpCode);
                    //break;
                    //throw new NotImplementedException();
            }

        }
    }
}
