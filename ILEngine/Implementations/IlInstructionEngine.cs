using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection;

namespace ILEngine
{
 
    /// <summary>
    /// IL
    /// </summary>
    public class IlInstructionEngine
    {

        public bool TriggerBreak = false;
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

            var methodBytes = method.GetMethodBody().GetILAsByteArray();
            var ilStream = IlInstructionReader.FromByteCode(method.GetMethodBody().GetILAsByteArray());
            var ilStreamCode = IlInstructionReader.ToString(ilStream);


            var resolver = new IlInstructionResolver(method);
            var locals = new ILVariable[localVariables.Count];
            for (var i = 0; i < locals.Length; i++)
                locals[localVariables[i].LocalIndex].CopyFrom(localVariables[i], body.InitLocals);


            bool doTrace = bool.Parse(bool.FalseString);
            if (doTrace)
            {
                System.Diagnostics.Trace.WriteLine(method);
                if (args != null && args.Length > 0)
                {
                    System.Diagnostics.Trace.WriteLine(string.Join("\r\n", Enumerable.Range(0, args.Length).Select(i => $"arg[{i}]: {args[i]}")));
                }
                if (locals != null && locals.Length > 0)
                {
                    System.Diagnostics.Trace.WriteLine(string.Join("\r\n", Enumerable.Range(0, locals.Length).Select(i => $"loc[{i}]: {locals[i]}")));
                }

                System.Diagnostics.Trace.WriteLine(ilStreamCode);
            }
            return ExecuteTyped(ilStream, resolver, args.ToArray(), locals);
        }
        object getInnerHandle(__arglist)
        {
            RuntimeArgumentHandle handle = __arglist;
            ArgIterator args = new ArgIterator(handle);
            var mi = typeof(ArgIterator).GetMembers();
            var meth = typeof(ArgIterator).GetMethod("GetNextArg", Type.EmptyTypes);
            var il = meth.GetMethodBody().GetILAsByteArray();
            //ExecuteTyped(meth, __arglist(handle));
            var t = args.GetNextArgType();
            TypedReference tr = args.GetNextArg();
            var targetType = TypedReference.GetTargetType(tr);
            var typedObject = TypedReference.ToObject(tr);
            return typedObject;
        }
        object getArglistHandle(__arglist)
        {
            var handle = __arglist;
            var members = typeof(RuntimeTypeHandle).GetMembers();
            var value = typeof(RuntimeTypeHandle).GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);

