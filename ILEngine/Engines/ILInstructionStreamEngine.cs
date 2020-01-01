using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{


    [ExcludeFromCodeCoverage]
    public class ILInstructionStreamEngine: IOpCodeEngine
    {


        public T ExecuteTyped<T>(MethodInfo method, params object[] args)
        {
            var result = ExecuteTyped(method, args);
            return (T)result;
        }

        public object ExecuteTyped(MethodInfo method, params object[] args)
        {
            var argLength = args?.Length;
            var methodParameters = method.GetParameters();
            if (method.CallingConvention == CallingConventions.VarArgs)
            {
                // throw new PlatformID
            }
            var methodParameterCount = methodParameters.Length;
            if (methodParameterCount == 0 && method.ContainsGenericParameters)
            {
                //var genericArguments = method.GetGenericArguments().Length;
            }
            if (!method.IsStatic) methodParameterCount += 1;
            var body = method.GetMethodBody();
            var localVariables = body.LocalVariables;
            if (methodParameterCount != argLength)
            {
                bool parsedParamArray = false;
                for (var i = 0; parsedParamArray == false && i < methodParameters.Length; i++)
                {
                    if (methodParameters[i].GetCustomAttribute<ParamArrayAttribute>() != null)
                    {
                        parsedParamArray = true;
                        var l = new List<object>();
                        for (var k = 0; k < i; k++)
                        {
                            l.Add(args[k]);
                        }

                        var paramArgs = new object[args.Length - i];
                        for (var k = i; k < args.Length; k++)
                        {
                            paramArgs[k - i] = args[k];
                        }
                        l.Add(paramArgs);
                        args = l.ToArray();
                    }
                }
                //if (argLength < methodParameterCount || !methodParameters.Any(x => !(x.GetCustomAttribute<ParamArrayAttribute>() is null)))
                if (!parsedParamArray)
                    throw new TargetParameterCountException($"{method.ReflectedType.Name}::{method} does not take {argLength} parameters");
            }


            //var ilStream = IlInstructionReader.FromByteCode(method.GetMethodBody().GetILAsByteArray());
            //var ilStreamCode = IlInstructionReader.ToString(ilStream);
            var ms = new MemoryStream(method.GetMethodBody().GetILAsByteArray());
            var resolver = new ILInstructionResolver(method);
            var locals = localVariables.Select(x => new ILVariable { Index = x.LocalIndex, Type = x.LocalType, Value = body.InitLocals && x.LocalType.IsValueType ? Activator.CreateInstance(x.LocalType) : null }).ToArray();

            return ExecuteTyped(ms, resolver, args.ToArray(), locals);
        }

        public T ExecuteTyped<T>(Stream stream, IILInstructionResolver resolver = null, object[] args = null, ILVariable[] locals = null)
        {
            var result = ExecuteTyped(stream, resolver, args, locals);
            return (T)result;
        }
        public object ExecuteTyped(Stream stream, IILInstructionResolver resolver = null, object[] args = null, ILVariable[] locals = null)
        {
            var stack = new Stack<object>();
            var br = new BinaryReader(stream);
            int op = 0;
            int i = 0;
            MethodBase method = null;
            Type type;
            object obj = null;
            object[] arr = null;
            Read:
            op = stream.ReadByte();

            //Execute:
            // can we keep tables at 255 to implement br_S. else jumps will be BR and use 4 bytes.
            // to implement need two switches to keep table at 127 entries
            switch (op)
            {
                case 0: goto Nop; // 0x00
                case 1: goto Break; // 0x01
                case 2: goto Ldarg_0; // 0x02
                case 3: goto Ldarg_1; // 0x03
                case 4: goto Ldarg_2; // 0x04
                case 5: goto Ldarg_3; // 0x05
                case 6: goto Ldloc_0; // 0x06
                case 7: goto Ldloc_1; // 0x07
                case 8: goto Ldloc_2; // 0x08
                case 9: goto Ldloc_3; // 0x09
                case 10: goto Stloc_0; // 0x0a
                case 11: goto Stloc_1; // 0x0b
                case 12: goto Stloc_2; // 0x0c
                case 13: goto Stloc_3; // 0x0d
                case 14: goto Ldarg_S; // 0x0e
                case 15: goto Ldarga_S; // 0x0f
                case 16: goto Starg_S; // 0x10
                case 17: goto Ldloc_S; // 0x11
                case 18: goto Ldloca_S; // 0x12
                case 19: goto Stloc_S; // 0x13
                case 20: goto Ldnull; // 0x14
                case 21: goto Ldc_I4_M1; // 0x15
                case 22: goto Ldc_I4_0; // 0x16
                case 23: goto Ldc_I4_1; // 0x17
                case 24: goto Ldc_I4_2; // 0x18
                case 25: goto Ldc_I4_3; // 0x19
                case 26: goto Ldc_I4_4; // 0x1a
                case 27: goto Ldc_I4_5; // 0x1b
                case 28: goto Ldc_I4_6; // 0x1c
                case 29: goto Ldc_I4_7; // 0x1d
                case 30: goto Ldc_I4_8; // 0x1e
                case 31: goto Ldc_I4_S; // 0x1f
                case 32: goto Ldc_I4; // 0x20
                case 33: goto Ldc_I8; // 0x21
                case 34: goto Ldc_R4; // 0x22
                case 35: goto Ldc_R8; // 0x23
                case 36: goto Exec_MSIL_I; // 0x24
                case 37: goto Dup; // 0x25
                case 38: goto Pop; // 0x26
                case 39: goto Jmp; // 0x27
                case 40: goto Call; // 0x28
                case 41: goto Calli; // 0x29
                case 42: goto Ret; // 0x2a
                case 43: goto Br_S; // 0x2b
                case 44: goto Brfalse_S; // 0x2c
                case 45: goto Brtrue_S; // 0x2d
                case 46: goto Beq_S; // 0x2e
                case 47: goto Bge_S; // 0x2f
                case 48: goto Bgt_S; // 0x30
                case 49: goto Ble_S; // 0x31
                case 50: goto Blt_S; // 0x32
                case 51: goto Bne_Un_S; // 0x33
                case 52: goto Bge_Un_S; // 0x34
                case 53: goto Bgt_Un_S; // 0x35
                case 54: goto Ble_Un_S; // 0x36
                case 55: goto Blt_Un_S; // 0x37
                case 56: goto Br; // 0x38
                case 57: goto Brfalse; // 0x39
                case 58: goto Brtrue; // 0x3a
                case 59: goto Beq; // 0x3b
                case 60: goto Bge; // 0x3c
                case 61: goto Bgt; // 0x3d
                case 62: goto Ble; // 0x3e
                case 63: goto Blt; // 0x3f
                case 64: goto Bne_Un; // 0x40
                case 65: goto Bge_Un; // 0x41
                case 66: goto Bgt_Un; // 0x42
                case 67: goto Ble_Un; // 0x43
                case 68: goto Blt_Un; // 0x44
                case 69: goto Switch; // 0x45
                case 70: goto Ldind_I1; // 0x46
                case 71: goto Ldind_U1; // 0x47
                case 72: goto Ldind_I2; // 0x48
                case 73: goto Ldind_U2; // 0x49
                case 74: goto Ldind_I4; // 0x4a
                case 75: goto Ldind_U4; // 0x4b
                case 76: goto Ldind_I8; // 0x4c
                case 77: goto Ldind_I; // 0x4d
                case 78: goto Ldind_R4; // 0x4e
                case 79: goto Ldind_R8; // 0x4f
                case 80: goto Ldind_Ref; // 0x50
                case 81: goto Stind_Ref; // 0x51
                case 82: goto Stind_I1; // 0x52
                case 83: goto Stind_I2; // 0x53
                case 84: goto Stind_I4; // 0x54
                case 85: goto Stind_I8; // 0x55
                case 86: goto Stind_R4; // 0x56
                case 87: goto Stind_R8; // 0x57
                case 88: goto Add; // 0x58
                case 89: goto Sub; // 0x59
                case 90: goto Mul; // 0x5a
                case 91: goto Div; // 0x5b
                case 92: goto Div_Un; // 0x5c
                case 93: goto Rem; // 0x5d
                case 94: goto Rem_Un; // 0x5e
                case 95: goto And; // 0x5f
                case 96: goto Or; // 0x60
                case 97: goto Xor; // 0x61
                case 98: goto Shl; // 0x62
                case 99: goto Shr; // 0x63
                case 100: goto Shr_Un; // 0x64
                case 101: goto Neg; // 0x65
                case 102: goto Not; // 0x66
                case 103: goto Conv_I1; // 0x67
                case 104: goto Conv_I2; // 0x68
                case 105: goto Conv_I4; // 0x69
                case 106: goto Conv_I8; // 0x6a
                case 107: goto Conv_R4; // 0x6b
                case 108: goto Conv_R8; // 0x6c
                case 109: goto Conv_U4; // 0x6d
                case 110: goto Conv_U8; // 0x6e
                case 111: goto Callvirt; // 0x6f
                case 112: goto Cpobj; // 0x70
                case 113: goto Ldobj; // 0x71
                case 114: goto Ldstr; // 0x72
                case 115: goto Newobj; // 0x73
                case 116: goto Castclass; // 0x74
                case 117: goto Isinst; // 0x75
                case 118: goto Conv_R_Un; // 0x76
                case 119: goto Exec_MSIL_S; // 0x77
                case 120: goto NotSupported_S; //0x78
                case 121: goto Unbox; // 0x79
                case 122: goto Throw; // 0x7a
                case 123: goto Ldfld; // 0x7b
                case 124: goto Ldflda; // 0x7c
                case 125: goto Stfld; // 0x7d
                case 126: goto Ldsfld; // 0x7e
                case 127: goto Ldsflda; // 0x7f
                case 128: goto Stsfld; // 0x80
                case 129: goto Stobj; // 0x81
                case 130: goto Conv_Ovf_I1_Un; // 0x82
                case 131: goto Conv_Ovf_I2_Un; // 0x83
                case 132: goto Conv_Ovf_I4_Un; // 0x84
                case 133: goto Conv_Ovf_I8_Un; // 0x85
                case 134: goto Conv_Ovf_U1_Un; // 0x86
                case 135: goto Conv_Ovf_U2_Un; // 0x87
                case 136: goto Conv_Ovf_U4_Un; // 0x88
                case 137: goto Conv_Ovf_U8_Un; // 0x89
                case 138: goto Conv_Ovf_I_Un; // 0x8a
                case 139: goto Conv_Ovf_U_Un; // 0x8b
                case 140: goto Box; // 0x8c
                case 141: goto Newarr; // 0x8d
                case 142: goto Ldlen; // 0x8e
                case 143: goto Ldelema; // 0x8f
                case 144: goto Ldelem_I1; // 0x90
                case 145: goto Ldelem_U1; // 0x91
                case 146: goto Ldelem_I2; // 0x92
                case 147: goto Ldelem_U2; // 0x93
                case 148: goto Ldelem_I4; // 0x94
                case 149: goto Ldelem_U4; // 0x95
                case 150: goto Ldelem_I8; // 0x96
                case 151: goto Ldelem_I; // 0x97
                case 152: goto Ldelem_R4; // 0x98
                case 153: goto Ldelem_R8; // 0x99
                case 154: goto Ldelem_Ref; // 0x9a
                case 155: goto Stelem_I; // 0x9b
                case 156: goto Stelem_I1; // 0x9c
                case 157: goto Stelem_I2; // 0x9d
                case 158: goto Stelem_I4; // 0x9e
                case 159: goto Stelem_I8; // 0x9f
                case 160: goto Stelem_R4; // 0xa0
                case 161: goto Stelem_R8; // 0xa1
                case 162: goto Stelem_Ref; // 0xa2
                case 163: goto Ldelem; // 0xa3
                case 164: goto Stelem; // 0xa4
                case 165: goto Unbox_Any; // 0xa5
                case 166: goto NotSupported_S; //0xA6
                case 167: goto NotSupported_S; //0xA7
                case 168: goto NotSupported_S; //0xA8
                case 169: goto NotSupported_S; //0xA9
                case 170: goto NotSupported_S; //0xAA
                case 171: goto NotSupported_S; //0xAB
                case 172: goto NotSupported_S; //0xAC
                case 173: goto NotSupported_S; //0xAD
                case 174: goto NotSupported_S; //0xAE
                case 175: goto NotSupported_S; //0xAF
                case 176: goto NotSupported_S; //0xB0
                case 177: goto NotSupported_S; //0xB1
                case 178: goto NotSupported_S; //0xB2
                case 179: goto Conv_Ovf_I1; // 0xb3
                case 180: goto Conv_Ovf_U1; // 0xb4
                case 181: goto Conv_Ovf_I2; // 0xb5
                case 182: goto Conv_Ovf_U2; // 0xb6
                case 183: goto Conv_Ovf_I4; // 0xb7
                case 184: goto Conv_Ovf_U4; // 0xb8
                case 185: goto Conv_Ovf_I8; // 0xb9
                case 186: goto Conv_Ovf_U8; // 0xba
                case 187: goto NotSupported_S; //0xBB
                case 188: goto NotSupported_S; //0xBC
                case 189: goto NotSupported_S; //0xBD
                case 190: goto NotSupported_S; //0xBE
                case 191: goto NotSupported_S; //0xBF
                case 192: goto NotSupported_S; //0xC0
                case 193: goto NotSupported_S; //0xC1
                case 194: goto Refanyval; // 0xc2
                case 195: goto Ckfinite; // 0xc3
                case 196: goto NotSupported_S; //0xC4
                case 197: goto NotSupported_S; //0xC5
                case 198: goto Mkrefany; // 0xc6
                case 199: goto NotSupported_S; //0xC7
                case 200: goto NotSupported_S; //0xC8
                case 201: goto NotSupported_S; //0xC9
                case 202: goto NotSupported_S; //0xCA
                case 203: goto NotSupported_S; //0xCB
                case 204: goto NotSupported_S; //0xCC
                case 205: goto NotSupported_S; //0xCD
                case 206: goto NotSupported_S; //0xCE
                case 207: goto NotSupported_S; //0xCF
                case 208: goto Ldtoken; // 0xd0
                case 209: goto Conv_U2; // 0xd1
                case 210: goto Conv_U1; // 0xd2
                case 211: goto Conv_I; // 0xd3
                case 212: goto Conv_Ovf_I; // 0xd4
                case 213: goto Conv_Ovf_U; // 0xd5
                case 214: goto Add_Ovf; // 0xd6
                case 215: goto Add_Ovf_Un; // 0xd7
                case 216: goto Mul_Ovf; // 0xd8
                case 217: goto Mul_Ovf_Un; // 0xd9
                case 218: goto Sub_Ovf; // 0xda
                case 219: goto Sub_Ovf_Un; // 0xdb
                case 220: goto Endfinally; // 0xdc
                case 221: goto Leave; // 0xdd
                case 222: goto Leave_S; // 0xde
                case 223: goto Stind_I; // 0xdf
                case 224: goto Conv_U; // 0xe0
                case 225: goto NotSupported_S; //0xE1
                case 226: goto NotSupported_S; //0xE2
                case 227: goto NotSupported_S; //0xE3
                case 228: goto NotSupported_S; //0xE4
                case 229: goto NotSupported_S; //0xE5
                case 230: goto NotSupported_S; //0xE6
                case 231: goto NotSupported_S; //0xE7
                case 232: goto NotSupported_S; //0xE8
                case 233: goto NotSupported_S; //0xE9
                case 234: goto NotSupported_S; //0xEA
                case 235: goto NotSupported_S; //0xEB
                case 236: goto NotSupported_S; //0xEC
                case 237: goto NotSupported_S; //0xED
                case 238: goto NotSupported_S; //0xEE
                case 239: goto NotSupported_S; //0xEF
                case 240: goto NotSupported_S; //0xF0
                case 241: goto NotSupported_S; //0xF1
                case 242: goto NotSupported_S; //0xF2
                case 243: goto NotSupported_S; //0xF3
                case 244: goto NotSupported_S; //0xF4
                case 245: goto NotSupported_S; //0xF5
                case 246: goto NotSupported_S; //0xF6
                case 247: goto NotSupported_S; //0xF7
                case 248: goto Prefix7; // 0xf8
                case 249: goto Prefix6; // 0xf9
                case 250: goto Prefix5; // 0xfa
                case 251: goto Prefix4; // 0xfb
                case 252: goto Prefix3; // 0xfc
                case 253: goto Prefix2; // 0xfd
                case 254: goto Prefix1; // 0xfe
                case 255: goto Prefixref; // 0xff
                default: throw new NotImplementedException();
            }

            NotSupported_S: goto NotSupported;
            NotImplemented_S: goto NotImplemented;

            Nop: goto Read;
            Break: System.Diagnostics.Debugger.Break(); goto Read;
            Ldarg_0: stack.Push(args[0]); goto Read;
            Ldarg_1: stack.Push(args[1]); goto Read;
            Ldarg_2: stack.Push(args[2]); goto Read;
            Ldarg_3: stack.Push(args[3]); goto Read;
            Ldloc_0: stack.Push(locals[0].Value); goto Read;
            Ldloc_1: stack.Push(locals[1].Value); goto Read;
            Ldloc_2: stack.Push(locals[2].Value); goto Read;
            Ldloc_3: stack.Push(locals[3].Value); goto Read;
            Stloc_0: locals[0].Value = stack.Pop(); goto Read;
            Stloc_1: locals[2].Value = stack.Pop(); goto Read;
            Stloc_2: locals[3].Value = stack.Pop(); goto Read;
            Stloc_3: locals[4].Value = stack.Pop(); goto Read;
            Ldarg_S: stack.Push(args[stream.ReadByte()]); goto Read;
            Ldarga_S: stack.Push(args[stream.ReadByte()]); goto Read;
            Starg_S: args[stream.ReadByte()] = stack.Pop(); goto Read;
            Ldloc_S: stack.Push(locals[stream.ReadByte()].Value); goto Read;
            Ldloca_S: stack.Push(locals[stream.ReadByte()].Value); goto Read;
            Stloc_S: locals[stream.ReadByte()].Value = stack.Pop(); goto Read;
            Ldnull: stack.Push(null); goto Read;
            Ldc_I4_M1: stack.Push(-1); goto Read;
            Ldc_I4_0: stack.Push(0); goto Read;
            Ldc_I4_1: stack.Push(1); goto Read;
            Ldc_I4_2: stack.Push(2); goto Read;
            Ldc_I4_3: stack.Push(3); goto Read;
            Ldc_I4_4: stack.Push(4); goto Read;
            Ldc_I4_5: stack.Push(5); goto Read;
            Ldc_I4_6: stack.Push(6); goto Read;
            Ldc_I4_7: stack.Push(7); goto Read;
            Ldc_I4_8: stack.Push(8); goto Read;
            Ldc_I4_S: stack.Push(stream.ReadByte()); goto Read;
            Ldc_I4: stack.Push(br.ReadInt32()); goto Read;
            Ldc_I8: stack.Push(br.ReadInt64()); goto Read;
            Ldc_R4: stack.Push(br.ReadSingle()); goto Read;
            Ldc_R8: stack.Push(br.ReadDouble()); goto Read;
            Exec_MSIL_I: goto NotSupported_S;
            Dup: stack.Push(stack.Peek()); goto Read;
            Pop: stack.Pop(); goto Read;
            Jmp: goto NotImplemented_S;
            Call:
            method = resolver.ResolveMethodToken(br.ReadInt32());
            arr = new object[method.GetParameters().Length];
            for (i = arr.Length - 1; i >= 0; i--) arr[i] = stack.Pop();
            var methodResult = method.Invoke(method.IsStatic ? null : stack.Pop(), arr);
            if (((MethodInfo)method).ReturnType != typeof(void)) stack.Push(methodResult);
            goto Read;
            Calli: goto Call;
            Ret: goto RetResult;
            //Br_S: stream.Position += br.ReadByte(); goto Read;
            //Brfalse_S: //jmp = stream.ReadByte(); if ((int)stack.Pop() == 0) stream.Position += jmp; goto Read;
            //Brtrue_S: jmp = stream.ReadByte(); if ((int)stack.Pop() == 1) stream.Position += jmp; goto Read;
            //Beq_S: jmp = stream.ReadByte(); if ((dynamic)stack.Pop() == (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Bge_S: jmp = stream.ReadByte(); if ((dynamic)stack.Pop() <= (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Bgt_S: jmp = stream.ReadByte(); if ((dynamic)stack.Pop() < (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Ble_S: jmp = stream.ReadByte(); if ((dynamic)stack.Pop() >= (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Blt_S: jmp = stream.ReadByte(); if ((dynamic)stack.Pop() > (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Bne_Un_S: jmp = stream.ReadByte(); if ((dynamic)stack.Pop() != (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Bge_Un_S: jmp = stream.ReadByte(); if ((dynamic)stack.Pop() <= (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Bgt_Un_S: jmp = stream.ReadByte(); if ((dynamic)stack.Pop() < (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Ble_Un_S: jmp = stream.ReadByte(); if ((dynamic)stack.Pop() >= (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Blt_Un_S: jmp = stream.ReadByte(); if ((dynamic)stack.Pop() > (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Br: stream.Position += br.ReadInt32(); goto Execute;
            //Brfalse: jmp = br.ReadInt32(); if ((int)stack.Pop() == 0) stream.Position += jmp; goto Read;
            //Brtrue: jmp = br.ReadInt32(); if ((int)stack.Pop() == 1) stream.Position += jmp; goto Read;
            //Beq: jmp = br.ReadInt32(); if ((dynamic)stack.Pop() == (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Bge: jmp = br.ReadInt32(); if ((dynamic)stack.Pop() <= (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Bgt: jmp = br.ReadInt32(); if ((dynamic)stack.Pop() < (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Ble: jmp = br.ReadInt32(); if ((dynamic)stack.Pop() >= (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Blt: jmp = br.ReadInt32(); if ((dynamic)stack.Pop() > (dynamic)stack.Pop()) stream.Position += jmp; goto Read; ;
            //Bne_Un: jmp = br.ReadInt32(); if ((dynamic)stack.Pop() != (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Bge_Un: jmp = br.ReadInt32(); if ((dynamic)stack.Pop() <= (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Bgt_Un: jmp = br.ReadInt32(); if ((dynamic)stack.Pop() < (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Ble_Un: jmp = br.ReadInt32(); if ((dynamic)stack.Pop() >= (dynamic)stack.Pop()) stream.Position += jmp; goto Read;
            //Blt_Un: jmp = br.ReadInt32(); if ((dynamic)stack.Pop() > (dynamic)stack.Pop()) stream.Position += jmp; goto Read;

            Br_S: stream.Seek(br.ReadByte(), SeekOrigin.Current); goto Read;
            Brfalse_S: stream.Seek((int)stack.Pop() == 0 ? br.ReadByte() : 1, SeekOrigin.Current); goto Read;//jmp = stream.ReadByte(); if ((int)stack.Pop() == 0) stream.Position += jmp; goto Read;
            Brtrue_S: stream.Seek((int)stack.Pop() == 1 ? br.ReadByte() : 1, SeekOrigin.Current); goto Read;
            Beq_S: stream.Seek((dynamic)stack.Pop() == (dynamic)stack.Pop() ? br.ReadByte() : 1, SeekOrigin.Current); goto Read;
            Bge_S: stream.Seek((dynamic)stack.Pop() <= (dynamic)stack.Pop() ? br.ReadByte() : 1, SeekOrigin.Current); goto Read;
            Bgt_S: stream.Seek((dynamic)stack.Pop() < (dynamic)stack.Pop() ? br.ReadByte() : 1, SeekOrigin.Current); goto Read;
            Ble_S: stream.Seek((dynamic)stack.Pop() >= (dynamic)stack.Pop() ? br.ReadByte() : 1, SeekOrigin.Current); goto Read;
            Blt_S: stream.Seek((dynamic)stack.Pop() > (dynamic)stack.Pop() ? br.ReadByte() : 1, SeekOrigin.Current); goto Read;
            Bne_Un_S: stream.Seek((dynamic)stack.Pop() != (dynamic)stack.Pop() ? br.ReadByte() : 1, SeekOrigin.Current); goto Read;
            Bge_Un_S: stream.Seek((dynamic)stack.Pop() <= (dynamic)stack.Pop() ? br.ReadByte() : 1, SeekOrigin.Current); goto Read;
            Bgt_Un_S: stream.Seek((dynamic)stack.Pop() < (dynamic)stack.Pop() ? br.ReadByte() : 1, SeekOrigin.Current); goto Read;
            Ble_Un_S: stream.Seek((dynamic)stack.Pop() >= (dynamic)stack.Pop() ? br.ReadByte() : 1, SeekOrigin.Current); goto Read;
            Blt_Un_S: stream.Seek((dynamic)stack.Pop() > (dynamic)stack.Pop() ? br.ReadByte() : 1, SeekOrigin.Current); goto Read;
            Br: stream.Seek(br.ReadInt32(), SeekOrigin.Current); goto Read;
            Brfalse: stream.Seek((int)stack.Pop() == 0 ? br.ReadInt32() : 4, SeekOrigin.Current); goto Read;
            Brtrue: stream.Seek((int)stack.Pop() == 1 ? br.ReadInt32() : 4, SeekOrigin.Current); goto Read;
            Beq: stream.Seek((dynamic)stack.Pop() == (dynamic)stack.Pop() ? br.ReadInt32() : 4, SeekOrigin.Current); goto Read;
            Bge: stream.Seek((dynamic)stack.Pop() <= (dynamic)stack.Pop() ? br.ReadInt32() : 4, SeekOrigin.Current); goto Read;
            Bgt: stream.Seek((dynamic)stack.Pop() < (dynamic)stack.Pop() ? br.ReadInt32() : 4, SeekOrigin.Current); goto Read;
            Ble: stream.Seek((dynamic)stack.Pop() >= (dynamic)stack.Pop() ? br.ReadInt32() : 4, SeekOrigin.Current); goto Read;
            Blt: stream.Seek((dynamic)stack.Pop() > (dynamic)stack.Pop() ? br.ReadInt32() : 4, SeekOrigin.Current); goto Read;
            Bne_Un: stream.Seek((dynamic)stack.Pop() != (dynamic)stack.Pop() ? br.ReadInt32() : 4, SeekOrigin.Current); goto Read;
            Bge_Un: stream.Seek((dynamic)stack.Pop() <= (dynamic)stack.Pop() ? br.ReadInt32() : 4, SeekOrigin.Current); goto Read;
            Bgt_Un: stream.Seek((dynamic)stack.Pop() < (dynamic)stack.Pop() ? br.ReadInt32() : 4, SeekOrigin.Current); goto Read;
            Ble_Un: stream.Seek((dynamic)stack.Pop() >= (dynamic)stack.Pop() ? br.ReadInt32() : 4, SeekOrigin.Current); goto Read;
            Blt_Un: stream.Seek((dynamic)stack.Pop() > (dynamic)stack.Pop() ? br.ReadInt32() : 4, SeekOrigin.Current); goto Read;

            Switch: goto NotImplemented_S; //TODO: implementation requrires full scan of msil instructions, jmp table, etc.
            Ldind_I1: goto NotImplemented_S;
            Ldind_U1: goto NotImplemented_S;
            Ldind_I2: goto NotImplemented_S;
            Ldind_U2: goto NotImplemented_S;
            Ldind_I4: goto NotImplemented_S;
            Ldind_U4: goto NotImplemented_S;
            Ldind_I8: goto NotImplemented_S;
            Ldind_I: goto NotImplemented_S;
            Ldind_R4: goto NotImplemented_S;
            Ldind_R8: goto NotImplemented_S;
            Ldind_Ref: goto NotImplemented_S;
            Stind_Ref: goto NotImplemented_S;
            Stind_I1: goto NotImplemented_S;
            Stind_I2: goto NotImplemented_S;
            Stind_I4: goto NotImplemented_S;
            Stind_I8: goto NotImplemented_S;
            Stind_R4: goto NotImplemented_S;
            Stind_R8: goto NotImplemented_S;
            Add: stack.Push((dynamic)stack.Pop() + (dynamic)stack.Pop()); goto Read;
            Sub: stack.Push((dynamic)stack.Pop() - (dynamic)stack.Pop()); goto Read;
            Mul: stack.Push((dynamic)stack.Pop() * (dynamic)stack.Pop()); goto Read;
            Div: stack.Push((dynamic)stack.Pop() / (dynamic)stack.Pop()); goto Read;
            Div_Un: stack.Push((dynamic)stack.Pop() / (dynamic)stack.Pop()); goto Read;
            Rem: stack.Push((dynamic)stack.Pop() % (dynamic)stack.Pop()); goto Read;
            Rem_Un: stack.Push((dynamic)stack.Pop() % (dynamic)stack.Pop()); goto Read;
            And: stack.Push((dynamic)stack.Pop() & (dynamic)stack.Pop()); goto Read;
            Or: stack.Push((dynamic)stack.Pop() | (dynamic)stack.Pop()); goto Read;
            Xor: stack.Push((dynamic)stack.Pop() ^ (dynamic)stack.Pop()); goto Read;
            Shl: stack.Push((dynamic)stack.Pop() << br.ReadInt32()); goto Read;
            Shr: stack.Push((dynamic)stack.Pop() >> br.ReadInt32()); goto Read;
            Shr_Un: stack.Push((dynamic)stack.Pop() >> br.ReadInt32()); goto Read;
            Neg: stack.Push(-(dynamic)stack.Pop()); goto Read;
            Not: stack.Push(!(dynamic)stack.Pop()); goto Read;
            Conv_I1: stack.Push(Convert.ToSByte(stack.Pop())); goto Read;
            Conv_I2: stack.Push(Convert.ToInt16(stack.Pop())); goto Read;
            Conv_I4: stack.Push(Convert.ToInt32(stack.Pop())); goto Read;
            Conv_I8: stack.Push(Convert.ToInt64(stack.Pop())); goto Read;
            Conv_R4: stack.Push(Convert.ToSingle(stack.Pop())); goto Read;
            Conv_R8: stack.Push(Convert.ToDouble(stack.Pop())); goto Read;
            Conv_U4: stack.Push(Convert.ToUInt32(stack.Pop())); goto Read;
            Conv_U8: stack.Push(Convert.ToUInt64(stack.Pop())); goto Read;
            Callvirt: goto Call;
            Cpobj: goto NotImplemented_S;
            Ldobj: goto NotImplemented_S;
            Ldstr: stack.Push(resolver.ResolveStringToken(br.ReadInt32())); goto Read;
            Newobj:
            method = resolver.ResolveMethodToken(br.ReadInt32());
            arr = new object[method.GetParameters().Length];
            for (i = arr.Length - 1; i >= 0; i--) arr[i] = stack.Pop();
            stack.Push(((ConstructorInfo)method).Invoke(arr));
            goto Read;
            Castclass: stack.Push(Convert.ChangeType(stack.Pop(), resolver.ResolveTypeToken(br.ReadInt32()))); goto Read;
            Isinst:
            type = resolver.ResolveTypeToken(br.ReadInt32());
            obj = stack.Pop();
            if (obj.GetType().IsAssignableFrom(type))
                stack.Push(Convert.ChangeType(obj, type));
            else
                stack.Push(0);

            Conv_R_Un: stack.Push(Convert.ToSingle(stack.Pop())); goto Read;
            Exec_MSIL_S: goto NotSupported_S;
            Unbox: stack.Push(Convert.ChangeType(stack.Pop(), resolver.ResolveTypeToken(br.ReadInt32())));
            Throw: goto NotSupported_S;
            Ldfld: stack.Push(resolver.ResolveFieldToken(br.ReadInt32()).GetValue(stack.Pop())); goto Read;
            Ldflda: goto NotSupported_S;
            Stfld: var val = stack.Pop(); resolver.ResolveFieldToken(br.ReadInt32()).SetValue(stack.Pop(), val); goto Read;
            Ldsfld: stack.Push(resolver.ResolveFieldToken(br.ReadInt32()).GetValue(null)); goto Read;
            Ldsflda: goto NotSupported_S;
            Stsfld: resolver.ResolveFieldToken(br.ReadInt32()).SetValue(null, stack.Pop()); goto Read;
            Stobj: goto NotSupported_S;
            Conv_Ovf_I1_Un: stack.Push(Convert.ToSByte(stack.Pop())); goto Read;
            Conv_Ovf_I2_Un: stack.Push(Convert.ToInt16(stack.Pop())); goto Read;
            Conv_Ovf_I4_Un: stack.Push(Convert.ToInt32(stack.Pop())); goto Read;
            Conv_Ovf_I8_Un: stack.Push(Convert.ToInt64(stack.Pop())); goto Read;
            Conv_Ovf_U1_Un: stack.Push(Convert.ToByte(stack.Pop())); goto Read;
            Conv_Ovf_U2_Un: stack.Push(Convert.ToUInt16(stack.Pop())); goto Read;
            Conv_Ovf_U4_Un: stack.Push(Convert.ToUInt32(stack.Pop())); goto Read;
            Conv_Ovf_U8_Un: stack.Push(Convert.ToUInt64(stack.Pop())); goto Read;
            Conv_Ovf_I_Un: goto NotSupported_S;
            Conv_Ovf_U_Un: goto NotSupported_S;
            Box: stack.Push((object)stack.Pop()); goto Read;
            Newarr: stack.Push(Array.CreateInstance(resolver.ResolveTypeToken(br.ReadInt32()), (int)(stack.Pop())));
            goto Read;
            Ldlen: stack.Push(((Array)stack.Pop()).Length); goto Read;
            Ldelema: goto Read;
            Ldelem_I1: i = (int)stack.Pop(); stack.Push(Convert.ToSByte(((object[])stack.Pop())[i])); goto Read;
            Ldelem_U1: i = (int)stack.Pop(); stack.Push(Convert.ToByte(((object[])stack.Pop())[i])); goto Read;
            Ldelem_I2: i = (int)stack.Pop(); stack.Push(Convert.ToInt16(((object[])stack.Pop())[i])); goto Read;
            Ldelem_U2: i = (int)stack.Pop(); stack.Push(Convert.ToUInt32(((object[])stack.Pop())[i])); goto Read;
            Ldelem_I4: i = (int)stack.Pop(); stack.Push(Convert.ToInt32(((object[])stack.Pop())[i])); goto Read;
            Ldelem_U4: i = (int)stack.Pop(); stack.Push(Convert.ToUInt32(((object[])stack.Pop())[i])); goto Read;
            Ldelem_I8: i = (int)stack.Pop(); stack.Push(Convert.ToInt64(((object[])stack.Pop())[i])); goto Read;
            Ldelem_I: goto NotSupported_S;
            Ldelem_R4: i = (int)stack.Pop(); stack.Push((float)((object[])stack.Pop())[i]); goto Read;
            Ldelem_R8: i = (int)stack.Pop(); stack.Push((double)((object[])stack.Pop())[i]); goto Read;
            Ldelem_Ref: i = (int)stack.Pop(); stack.Push(((object[])stack.Pop())[i]); goto Read;
            Stelem_I: goto NotSupported_S;
            Stelem_I1: obj = Convert.ToSByte(stack.Pop()); i = (int)stack.Pop(); ((object[])stack.Pop())[i] = obj; goto Read;
            Stelem_I2: obj = Convert.ToInt16(stack.Pop()); i = (int)stack.Pop(); ((object[])stack.Pop())[i] = obj; goto Read;
            Stelem_I4: obj = Convert.ToInt32(stack.Pop()); i = (int)stack.Pop(); ((object[])stack.Pop())[i] = obj; goto Read;
            Stelem_I8: obj = Convert.ToInt64(stack.Pop()); i = (int)stack.Pop(); ((object[])stack.Pop())[i] = obj; goto Read;
            Stelem_R4: obj = Convert.ToSingle(stack.Pop()); i = (int)stack.Pop(); ((object[])stack.Pop())[i] = obj; goto Read;
            Stelem_R8: obj = Convert.ToInt64(stack.Pop()); i = (int)stack.Pop(); ((object[])stack.Pop())[i] = obj; goto Read;
            Stelem_Ref: obj = stack.Pop(); i = (int)stack.Pop(); ((object[])stack.Pop())[i] = obj; goto Read;
            Ldelem: i = (int)stack.Pop(); stack.Push(Convert.ChangeType(((object[])stack.Pop())[i], resolver.ResolveTypeToken(br.ReadInt32()))); goto Read;
            Stelem: obj = Convert.ChangeType(stack.Pop(), resolver.ResolveTypeToken(br.ReadInt32())); i = (int)stack.Pop(); ((object[])stack.Pop())[i] = obj; goto Read;
            Unbox_Any: stack.Push(Convert.ChangeType(stack.Pop(), resolver.ResolveTypeToken(br.ReadInt32()))); goto Read;
            Conv_Ovf_I1: stack.Push(Convert.ToSByte(stack.Pop())); goto Read;
            Conv_Ovf_U1: stack.Push(Convert.ToByte(stack.Pop())); goto Read;
            Conv_Ovf_I2: stack.Push(Convert.ToInt16(stack.Pop())); goto Read;
            Conv_Ovf_U2: stack.Push(Convert.ToUInt16(stack.Pop())); goto Read;
            Conv_Ovf_I4: stack.Push(Convert.ToInt32(stack.Pop())); goto Read;
            Conv_Ovf_U4: stack.Push(Convert.ToUInt32(stack.Pop())); goto Read;
            Conv_Ovf_I8: stack.Push(Convert.ToInt32(stack.Pop())); goto Read;
            Conv_Ovf_U8: stack.Push(Convert.ToUInt64(stack.Pop())); goto Read;
            Refanyval: goto NotSupported_S;
            Ckfinite: var ckval = stack.Pop(); stack.Push(Convert.ToInt32(ckval is float ? float.IsInfinity((float)ckval) : double.IsInfinity((double)ckval))); goto Read;
            Mkrefany: goto NotSupported_S;
            Ldtoken: stack.Push(resolver.ResolveMemberToken(br.ReadInt32())); goto Read;
            Conv_U2: stack.Push(unchecked((ushort)stack.Pop())); goto Read;
            Conv_U1: stack.Push(unchecked((byte)stack.Pop())); goto Read;
            Conv_I: goto NotSupported_S;
            Conv_Ovf_I: goto NotSupported_S;
            Conv_Ovf_U: goto NotSupported_S;
            Add_Ovf: goto Add;
            Add_Ovf_Un: goto Add;
            Mul_Ovf: goto Mul;
            Mul_Ovf_Un: goto Mul;
            Sub_Ovf: goto Sub;
            Sub_Ovf_Un: goto Sub;
            Endfinally: goto NotSupported_S;
            Leave: goto NotSupported_S;
            Leave_S: goto NotSupported_S;
            Stind_I: goto NotSupported_S;
            Conv_U: goto NotSupported_S;
            Prefix7: goto NotSupported_S;
            Prefix6: goto NotSupported_S;
            Prefix5: goto NotSupported_S;
            Prefix4: goto NotSupported_S;
            Prefix3: goto NotSupported_S;
            Prefix2: goto NotSupported_S;

            Prefix1:

            switch (op = stream.ReadByte())
            {
                case 0: goto NotSupportedLong; //arglist
                case 1: stack.Push(Convert.ToInt32((dynamic)stack.Pop() == (dynamic)stack.Pop())); goto Read; // Ceq = 0xfe01,
                case 2: stack.Push(Convert.ToInt32((dynamic)stack.Pop() < (dynamic)stack.Pop())); goto Read;  // Cgt = 0xfe02,
                case 3: stack.Push(Convert.ToInt32((dynamic)stack.Pop() < (dynamic)stack.Pop())); goto Read;   // Cgt_Un = 0xfe03,
                case 4: stack.Push(Convert.ToInt32((dynamic)stack.Pop() > (dynamic)stack.Pop())); goto Read;  //     Clt = 0xfe04,
                case 5: stack.Push(Convert.ToInt32((dynamic)stack.Pop() < (dynamic)stack.Pop())); goto Read;  //       Clt_Un = 0xfe05,
                case 6: goto NotSupportedLong; // Ldftn = 0xfe06,
                case 7: goto NotSupportedLong;  // Ldvirtftn = 0xfe07,
                case 8: goto NotImplementedLong; // 0xfe08,
                case 9: stack.Push(args[br.ReadInt16()]); goto Read;  // Ldarg = 0xfe09,
                case 10: stack.Push(args[br.ReadInt16()]); goto Read;  // Ldarga = 0xfe0a,
                case 11: args[br.ReadInt16()] = stack.Pop(); goto Read;  // Starg = 0xfe0b,
                case 12: stack.Push(locals[br.ReadInt16()].Value); goto Read; // Ldloc = 0xfe0c,
                case 13: stack.Push(locals[br.ReadInt16()].Value); goto Read;   // Ldloca = 0xfe0d,
                case 14: locals[br.ReadInt16()].Value = stack.Pop(); goto Read;   // Stloc = 0xfe0e,
                case 15: goto NotSupportedLong;  // Localloc = 0xfe0f,
                case 16: goto NotImplementedLong; //0xfe10,
                case 17: goto NotSupportedLong;  //  Endfilter = 0xfe11,
                case 18: goto NotSupportedLong;   // Unaligned_ = 0xfe12,
                case 19: goto NotSupportedLong;  // Volatile_ = 0xfe13,
                case 20: goto NotSupportedLong;  // Tail_ = 0xfe14,
                case 21: goto NotSupportedLong;  // Initobj = 0xfe15,
                case 22: goto NotSupportedLong;  // Constrained_ = 0xfe16,
                case 23: goto NotSupportedLong;  // Cpblk = 0xfe17,
                case 24: goto NotSupportedLong;  // Initblk = 0xfe18,
                case 25: goto NotImplementedLong; //0xfe19
                case 26: goto NotSupportedLong;  // Rethrow = 0xfe1a

                case 27: goto NotImplementedLong;  //  = 0xfe1b
                case 28: goto NotSupportedLong; // Sizeof = 0xfe1c,
                case 29: goto NotSupportedLong;  // Refanytype = 0xfe1d,
                case 30: goto NotSupportedLong;  //  Readonly_ = 0xfe1e,
            }

            Prefixref: goto Read;

            NotImplemented: throw new NotImplementedException($"OpCode {op} is not implemented");
            NotImplementedLong: throw new NotImplementedException($"OpCode { (short)((ushort)(0xFE00) | op)} is not implemented");

            NotSupported: throw new NotImplementedException($"OpCode { OpCodeLookup.GetILOpcode(op)} is not supported");
            NotSupportedLong: throw new NotImplementedException($"OpCode { OpCodeLookup.GetILOpcode((short)((ushort)(0xFE00) | op))} is not supported");



            RetResult: return (stack.Count > 0) ? stack.Pop() : null;

        }


        //idx = stream.Position; goto READ;
        ////var code = OpCodeLookup.GetILOpcode(first);
        //if (opCode == Prefix1)
        //{
        //    opCode <<= 8;
        //    opCode += stream.ReadByte();
        //}
        //// either pass stack, stream, resolver, args, locals to each action
        //// test performance of dictionary 






        //Also consider
        //or build each action so it has a reference? 
        //   this approach requires either actions are declared locally within the method (hence initialized within the method);
        //instructions[opCode]();

        //  or the action constructor some how has a reference to stack, stream, resolver, args, locals
        // only efficient way to do this would be with static handlers. However, that's not thread safe.
        //    Eg: var inlineInstruction= new OpCodeHandler(); 
        //      and then when the method is called set a field/property for the other:
        //          OpCodeHandlers.ForEach(x=> setRefs(stack,stream,resolver,args,locals));
        // => opCodeHandlers[opCode].Execute();

        //No need for jump label. Just keep reading the stream. Instructions can perform jumps by setting the Stream.Position;
        //Next:



    }
}
