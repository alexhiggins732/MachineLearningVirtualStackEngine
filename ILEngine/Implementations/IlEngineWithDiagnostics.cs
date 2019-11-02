using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ILEngine.Implementations
{
    public class IlEngineWithDiagnostics
    {
        public void ExecuteFrame(IlStackFrameWithDiagnostics frame)
        {

            frame.Reset();
            goto Inc;
            ReadNext:
            frame.ReadNext();

            short opCodeValue = frame.Code.Value;
            switch (opCodeValue)
            {
                case (short)ILOpCodeValues.Nop: break;
                case (short)ILOpCodeValues.Ret: goto Ret;

                case (short)ILOpCodeValues.Stloc_0:
                    frame.Locals[0].Value = frame.Stack.Pop();
                    break;
                case (short)ILOpCodeValues.Stloc_1:
                    frame.Locals[1].Value = frame.Stack.Pop();
                    break;
                case (short)ILOpCodeValues.Stloc_2:
                    frame.Locals[2].Value = frame.Stack.Pop();
                    break;
                case (short)ILOpCodeValues.Stloc_3:
                    frame.Locals[3].Value = frame.Stack.Pop();
                    break;
                case unchecked((short)ILOpCodeValues.Stloc):
                    frame.Locals[(int)frame.Current.Arg].Value = frame.Stack.Pop();
                    break;
                case (short)ILOpCodeValues.Stloc_S:
                    frame.Locals[(byte)frame.Current.Arg].Value = frame.Stack.Pop();
                    break;

                case (short)ILOpCodeValues.Stobj:
                    throw new NotImplementedException();
                case (short)ILOpCodeValues.Ldloc_0:
                    frame.Stack.Push(frame.Locals[0].Value);
                    break;
                case (short)ILOpCodeValues.Ldloc_1:
                    frame.Stack.Push(frame.Locals[1].Value);
                    break;
                case (short)ILOpCodeValues.Ldloc_2:
                    frame.Stack.Push(frame.Locals[2].Value);
                    break;
                case (short)ILOpCodeValues.Ldloc_3:
                    frame.Stack.Push(frame.Locals[3].Value);
                    break;
                case unchecked((short)ILOpCodeValues.Ldloc):
                    frame.Stack.Push(frame.Locals[(int)frame.Current.Arg].Value);
                    break;
                case (short)ILOpCodeValues.Ldloc_S:
                    frame.Stack.Push(frame.Locals[(byte)frame.Current.Arg].Value);
                    break;
                case unchecked((short)ILOpCodeValues.Ldloca):
                    frame.Stack.Push(frame.Locals[(int)frame.Current.Arg]);
                    break;
                case (short)ILOpCodeValues.Ldloca_S:
                    frame.Stack.Push(frame.Locals[(byte)frame.Current.Arg]);
                    break;
                case (short)ILOpCodeValues.Ldarg_0:
                    frame.Stack.Push(frame.Args[0]);
                    break;
                case (short)ILOpCodeValues.Ldarg_1:
                    frame.Stack.Push(frame.Args[1]);
                    break;
                case (short)ILOpCodeValues.Ldarg_2:
                    frame.Stack.Push(frame.Args[2]);
                    break;
                case (short)ILOpCodeValues.Ldarg_3:
                    frame.Stack.Push(frame.Args[3]);
                    break;
                case unchecked((short)ILOpCodeValues.Ldarg):
                    frame.Stack.Push(frame.Args[(int)frame.Current.Arg]);
                    break;
                case unchecked((short)ILOpCodeValues.Ldarg_S):
                    frame.Stack.Push(frame.Args[(byte)frame.Current.Arg]);
                    break;
                case (short)ILOpCodeValues.Ldarga_S:
                    frame.Stack.Push(frame.Args[(byte)frame.Current.Arg]);
                    break;
                case unchecked((short)ILOpCodeValues.Ldarga):
                    frame.Stack.Push(frame.Args[(int)frame.Current.Arg]);
                    break;

                case (short)ILOpCodeValues.Ldc_I4:
                    frame.Stack.Push((int)frame.Current.Arg);
                    break;

                case (short)ILOpCodeValues.Ldc_I4_0:
                    frame.Stack.Push(0);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_1:
                    frame.Stack.Push(1);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_2:
                    frame.Stack.Push(2);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_3:
                    frame.Stack.Push(3);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_4:
                    frame.Stack.Push(4);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_5:
                    frame.Stack.Push(5);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_6:
                    frame.Stack.Push(6);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_7:
                    frame.Stack.Push(7);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_8:
                    frame.Stack.Push(8);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_S:
                    frame.Stack.Push(Convert.ToInt32(frame.Current.Arg));
                    break;
                case (short)ILOpCodeValues.Ldc_I4_M1:
                    frame.Stack.Push(-1);
                    break;
                case (short)ILOpCodeValues.Ldc_I8:
                    frame.Stack.Push(Convert.ToInt64(frame.Current.Arg));
                    break;
                case (short)ILOpCodeValues.Ldc_R4:
                    frame.Stack.Push(Convert.ToSingle(frame.Current.Arg));
                    break;
                case (short)ILOpCodeValues.Ldc_R8:
                    frame.Stack.Push(Convert.ToDouble(frame.Current.Arg));
                    break;
                case (short)ILOpCodeValues.Box: // 140: //box
                    frame.Stack.Push(((object)frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Ckfinite: // 195: //ckfinite
                    var ckval = frame.Stack.Pop(); frame.Stack.Push((ckval is float ? float.IsInfinity((float)ckval) : double.IsInfinity((double)ckval)));
                    break;
                case (short)ILOpCodeValues.Conv_I1:// 103: //conv.i1
                    frame.Stack.Push(unchecked(Convert.ToSByte(frame.Stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Conv_I2: //104: //conv.i2
                    frame.Stack.Push(unchecked(Convert.ToInt16(frame.Stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Conv_I4: //105: //conv.i4
                    frame.Stack.Push(unchecked(Convert.ToInt32(frame.Stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Conv_I8: //106: //conv.i8
                    frame.Stack.Push(unchecked(Convert.ToInt64(frame.Stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I1: //179: //conv.ovf.i1
                    frame.Stack.Push(Convert.ToSByte(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I1_Un: //130: //conv.ovf.i1.un
                    frame.Stack.Push(Convert.ToSByte(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I2: //181: //conv.ovf.i2
                    frame.Stack.Push(Convert.ToInt16(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I2_Un: //131: //conv.ovf.i2.un
                    frame.Stack.Push(Convert.ToInt16(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I4: //183: //conv.ovf.i4
                    frame.Stack.Push(Convert.ToInt32(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I4_Un: //132: //conv.ovf.i4.un
                    frame.Stack.Push(Convert.ToInt32(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I8: //185: //conv.ovf.i8
                    frame.Stack.Push(Convert.ToInt64(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I8_Un: //133: //conv.ovf.i8.un
                    frame.Stack.Push(Convert.ToInt64(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U1: //180: //conv.ovf.u1
                    frame.Stack.Push(Convert.ToByte(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U1_Un: //134: //conv.ovf.u1.un
                    frame.Stack.Push(Convert.ToByte(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U2: //182: //conv.ovf.u2
                    frame.Stack.Push(Convert.ToUInt16(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U2_Un: //135: //conv.ovf.u2.un
                    frame.Stack.Push(Convert.ToUInt16(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U4: //184: //conv.ovf.u4
                    frame.Stack.Push(Convert.ToUInt32(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U4_Un: //136: //conv.ovf.u4.un
                    frame.Stack.Push(Convert.ToUInt32(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U8: //186: //conv.ovf.u8
                    frame.Stack.Push(Convert.ToUInt64(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U8_Un: //137: //conv.ovf.u8.un
                    frame.Stack.Push(Convert.ToUInt64(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_R4: //107: //conv.r4
                    frame.Stack.Push(Convert.ToSingle(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_R8: //108: //conv.r8
                    frame.Stack.Push(Convert.ToDouble(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_R_Un: //118: //conv.r.un
                    frame.Stack.Push(Convert.ToSingle(frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_U1: //210: //conv.u1
                    frame.Stack.Push(unchecked(Convert.ToByte(frame.Stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Conv_U2: //209: //conv.u2
                    frame.Stack.Push(unchecked(Convert.ToUInt16(frame.Stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Conv_U4: //109: //conv.u4
                    frame.Stack.Push(unchecked(Convert.ToUInt32(frame.Stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Conv_U8: //110: //conv.u8
                    frame.Stack.Push(unchecked(Convert.ToUInt64(frame.Stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Neg: //101: //neg
                    frame.Stack.Push(-((dynamic)frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Newarr: //141: //newarr
                    frame.Stack.Push(Array.CreateInstance(((frame.Current.Arg is int) ? frame.ResolveTypeToken((int)frame.Current.Arg) : (Type)frame.Current.Arg), (int)frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Not: //102: //not
                    frame.Stack.Push(!((dynamic)frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Pop: //38: //pop
                    frame.Stack.Pop();// no push
                    break;
                case (short)ILOpCodeValues.Shl: //98: //shl
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) << ((int)frame.Current.Arg));
                    break;
                case (short)ILOpCodeValues.Shr: //99: //shr
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) >> ((int)frame.Current.Arg));
                    break;
                case (short)ILOpCodeValues.Shr_Un: //100: //shr.un
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) >> ((int)frame.Current.Arg));
                    break;
                case (short)ILOpCodeValues.Unbox: //121: //unbox
                    frame.Stack.Push(Convert.ChangeType(frame.Stack.Pop(), frame.ResolveTypeToken((int)frame.Current.Arg)));
                    break;
                case (short)ILOpCodeValues.Unbox_Any: //165: //unbox.any
                    frame.Stack.Push(Convert.ChangeType(frame.Stack.Pop(), frame.ResolveTypeToken((int)frame.Current.Arg)));
                    break;
                case (short)ILOpCodeValues.Add: //: //add
                    frame.Stack.Push(unchecked(((dynamic)frame.Stack.Pop()) + ((dynamic)frame.Stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Add_Ovf: // 214: //add.ovf
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) + ((dynamic)frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Add_Ovf_Un: // 215: //add.ovf.un
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) + ((dynamic)frame.Stack.Pop()));
                    break;
                case (short)ILOpCodeValues.And: //95: //and
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) & ((dynamic)frame.Stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Ceq): //: //ceq
                    {
                        var opa = frame.Stack.Pop();
                        var opb = frame.Stack.Pop();

                        if (opa is IConvertible && opb is IConvertible)
                        {
                            var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
                            frame.Stack.Push(compc == 0 ? 1 : 0);
                        }
                        else
                        {
                            frame.Stack.Push(Convert.ToInt32(((dynamic)opa) == ((dynamic)opb)));
                        }
                    }

                    break;
                case unchecked((short)ILOpCodeValues.Cgt): // -510: //cgt
                    frame.Stack.Push(Convert.ToInt32(((dynamic)frame.Stack.Pop()) < ((dynamic)frame.Stack.Pop())));
                    break;
                case unchecked((short)ILOpCodeValues.Cgt_Un) - 509: //cgt.un
                    frame.Stack.Push(Convert.ToInt32(((dynamic)frame.Stack.Pop()) < ((dynamic)frame.Stack.Pop())));
                    break;
                case unchecked((short)ILOpCodeValues.Clt): // -508: //clt
                    frame.Stack.Push(Convert.ToInt32(((dynamic)frame.Stack.Pop()) > ((dynamic)frame.Stack.Pop())));//either swap pop order or sign
                    break;
                case unchecked((short)ILOpCodeValues.Clt_Un): // -507: //clt.un
                    frame.Stack.Push(Convert.ToInt32(((dynamic)frame.Stack.Pop()) > ((dynamic)frame.Stack.Pop())));
                    break;
                case unchecked((short)ILOpCodeValues.Div): // 91: //div
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) / ((dynamic)frame.Stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Div_Un): // 92: //div.un
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) / ((dynamic)frame.Stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Mul): // 90: //mul
                    frame.Stack.Push(unchecked(((dynamic)frame.Stack.Pop()) * ((dynamic)frame.Stack.Pop())));
                    break;
                case unchecked((short)ILOpCodeValues.Mul_Ovf): // 216: //mul.ovf
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) * ((dynamic)frame.Stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Mul_Ovf_Un): // 217: //mul.ovf.un
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) * ((dynamic)frame.Stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Or): // 96: //or
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) | ((dynamic)frame.Stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Rem): // 93: //rem
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) % ((dynamic)frame.Stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Rem_Un): // 94: //rem.un
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) % ((dynamic)frame.Stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Sub): // 89: //sub
                    frame.Stack.Push(unchecked(((dynamic)frame.Stack.Pop()) - ((dynamic)frame.Stack.Pop())));
                    break;
                case unchecked((short)ILOpCodeValues.Sub_Ovf): // 218: //sub.ovf
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) - ((dynamic)frame.Stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Sub_Ovf_Un): // 219: //sub.ovf.un
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) - ((dynamic)frame.Stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Xor): // 97: //xor
                    frame.Stack.Push(((dynamic)frame.Stack.Pop()) ^ ((dynamic)frame.Stack.Pop()));
                    break;

                case (short)ILOpCodeValues.Ldstr:
                    {
                        if (frame.Current.Arg is string)
                            frame.Stack.Push((string)frame.Current.Arg);
                        else
                            frame.Stack.Push(frame.ResolveStringToken((int)frame.Current.Arg));
                        break;
                    }
                case (short)ILOpCodeValues.Newobj:
                    {
                        var ctor = (System.Reflection.ConstructorInfo)frame.ResolveMethodToken((int)frame.Current.Arg);
                        var ctorArgs = new object[ctor.GetParameters().Length];
                        for (var i = ctorArgs.Length - 1; i > -1; i--)
                            ctorArgs[i] = frame.Stack.Pop();
                        //var reversed = ctorArgs.Reverse().ToArray();
                        var ctorResult = ctor.Invoke(ctorArgs);
                        frame.Stack.Push(ctorResult);
                        break;
                    }
                case (short)ILOpCodeValues.Ldfld:
                    {
                        var field = frame.ResolveFieldToken((int)frame.Current.Arg);
                        var target = frame.Stack.Pop();
                        var value = field.GetValue(target);
                        frame.Stack.Push(value);
                        break;
                    }
                case (short)ILOpCodeValues.Ldsfld:
                    {
                        var field = frame.ResolveFieldToken((int)frame.Current.Arg);
                        var value = field.GetValue(null);
                        frame.Stack.Push(value);
                        break;
                    }
                case (short)ILOpCodeValues.Stfld:
                    {
                        var field = frame.ResolveFieldToken((int)frame.Current.Arg);
                        var fo = frame.Stack.Pop();
                        var target = frame.Stack.Pop();
                        field.SetValue(target, fo);
                        //frame.Stack.Push(value);
                        break;
                    }
                case (short)ILOpCodeValues.Stsfld:
                    {
                        var field = frame.ResolveFieldToken((int)frame.Current.Arg);
                        var fo = frame.Stack.Pop();
                        //var target = frame.Stack.Pop();
                        field.SetValue(null, fo);
                        break;
                    }
                case (short)ILOpCodeValues.Ldlen:
                    {
                        var array = frame.Stack.Pop();
                        var arr = (Array)array;
                        frame.Stack.Push(arr.Length);
                        break;
                    }

                case (short)ILOpCodeValues.Stelem:
                    {
                        object el = frame.Stack.Pop();
                        int index = (int)frame.Stack.Pop();
                        var array = (Array)frame.Stack.Pop();

                        array.GetType()
                            .GetMethod("SetValue", new[] { typeof(object), typeof(int) })
                            .Invoke(array, new object[] { el, index });
                        break;
                    }
                case (short)ILOpCodeValues.Stelem_I1:
                    {
                        object el = frame.Stack.Pop();
                        int index = (int)frame.Stack.Pop();
                        var array = frame.Stack.Pop();
                        var arrType = array.GetType();
                        var arrElType = arrType.GetElementType();
                        var elType = el.GetType();
                        if (elType == arrElType) ((Array)array).SetValue(el, index);
                        else if (arrElType == typeof(sbyte)) ((sbyte[])array)[index] = (sbyte)(int)el;
                        else ((byte[])array)[index] = (byte)(int)el;
                        break;

                    }
                case (short)ILOpCodeValues.Stelem_I2:
                    {
                        object el = frame.Stack.Pop();
                        int index = (int)frame.Stack.Pop();
                        var array = frame.Stack.Pop();
                        var arrType = array.GetType();
                        var arrElType = arrType.GetElementType();
                        var elType = el.GetType();

                        if (elType == arrElType) ((Array)array).SetValue(el, index);
                        else if (arrElType == typeof(short)) ((Array)array).SetValue((short)(int)el, index);
                        else if (arrElType == typeof(short)) ((Array)array).SetValue((ushort)(int)el, index);
                        else ((Array)array).SetValue(Convert.ChangeType(el, arrElType), index);
                        break;
                    }
                case (short)ILOpCodeValues.Stelem_I4:
                    {
                        object el = frame.Stack.Pop();
                        int index = (int)frame.Stack.Pop();
                        var array = frame.Stack.Pop();
                        var arrType = array.GetType();
                        var arrElType = arrType.GetElementType();
                        var elType = el.GetType();
                        if (elType == arrElType) ((Array)array).SetValue(el, index);
                        else if (arrElType == typeof(int)) ((int[])array)[index] = (int)el;
                        else ((uint[])array)[index] = (uint)el;
                        break;
                    }
                case (short)ILOpCodeValues.Stelem_I8:
                    {
                        object el = frame.Stack.Pop();
                        int index = (int)frame.Stack.Pop();
                        var array = frame.Stack.Pop();
                        var arrType = array.GetType();
                        var arrElType = arrType.GetElementType();
                        var elType = el.GetType();
                        if (elType == arrElType) ((Array)array).SetValue(el, index);
                        else if (arrElType == typeof(long)) ((long[])array)[index] = (long)el;
                        else ((ulong[])array)[index] = (ulong)el;
                        break;
                    }
                case (short)ILOpCodeValues.Stelem_R4:
                    {
                        object el = frame.Stack.Pop();
                        int index = (int)frame.Stack.Pop();
                        var array = frame.Stack.Pop();
                        var arrType = array.GetType();
                        var arrElType = arrType.GetElementType();
                        var elType = el.GetType();
                        if (elType == arrElType) ((Array)array).SetValue(el, index);
                        else if (arrElType == typeof(float)) ((float[])array)[index] = (float)el;
                        //else ((ulong[])array)[index] = (ulong)el;
                        break;
                    }
                case (short)ILOpCodeValues.Stelem_R8:
                    {
                        object el = frame.Stack.Pop();
                        int index = (int)frame.Stack.Pop();
                        var array = frame.Stack.Pop();
                        var arrType = array.GetType();
                        var arrElType = arrType.GetElementType();
                        var elType = el.GetType();
                        if (elType == arrElType) ((Array)array).SetValue(el, index);
                        else if (arrElType == typeof(double)) ((double[])array)[index] = (double)el;
                        //else ((ulong[])array)[index] = (ulong)el;
                        break;
                    }
                case (short)ILOpCodeValues.Stelem_Ref:
                    {

                        object val = frame.Stack.Pop();
                        int index = (int)frame.Stack.Pop();
                        var array = frame.Stack.Pop();
                        ((Array)array).SetValue(val, index);
                        break;
                    }
                case (short)ILOpCodeValues.Ldelema:
                case (short)ILOpCodeValues.Ldelem_I:
                case (short)ILOpCodeValues.Ldelem_Ref:
                    throw new NotSupportedException();

                case (short)ILOpCodeValues.Ldelem_I1:
                case (short)ILOpCodeValues.Ldelem_I2:
                case (short)ILOpCodeValues.Ldelem_I4:
                case (short)ILOpCodeValues.Ldelem_I8:

                case (short)ILOpCodeValues.Ldelem_U1:
                case (short)ILOpCodeValues.Ldelem_U2:
                    {

                        var idx = (int)frame.Stack.Pop();
                        var array = (Array)frame.Stack.Pop();
                        var val = array.GetValue(idx);
                        var target = (ushort)Convert.ToUInt32(val);
                        frame.Stack.Push(target);
                        break;
                    }

                case (short)ILOpCodeValues.Ldelem_U4:


                case (short)ILOpCodeValues.Ldelem_R4:
                case (short)ILOpCodeValues.Ldelem_R8:



                case (short)ILOpCodeValues.Conv_I:
                case (short)ILOpCodeValues.Conv_Ovf_I_Un:
                case (short)ILOpCodeValues.Conv_Ovf_I:
                    //Todo: native int operations
                    throw new NotImplementedException();
                case (short)ILOpCodeValues.Dup:
                    frame.Stack.Push(frame.Stack.Peek());
                    break;

                //TODO: Implemented scopre validation for branch: (EG, inside try, catch, finally,etc)
                case (short)ILOpCodeValues.Leave_S:
                case (short)ILOpCodeValues.Br_S: //0x2b:
                    {

                        var delta = (int)(sbyte)Convert.ToByte(frame.Current.Arg);
                        var directpos = (int)frame.Stream[frame.Position + 1].ByteIndex + delta;
                        frame.Position = frame.JumpTable[directpos];
                        goto MoveNext;

                    }
                case (short)ILOpCodeValues.Leave:
                case (short)ILOpCodeValues.Br: // 0x38: 
                    {
                        var delta = Convert.ToInt32(frame.Current.Arg);
                        var directpos = (int)frame.Stream[frame.Position + 1].ByteIndex + delta;
                        frame.Position = frame.JumpTable[directpos];
                        goto MoveNext;
                    }
                case (short)ILOpCodeValues.Beq:
                    {

                        var opa = frame.Stack.Pop();
                        var opb = frame.Stack.Pop();
                        if (opa is IConvertible && opb is IConvertible)
                        {
                            var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
                            frame.Stack.Push(compc == 0 ? 1 : 0);
                        }
                        else
                        {
                            frame.Stack.Push(Convert.ToInt32(((dynamic)opa) == ((dynamic)opb)));
                        }
                        goto case (short)ILOpCodeValues.Brtrue;
                    }
                case (short)ILOpCodeValues.Beq_S:
                    {
                        var opa = frame.Stack.Pop();
                        var opb = frame.Stack.Pop();

                        if (opa is IConvertible && opb is IConvertible)
                        {
                            var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
                            frame.Stack.Push(compc == 0 ? 1 : 0);
                        }
                        else
                        {
                            frame.Stack.Push(Convert.ToInt32(((dynamic)opa) == ((dynamic)opb)));
                        }
                        goto case (short)ILOpCodeValues.Brtrue_S;
                    }

                case (short)ILOpCodeValues.Brfalse: //‭00111001‬
                    {
                        var chk = frame.Stack.Pop();
                        int cond = Convert.ToInt32((chk is IConvertible) ? Convert.ToBoolean(chk) : Convert.ToBoolean(!(chk is null)));
                        var condtarget = -1;
                        if (cond == 0)
                        {
                            condtarget = (int)frame.Stream[frame.Position + 1].ByteIndex + Convert.ToInt32(frame.Current.Arg);
                            frame.Position = frame.JumpTable[condtarget];
                            goto MoveNext;
                        }
                        break;
                    }
                case (short)ILOpCodeValues.Brfalse_S:
                    {
                        var chk = frame.Stack.Pop();
                        int cond = Convert.ToInt32((chk is IConvertible) ? Convert.ToBoolean(chk) : Convert.ToBoolean(!(chk is null)));
                        var condtarget = -1;
                        if (cond == 0)
                        {
                            condtarget = (int)frame.Stream[frame.Position + 1].ByteIndex + (int)(sbyte)Convert.ToByte(frame.Current.Arg);
                            frame.Position = frame.JumpTable[condtarget];
                            goto MoveNext;
                        }
                        break;
                    }
                case (short)ILOpCodeValues.Brtrue:
                    {
                        var chk = frame.Stack.Pop();
                        int cond = Convert.ToInt32((chk is IConvertible) ? Convert.ToBoolean(chk) : Convert.ToBoolean(!(chk is null)));
                        var condtarget = -1;
                        if (cond == 1)
                        {
                            condtarget = (int)frame.Stream[frame.Position + 1].ByteIndex + Convert.ToInt32(frame.Current.Arg);
                            frame.Position = frame.JumpTable[condtarget];
                            goto MoveNext;
                        }
                        break;
                    }
                case (short)ILOpCodeValues.Brtrue_S:
                    {
                        var chk = frame.Stack.Pop();
                        int cond = Convert.ToInt32((chk is IConvertible) ? Convert.ToBoolean(chk) : Convert.ToBoolean(!(chk is null)));
                        var condtarget = -1;
                        if (cond == 1)
                        {
                            condtarget = (int)frame.Stream[frame.Position + 1].ByteIndex + (int)(sbyte)Convert.ToByte(frame.Current.Arg);
                            frame.Position = frame.JumpTable[condtarget];
                            goto MoveNext;
                        }
                        break;
                    }
                case (short)ILOpCodeValues.Call:
                case (short)ILOpCodeValues.Callvirt:
                    System.Reflection.MethodBase method = null;
                    object resolved = null;
                    if (frame.Current.Arg is System.Reflection.MethodInfo)
                        method = (System.Reflection.MethodInfo)frame.Current.Arg;
                    else
                        resolved = frame.ResolveMethodToken((int)frame.Current.Arg);


                    if (resolved is ConstructorInfo)
                    {
                        method = (ConstructorInfo)resolved;
                    }
                    else
                    {
                        method = (System.Reflection.MethodInfo)resolved;
                    }

                    var parameters = method.GetParameters();
                    var methodArgs = new object[parameters.Length];

                    for (var i = methodArgs.Length - 1; i >= 0; i--)
                    {
                        var val = frame.Stack.Pop();
                        if (val is ILVariable)
                            methodArgs[i] = ((ILVariable)(val)).Value;
                        else
                            methodArgs[i] = val;
                    }

                    object methodTarget = null;
                    if (!method.IsStatic)
                        methodTarget = frame.Stack.Pop();
                    if (methodTarget is ILVariable)
                        methodTarget = ((ILVariable)methodTarget).Value;
                    //var t = default(RuntimeTypeHandle);
                    //var t1 = default(ArgIterator);
                    //var tobject = new object[] { t, t };
                    //var del = Delegate.CreateDelegate()
                    //((ConstructorInfo)method).Invoke( new object[]{ t});
                    for (var i = methodArgs.Length - 1; i >= 0; i--)
                        if (methodArgs[i] is IConvertible)
                            methodArgs[i] = Convert.ChangeType(methodArgs[i], parameters[i].ParameterType);

                    //if the current method is invoking another method then convert the arguments for the inner method.
                    if (methodTarget is MethodBase && methodArgs.Length == 2 && methodArgs[1] is Array)
                    {
                        var invokeArgs = (Array)methodArgs[1];
                        var invokeParameters = ((MethodInfo)methodTarget).GetParameters();
                        for (var i = invokeArgs.Length - 1; i >= 0; i--)
                        {
                            var arg = invokeArgs.GetValue(i);
                            if (arg is IConvertible)
                                invokeArgs.SetValue(Convert.ChangeType(arg, invokeParameters[i].ParameterType), i);
                        }
                        //if (invokeArgs.GetValue(i) is IConvertible)
                        //    invokeArgs.SetValue(Convert.ChangeType(invokeArgs[i], invokeParameters[i].ParameterType));
                    }

                    // Roadblock here: Int.CompareTo(object value) -> argument value must be of type int but there is no way to programatically determine the expected destination type.

                    var methodresult = (method is MethodInfo) ? method.Invoke(methodTarget, methodArgs) : ((ConstructorInfo)method).Invoke(methodArgs);
                    if (frame.Code.StackBehaviourPush == StackBehaviour.Varpush)
                        if ((method as MethodInfo)?.ReturnType != typeof(void) || method.IsConstructor)
                            frame.Stack.Push(methodresult);
                    break;
                case (short)ILOpCodeValues.Ldnull:
                    frame.Stack.Push(null);
                    break;
                case unchecked((short)ILOpCodeValues.Ldftn):
                    var ftnToken = (int)frame.Current.Arg;
                    var ftnMethod = frame.ResolveMethodToken(ftnToken);
                    frame.Stack.Push(ftnMethod.MethodHandle.GetFunctionPointer());
                    break;
                case unchecked((short)ILOpCodeValues.Initobj):

                    var newObj = Activator.CreateInstance(frame.ResolveTypeToken((int)frame.Current.Arg));
                    var inst = frame.Stack.Pop();
                    if (inst is ILVariable ilvar)
                    {
                        ilvar.Value = newObj;
                        frame.Locals[ilvar.Index] = ilvar;
                    }
                    else
                        inst = newObj;

                    break;
                case (short)ILOpCodeValues.Ldtoken:
                    var metaToken = (int)frame.Current.Arg;
                    var memToken = frame.ResolveMemberToken(metaToken);
                    var tokenType = memToken.GetType();


                    switch (tokenType.Name)
                    {
                        case "RtFieldInfo":
                            {
                                var fieldInfo = frame.ResolveFieldToken(metaToken);
                                var handle = (FieldInfo)fieldInfo;
                                frame.Stack.Push(handle.FieldHandle);
                            }
                            break;
                        case "RuntimeType":
                            {
                                var type = frame.ResolveTypeToken(metaToken);
                                var handle = (Type)type;
                                frame.Stack.Push(handle.TypeHandle);
                            }

                            break;
                        default:

                            throw new NotImplementedException();
                    }


                    break;
                case (short)ILOpCodeValues.Castclass:

                    var targetClassToken = (int)frame.Current.Arg;
                    var targetType = frame.ResolveTypeToken(targetClassToken);

                    var listType = typeof(List<>).MakeGenericType(new[] { targetType });
                    var instance = Activator.CreateInstance(listType);
                    var prop = listType.GetProperty("Item");
                    var src = frame.Stack.Pop();


                    var add = listType.GetMethod("Add");
                    add.Invoke(instance, new[] { src });
                    var get = listType.GetMethod("get_Item");
                    var listresult = get.Invoke(instance, new object[] { 0 });
                    frame.Stack.Push(listresult);

                    break;
                case (short)ILOpCodeValues.Break:
                    if (frame.TriggerBreak) System.Diagnostics.Debugger.Break();
                    break;

                default:
                    frame.Exception = new NotImplementedException($"{nameof(OpCode)} {frame.Code} is not implemented");
                    goto Ret;

            }

            Inc:
            frame.Inc();
            MoveNext:
            if (frame.MoveNext())
                goto ReadNext;


            Ret:
            frame.ReturnResult = (frame.Stack.Count > 0 && frame.Exception == null) ? frame.Stack.Pop() : null;

        }
    }
}