            return getInnerHandle(__arglist(handle));
        }
        public void GetArgIterator(object a, __arglist)
        {

            RuntimeArgumentHandle handle = __arglist;
            GetArgIterator(1, __arglist(handle));
            ArgIterator args = new ArgIterator(handle);
            TypedReference tr = args.GetNextArg();
            RuntimeTypeHandle runtimeTypeHandle = TypedReference.TargetTypeToken(tr);
            ModuleHandle mh = runtimeTypeHandle.GetModuleHandle();
            IntPtr mhPtr = runtimeTypeHandle.Value;

            var targetType = TypedReference.GetTargetType(tr);
            var typedObject = TypedReference.ToObject(tr);



            args.End();
        }

        public T ExecuteTyped<T>(List<IlInstruction> stream, IIlInstructionResolver resolver = null, object[] args = null, ILVariable[] locals = null)
        {
            var result = ExecuteTyped(stream, resolver, args, locals);
            return (T)result;
        }
        //Execution using strongly typed enumerations. Working implementation will be converted native integer values
        //  to allow native MSIL instruction
        public object ExecuteTyped(List<IlInstruction> stream, IIlInstructionResolver resolver = null, object[] args = null, ILVariable[] locals = null)
        {
            //getArglistHandle(__arglist(1, 2, "s", 8)); <-- can't support dynamic/reflection based invocation
            //  probably best we can do is detect varargs, convert them to object[] and reinterprit msil.
            //  for now will just have to throw excpetion.


            if (resolver is null) resolver = IlInstructionResolver.ExecutingAssemblyResolver;

            var resolveField = resolver.ResolveFieldToken;
            var resolveMember = resolver.ResolveMemberToken;
            var resolveMethod = resolver.ResolveMethodToken;
            var resolveSignature = resolver.ResolveSignatureToken;
            var resolveString = resolver.ResolveStringToken;
            var resolveType = resolver.ResolveTypeToken;



            var stack = new Stack<object>();
            IlInstruction current;
            OpCode code;
            int pos = -1;
            Dictionary<int, int> jmptable = stream.ToDictionary(x => (int)x.ByteIndex, x => ++pos);
            pos = -1;
            goto Inc;

            ReadNext:
            current = stream[pos];
            code = current.OpCode;

            short opCodeValue = code.Value;
            switch (opCodeValue)
            {
                case (short)ILOpCodeValues.Nop: break;
                case (short)ILOpCodeValues.Ret: goto Ret;

                case (short)ILOpCodeValues.Stloc_0:
                    locals[0].Value = stack.Pop();
                    break;
                case (short)ILOpCodeValues.Stloc_1:
                    locals[1].Value = stack.Pop();
                    break;
                case (short)ILOpCodeValues.Stloc_2:
                    locals[2].Value = stack.Pop();
                    break;
                case (short)ILOpCodeValues.Stloc_3:
                    locals[3].Value = stack.Pop();
                    break;
                case unchecked((short)ILOpCodeValues.Stloc):
                    locals[(int)current.Arg].Value = stack.Pop();
                    break;
                case (short)ILOpCodeValues.Stloc_S:
                    locals[(byte)current.Arg].Value = stack.Pop();
                    break;

                case (short)ILOpCodeValues.Stobj:
                    throw new NotImplementedException();
                case (short)ILOpCodeValues.Ldloc_0:
                    stack.Push(locals[0].Value);
                    break;
                case (short)ILOpCodeValues.Ldloc_1:
                    stack.Push(locals[1].Value);
                    break;
                case (short)ILOpCodeValues.Ldloc_2:
                    stack.Push(locals[2].Value);
                    break;
                case (short)ILOpCodeValues.Ldloc_3:
                    stack.Push(locals[3].Value);
                    break;
                case unchecked((short)ILOpCodeValues.Ldloc):
                    stack.Push(locals[(int)current.Arg].Value);
                    break;
                case (short)ILOpCodeValues.Ldloc_S:
                    stack.Push(locals[(byte)current.Arg].Value);
                    break;
                case unchecked((short)ILOpCodeValues.Ldloca):
                    stack.Push(locals[(int)current.Arg]);
                    break;
                case (short)ILOpCodeValues.Ldloca_S:
                    stack.Push(locals[(byte)current.Arg]);
                    break;
                case (short)ILOpCodeValues.Ldarg_0:
                    stack.Push(args[0]);
                    break;
                case (short)ILOpCodeValues.Ldarg_1:
                    stack.Push(args[1]);
                    break;
                case (short)ILOpCodeValues.Ldarg_2:
                    stack.Push(args[2]);
                    break;
                case (short)ILOpCodeValues.Ldarg_3:
                    stack.Push(args[3]);
                    break;
                case unchecked((short)ILOpCodeValues.Ldarg):
                    stack.Push(args[(int)current.Arg]);
                    break;
                case unchecked((short)ILOpCodeValues.Ldarg_S):
                    stack.Push(args[(byte)current.Arg]);
                    break;
                case (short)ILOpCodeValues.Ldarga_S:
                    stack.Push(args[(byte)current.Arg]);
                    break;
                case unchecked((short)ILOpCodeValues.Ldarga):
                    stack.Push(args[(int)current.Arg]);
                    break;

                case (short)ILOpCodeValues.Ldc_I4:
                    stack.Push((int)current.Arg);
                    break;

                case (short)ILOpCodeValues.Ldc_I4_0:
                    stack.Push(0);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_1:
                    stack.Push(1);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_2:
                    stack.Push(2);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_3:
                    stack.Push(3);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_4:
                    stack.Push(4);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_5:
                    stack.Push(5);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_6:
                    stack.Push(6);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_7:
                    stack.Push(7);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_8:
                    stack.Push(8);
                    break;
                case (short)ILOpCodeValues.Ldc_I4_S:
                    stack.Push(Convert.ToInt32(current.Arg));
                    break;
                case (short)ILOpCodeValues.Ldc_I4_M1:
                    stack.Push(-1);
                    break;
                case (short)ILOpCodeValues.Ldc_I8:
                    stack.Push(Convert.ToInt64(current.Arg));
                    break;
                case (short)ILOpCodeValues.Ldc_R4:
                    stack.Push(Convert.ToSingle(current.Arg));
                    break;
                case (short)ILOpCodeValues.Ldc_R8:
                    stack.Push(Convert.ToDouble(current.Arg));
                    break;
                case (short)ILOpCodeValues.Box: // 140: //box
                    stack.Push(((object)stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Ckfinite: // 195: //ckfinite
                    var ckval = stack.Pop(); stack.Push((ckval is float ? float.IsInfinity((float)ckval) : double.IsInfinity((double)ckval)));
                    break;
                case (short)ILOpCodeValues.Conv_I1:// 103: //conv.i1
                    stack.Push(unchecked(Convert.ToSByte(stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Conv_I2: //104: //conv.i2
                    stack.Push(unchecked(Convert.ToInt16(stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Conv_I4: //105: //conv.i4
                    stack.Push(unchecked(Convert.ToInt32(stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Conv_I8: //106: //conv.i8
                    stack.Push(unchecked(Convert.ToInt64(stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I1: //179: //conv.ovf.i1
                    stack.Push(Convert.ToSByte(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I1_Un: //130: //conv.ovf.i1.un
                    stack.Push(Convert.ToSByte(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I2: //181: //conv.ovf.i2
                    stack.Push(Convert.ToInt16(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I2_Un: //131: //conv.ovf.i2.un
                    stack.Push(Convert.ToInt16(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I4: //183: //conv.ovf.i4
                    stack.Push(Convert.ToInt32(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I4_Un: //132: //conv.ovf.i4.un
                    stack.Push(Convert.ToInt32(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I8: //185: //conv.ovf.i8
                    stack.Push(Convert.ToInt64(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_I8_Un: //133: //conv.ovf.i8.un
                    stack.Push(Convert.ToInt64(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U1: //180: //conv.ovf.u1
                    stack.Push(Convert.ToByte(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U1_Un: //134: //conv.ovf.u1.un
                    stack.Push(Convert.ToByte(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U2: //182: //conv.ovf.u2
                    stack.Push(Convert.ToUInt16(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U2_Un: //135: //conv.ovf.u2.un
                    stack.Push(Convert.ToUInt16(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U4: //184: //conv.ovf.u4
                    stack.Push(Convert.ToUInt32(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U4_Un: //136: //conv.ovf.u4.un
                    stack.Push(Convert.ToUInt32(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U8: //186: //conv.ovf.u8
                    stack.Push(Convert.ToUInt64(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_Ovf_U8_Un: //137: //conv.ovf.u8.un
                    stack.Push(Convert.ToUInt64(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_R4: //107: //conv.r4
                    stack.Push(Convert.ToSingle(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_R8: //108: //conv.r8
                    stack.Push(Convert.ToDouble(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_R_Un: //118: //conv.r.un
                    stack.Push(Convert.ToSingle(stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Conv_U1: //210: //conv.u1
                    stack.Push(unchecked(Convert.ToByte(stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Conv_U2: //209: //conv.u2
                    stack.Push(unchecked(Convert.ToUInt16(stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Conv_U4: //109: //conv.u4
                    stack.Push(unchecked(Convert.ToUInt32(stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Conv_U8: //110: //conv.u8
                    stack.Push(unchecked(Convert.ToUInt64(stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Neg: //101: //neg
                    stack.Push(-((dynamic)stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Newarr: //141: //newarr
                    stack.Push(Array.CreateInstance(((current.Arg is int) ? resolveType((int)current.Arg) : (Type)current.Arg), (int)stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Not: //102: //not
                    stack.Push(!((dynamic)stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Pop: //38: //pop
                    stack.Pop();// no push
                    break;
                case (short)ILOpCodeValues.Shl: //98: //shl
                    stack.Push(((dynamic)stack.Pop()) << ((int)current.Arg));
                    break;
                case (short)ILOpCodeValues.Shr: //99: //shr
                    stack.Push(((dynamic)stack.Pop()) >> ((int)current.Arg));
                    break;
                case (short)ILOpCodeValues.Shr_Un: //100: //shr.un
                    stack.Push(((dynamic)stack.Pop()) >> ((int)current.Arg));
                    break;
                case (short)ILOpCodeValues.Unbox: //121: //unbox
                    stack.Push(Convert.ChangeType(stack.Pop(), resolveType((int)current.Arg)));
                    break;
                case (short)ILOpCodeValues.Unbox_Any: //165: //unbox.any
                    stack.Push(Convert.ChangeType(stack.Pop(), resolveType((int)current.Arg)));
                    break;
                case (short)ILOpCodeValues.Add: //: //add
                    stack.Push(unchecked(((dynamic)stack.Pop()) + ((dynamic)stack.Pop())));
                    break;
                case (short)ILOpCodeValues.Add_Ovf: // 214: //add.ovf
                    stack.Push(((dynamic)stack.Pop()) + ((dynamic)stack.Pop()));
                    break;
                case (short)ILOpCodeValues.Add_Ovf_Un: // 215: //add.ovf.un
                    stack.Push(((dynamic)stack.Pop()) + ((dynamic)stack.Pop()));
                    break;
                case (short)ILOpCodeValues.And: //95: //and
                    stack.Push(((dynamic)stack.Pop()) & ((dynamic)stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Ceq): //: //ceq
                    {
                        var opa = stack.Pop();
                        var opb = stack.Pop();

                        if (opa is IConvertible && opb is IConvertible)
                        {
                            var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
                            stack.Push(compc == 0 ? 1 : 0);
                        }
                        else
                        {
                            stack.Push(Convert.ToInt32(((dynamic)opa) == ((dynamic)opb)));
                        }
                    }

                    break;
                case unchecked((short)ILOpCodeValues.Cgt): // -510: //cgt
                    stack.Push(Convert.ToInt32(((dynamic)stack.Pop()) < ((dynamic)stack.Pop())));
                    break;
                case unchecked((short)ILOpCodeValues.Cgt_Un) - 509: //cgt.un
                    stack.Push(Convert.ToInt32(((dynamic)stack.Pop()) < ((dynamic)stack.Pop())));
                    break;
                case unchecked((short)ILOpCodeValues.Clt): // -508: //clt
                    stack.Push(Convert.ToInt32(((dynamic)stack.Pop()) > ((dynamic)stack.Pop())));//either swap pop order or sign
                    break;
                case unchecked((short)ILOpCodeValues.Clt_Un): // -507: //clt.un
                    stack.Push(Convert.ToInt32(((dynamic)stack.Pop()) > ((dynamic)stack.Pop())));
                    break;
                case unchecked((short)ILOpCodeValues.Div): // 91: //div
                    stack.Push(((dynamic)stack.Pop()) / ((dynamic)stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Div_Un): // 92: //div.un
                    stack.Push(((dynamic)stack.Pop()) / ((dynamic)stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Mul): // 90: //mul
                    stack.Push(unchecked(((dynamic)stack.Pop()) * ((dynamic)stack.Pop())));
                    break;
                case unchecked((short)ILOpCodeValues.Mul_Ovf): // 216: //mul.ovf
                    stack.Push(((dynamic)stack.Pop()) * ((dynamic)stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Mul_Ovf_Un): // 217: //mul.ovf.un
                    stack.Push(((dynamic)stack.Pop()) * ((dynamic)stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Or): // 96: //or
                    stack.Push(((dynamic)stack.Pop()) | ((dynamic)stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Rem): // 93: //rem
                    stack.Push(((dynamic)stack.Pop()) % ((dynamic)stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Rem_Un): // 94: //rem.un
                    stack.Push(((dynamic)stack.Pop()) % ((dynamic)stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Sub): // 89: //sub
                    stack.Push(unchecked(((dynamic)stack.Pop()) - ((dynamic)stack.Pop())));
                    break;
                case unchecked((short)ILOpCodeValues.Sub_Ovf): // 218: //sub.ovf
                    stack.Push(((dynamic)stack.Pop()) - ((dynamic)stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Sub_Ovf_Un): // 219: //sub.ovf.un
                    stack.Push(((dynamic)stack.Pop()) - ((dynamic)stack.Pop()));
                    break;
                case unchecked((short)ILOpCodeValues.Xor): // 97: //xor
                    stack.Push(((dynamic)stack.Pop()) ^ ((dynamic)stack.Pop()));
                    break;

                case (short)ILOpCodeValues.Ldstr:
                    {
                        if (current.Arg is string)
                            stack.Push((string)current.Arg);
                        else
                            stack.Push(resolveString((int)current.Arg));
                        break;
                    }
                case (short)ILOpCodeValues.Newobj:
                    {
                        var ctor = (System.Reflection.ConstructorInfo)resolveMethod((int)current.Arg);
                        var ctorArgs = new object[ctor.GetParameters().Length];
                        for (var i = ctorArgs.Length - 1; i > -1; i--)
                            ctorArgs[i] = stack.Pop();
                        //var reversed = ctorArgs.Reverse().ToArray();
                        var ctorResult = ctor.Invoke(ctorArgs);
                        stack.Push(ctorResult);
                        break;
                    }
                case (short)ILOpCodeValues.Ldfld:
                    {
                        var field = resolveField((int)current.Arg);
                        var target = stack.Pop();
                        var value = field.GetValue(target);
                        stack.Push(value);
                        break;
                    }
                case (short)ILOpCodeValues.Ldsfld:
                    {
                        var field = resolveField((int)current.Arg);
                        var value = field.GetValue(null);
                        stack.Push(value);
                        break;
                    }
                case (short)ILOpCodeValues.Stfld:
                    {
                        var field = resolveField((int)current.Arg);
                        var fo = stack.Pop();
                        var target = stack.Pop();
                        field.SetValue(target, fo);
                        //stack.Push(value);
                        break;
                    }
                case (short)ILOpCodeValues.Stsfld:
                    {
                        var field = resolveField((int)current.Arg);
                        var fo = stack.Pop();
                        //var target = stack.Pop();
                        field.SetValue(null, fo);
                        break;
                    }
                case (short)ILOpCodeValues.Ldlen:
                    {
                        var array = stack.Pop();
                        var arr = (Array)array;
                        stack.Push(arr.Length);
                        break;
                    }

                case (short)ILOpCodeValues.Stelem:
                    {
                        object el = stack.Pop();
                        int index = (int)stack.Pop();
                        var array = (Array)stack.Pop();

                        array.GetType()
                            .GetMethod("SetValue", new[] { typeof(object), typeof(int) })
                            .Invoke(array, new object[] { el, index });
                        break;
                    }
                case (short)ILOpCodeValues.Stelem_I1:
                    {
                        object el = stack.Pop();
                        int index = (int)stack.Pop();
                        var array = stack.Pop();
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
                        object el = stack.Pop();
                        int index = (int)stack.Pop();
                        var array = stack.Pop();
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
                        object el = stack.Pop();
                        int index = (int)stack.Pop();
                        var array = stack.Pop();
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
                        object el = stack.Pop();
                        int index = (int)stack.Pop();
                        var array = stack.Pop();
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
                        object el = stack.Pop();
                        int index = (int)stack.Pop();
                        var array = stack.Pop();
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
                        object el = stack.Pop();
                        int index = (int)stack.Pop();
                        var array = stack.Pop();
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

                        object val = stack.Pop();
                        int index = (int)stack.Pop();
                        var array = stack.Pop();
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

                        var idx = (int)stack.Pop();
                        var array = (Array)stack.Pop();
                        var val = array.GetValue(idx);
                        var target = (ushort)Convert.ToUInt32(val);
                        stack.Push(target);
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
                    stack.Push(stack.Peek());
                    break;

                //TODO: Implemented scopre validation for branch: (EG, inside try, catch, finally,etc)
                case (short)ILOpCodeValues.Leave_S:
                case (short)ILOpCodeValues.Br_S: //0x2b:
                    {

                        var delta = (int)(sbyte)Convert.ToByte(current.Arg);
                        var directpos = (int)stream[pos + 1].ByteIndex + delta;
                        pos = jmptable[directpos];
                        goto MoveNext;

                    }
                case (short)ILOpCodeValues.Leave:
                case (short)ILOpCodeValues.Br: // 0x38: 
                    {
                        var delta = Convert.ToInt32(current.Arg);
                        var directpos = (int)stream[pos + 1].ByteIndex + delta;
                        pos = jmptable[directpos];
                        goto MoveNext;
                    }
                case (short)ILOpCodeValues.Beq:
                    {

                        var opa = stack.Pop();
                        var opb = stack.Pop();
                        if (opa is IConvertible && opb is IConvertible)
                        {
                            var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
                            stack.Push(compc == 0 ? 1 : 0);
                        }
                        else
                        {
                            stack.Push(Convert.ToInt32(((dynamic)opa) == ((dynamic)opb)));
                        }
                        goto case (short)ILOpCodeValues.Brtrue;
                    }
                case (short)ILOpCodeValues.Beq_S:
                    {
                        var opa = stack.Pop();
                        var opb = stack.Pop();

                        if (opa is IConvertible && opb is IConvertible)
                        {
                            var compc = ((IComparable)(opa)).CompareTo(Convert.ChangeType(opb, Convert.GetTypeCode(opa)));
                            stack.Push(compc == 0 ? 1 : 0);
                        }
                        else
                        {
                            stack.Push(Convert.ToInt32(((dynamic)opa) == ((dynamic)opb)));
                        }
                        goto case (short)ILOpCodeValues.Brtrue_S;
                    }

                case (short)ILOpCodeValues.Brfalse: //‭00111001‬
                    {
                        var chk = stack.Pop();
                        int cond = Convert.ToInt32((chk is IConvertible) ? Convert.ToBoolean(chk) : Convert.ToBoolean(!(chk is null)));
                        var condtarget = -1;
                        if (cond == 0)
                        {
                            condtarget = (int)stream[pos + 1].ByteIndex + Convert.ToInt32(current.Arg);
                            pos = jmptable[condtarget];
                            goto MoveNext;
                        }
                        break;
                    }
                case (short)ILOpCodeValues.Brfalse_S:
                    {
                        var chk = stack.Pop();
                        int cond = Convert.ToInt32((chk is IConvertible) ? Convert.ToBoolean(chk) : Convert.ToBoolean(!(chk is null)));
                        var condtarget = -1;
                        if (cond == 0)
                        {
                            condtarget = (int)stream[pos + 1].ByteIndex + (int)(sbyte)Convert.ToByte(current.Arg);
                            pos = jmptable[condtarget];
                            goto MoveNext;
                        }
                        break;
                    }
                case (short)ILOpCodeValues.Brtrue:
                    {
                        var chk = stack.Pop();
                        int cond = Convert.ToInt32((chk is IConvertible) ? Convert.ToBoolean(chk) : Convert.ToBoolean(!(chk is null)));
                        var condtarget = -1;
                        if (cond == 1)
                        {
                            condtarget = (int)stream[pos + 1].ByteIndex + Convert.ToInt32(current.Arg);
                            pos = jmptable[condtarget];
                            goto MoveNext;
                        }
                        break;
                    }
                case (short)ILOpCodeValues.Brtrue_S:
                    {
                        var chk = stack.Pop();
                        int cond = Convert.ToInt32((chk is IConvertible) ? Convert.ToBoolean(chk) : Convert.ToBoolean(!(chk is null)));
                        var condtarget = -1;
                        if (cond == 1)
                        {
                            condtarget = (int)stream[pos + 1].ByteIndex + (int)(sbyte)Convert.ToByte(current.Arg);
                            pos = jmptable[condtarget];
                            goto MoveNext;
                        }
                        break;
                    }
                case (short)ILOpCodeValues.Call:
                case (short)ILOpCodeValues.Callvirt:
                    System.Reflection.MethodBase method = null;
                    object resolved = null;
                    if (current.Arg is System.Reflection.MethodInfo)
                        method = (System.Reflection.MethodInfo)current.Arg;
                    else
                        resolved = resolveMethod((int)current.Arg);


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
                        var val = stack.Pop();
                        if (val is ILVariable)
                            methodArgs[i] = ((ILVariable)(val)).Value;
                        else
                            methodArgs[i] = val;
                    }

                    object methodTarget = null;
                    if (!method.IsStatic)
                        methodTarget = stack.Pop();
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
                    if (code.StackBehaviourPush == StackBehaviour.Varpush)
                        if ((method as MethodInfo)?.ReturnType != typeof(void) || method.IsConstructor)
                            stack.Push(methodresult);
                    break;
                case (short)ILOpCodeValues.Ldnull:
                    stack.Push(null);
                    break;
                case unchecked((short)ILOpCodeValues.Ldftn):
                    var ftnToken = (int)current.Arg;
                    var ftnMethod = resolver.ResolveMethodToken(ftnToken);
                    stack.Push(ftnMethod.MethodHandle.GetFunctionPointer());
                    break;
                case unchecked((short)ILOpCodeValues.Initobj):

                    var newObj = Activator.CreateInstance(resolver.ResolveTypeToken((int)current.Arg));
                    var inst = stack.Pop();
                    if (inst is ILVariable ilvar)
                    {
                        ilvar.Value = newObj;
                        locals[ilvar.Index] = ilvar;
                    }
                    else
                        inst = newObj;

                    break;
                case (short)ILOpCodeValues.Ldtoken:
                    var metaToken = (int)current.Arg;
                    var memToken = resolver.ResolveMemberToken(metaToken);
                    var tokenType = memToken.GetType();


                    switch (tokenType.Name)
                    {
                        case "RtFieldInfo":
                            {
                                var fieldInfo = resolver.ResolveFieldToken(metaToken);
                                var handle = (FieldInfo)fieldInfo;
                                stack.Push(handle.FieldHandle);
                            }
                            break;
                        case "RuntimeType":
                            {
                                var type = resolver.ResolveTypeToken(metaToken);
                                var handle = (Type)type;
                                stack.Push(handle.TypeHandle);
                            }

                            break;
                        default:

                            throw new NotImplementedException();
                    }


                    break;
                case (short)ILOpCodeValues.Castclass:

                    var targetClassToken = (int)current.Arg;
                    var targetType = resolver.ResolveTypeToken(targetClassToken);

                    var listType = typeof(List<>).MakeGenericType(new[] { targetType });
                    var instance = Activator.CreateInstance(listType);
                    var prop = listType.GetProperty("Item");
                    var src = stack.Pop();


                    var add = listType.GetMethod("Add");
                    add.Invoke(instance, new[] { src });
                    var get = listType.GetMethod("get_Item");
                    var listresult = get.Invoke(instance, new object[] { 0 });
                    stack.Push(listresult);

                    break;
                case (short)ILOpCodeValues.Break:
                    if(TriggerBreak) System.Diagnostics.Debugger.Break();
                    break;

                default:
                    throw new NotImplementedException($"IL Instruction not implemented. {code}");
                    var notImplemented = code;// todo throw
                    break;
            }


            Inc:
            pos++;
            MoveNext:
            if (pos < stream.Count)
            {
                goto ReadNext;
            }

            Ret:
            var result = (stack.Count > 0) ? stack.Pop() : null;
            if (TriggerBreak)
            {
                System.Diagnostics.Debug.Assert(stack.Count() == 0);
            } else
            {
                //if (stack.Count > 0) throw new InvalidProgramException("Stack is not empty {stack.Count}");
            }

            return result;

        }

        private void newObject(Type objType, ref object value)
        {
            value = Activator.CreateInstance(objType);
        }

    }
}
