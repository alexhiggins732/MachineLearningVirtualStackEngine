using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{
    public class ILEngineNativeInstructionGenerator
    {
        public static void GenerateWithPushPop()
        {
            var asmName = new AssemblyName("IlEngineNative");
            AppDomain domain = AppDomain.CurrentDomain;

            AssemblyBuilder wrapperAssembly =
                domain.DefineDynamicAssembly(asmName,
                    AssemblyBuilderAccess.RunAndSave);

            var assemblyPath = asmName.Name + ".dll";

            ModuleBuilder wrapperModule =
                wrapperAssembly.DefineDynamicModule(asmName.Name,
                   assemblyPath);

            // Define a type to contain the method.
            TypeBuilder typeBuilder =
                wrapperModule.DefineType("IlEngineNative", TypeAttributes.Public);


            //generate codes for native opcodes;

            //var t= new Stack
            var stackType = typeof(Stack<object>);
            var pushMethod = stackType.GetMethod(nameof(Stack<object>.Push), new[] { typeof(object) });
            var popMethod = stackType.GetMethod(nameof(Stack<object>.Pop), Type.EmptyTypes);

            MethodAttributes atts = MethodAttributes.Public | MethodAttributes.Static;
            MethodBuilder methodBuilder =
             typeBuilder.DefineMethod($"ExecuteNativeMsil",
                                        atts,
                                        typeof(object),
                                        new[] { typeof(object), typeof(object) });
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "stack");
            methodBuilder.DefineParameter(2, ParameterAttributes.None, "opcode");


            ILGenerator il = methodBuilder.GetILGenerator();

            List<IlEmitOpCodeInfo> ilCodes = GetCodesForPushPop(il);
            ilCodes = ilCodes.Where(x => x.OpCode == OpCodes.Add).ToList();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("\t\tshort opCodeValue = 0;");
            sb.AppendLine("\t\tswitch (opCodeValue)");
            sb.AppendLine("\t\t{");
            short opCodeValue = 0;
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Switch, ilCodes.Select(x => x.Label).ToArray());
            int switchStart = -1;
            foreach (var ilCode in ilCodes)
            {
                opCodeValue = ilCode.OpCode.Value;

                il.MarkLabel(ilCode.Label);
                var offset = il.ILOffset;
                if (switchStart == -1) switchStart = offset;
                ilCode.EmitInstructions(il, pushMethod, popMethod);
                sb.AppendLine($"\t\t\tcase {opCodeValue}: //{ilCode.OpCode}");

                sb.AppendLine($"\t\t\t\tIlEngineNative.ExecuteNativeMsil(stack, {offset - switchStart});");
                sb.AppendLine($"\t\t\t\tgoto Inc;");
            }

            sb.AppendLine($"\t\t\tdefault: break;");
            sb.AppendLine("\t\t}");
            var callerSwitchStatement = sb.ToString();

            var result = typeBuilder.CreateType();
            wrapperAssembly.Save(assemblyPath);


        }
        public static string GenerateDynamicCs()
        {
            List<IlEmitOpCodeInfo> ilCodes = GetCodesForDynamicCsDynamic();
            var sb = new StringBuilder();
            sb.AppendLine("\t\tshort opCodeValue = 0;");
            sb.AppendLine("\t\tswitch (opCodeValue)");
            sb.AppendLine("\t\t{");


            var instructionFormat = "\t\t\t\tstack.Push({0});";
            var instructionExpression = "";
            foreach (var ilCode in ilCodes)
            {
                var csFormat = ilCode.CsExpression;
                var opCodeValue = ilCode.OpCode.Value;



                if (ilCode is IlEmitUnaryOpCode)
                {
                    if (ilCode.OpCode == OpCodes.Ckfinite)
                    {
                        instructionExpression = "var ckval = stack.Pop(); stack.Push(" + string.Format(csFormat, "ckval") + ");";

                        sb.AppendLine($"\t\t\tcase {opCodeValue}: //{ilCode.OpCode}");

                        sb.AppendLine($"\t\t\t" + instructionExpression);
                        sb.AppendLine($"\t\t\t\tgoto Inc;");
                        continue;
                    }
                    else if (ilCode.OpCode == OpCodes.Pop)
                    {
                        instructionExpression = "stack.Pop()";
                    }
                    else
                    {
                        instructionExpression = string.Format(csFormat, "stack.Pop()", "current.Arg");
                    }

                }
                else
                {
                    instructionExpression = string.Format(csFormat, "stack.Pop()", "stack.Pop()");
                }
                sb.AppendLine($"\t\t\tcase {opCodeValue}: //{ilCode.OpCode}");

                sb.AppendLine(string.Format(instructionFormat, instructionExpression));
                sb.AppendLine($"\t\t\t\tgoto Inc;");
            }
            sb.AppendLine($"\t\t\t\tdefault: break;");
            sb.AppendLine("\t\t}");
            var dynamicSwitch = sb.ToString();
            return dynamicSwitch;
        }

        public static Type Generate()
        {
            var asmName = new AssemblyName("IlEngineNative");
            AppDomain domain = AppDomain.CurrentDomain;

            AssemblyBuilder wrapperAssembly =
                domain.DefineDynamicAssembly(asmName,
                    AssemblyBuilderAccess.RunAndSave);

            var assemblyPath = asmName.Name + ".dll";

            ModuleBuilder wrapperModule =
                wrapperAssembly.DefineDynamicModule(asmName.Name,
                   assemblyPath);

            // Define a type to contain the method.
            TypeBuilder typeBuilder =
                wrapperModule.DefineType("IlEngineNative", TypeAttributes.Public);




            MethodAttributes atts = MethodAttributes.Public | MethodAttributes.Static;






            List<IlEmitOpCodeInfo> ilCodes = GetCodesForMethodCall();
            ilCodes = ilCodes.Where(x => x.OpCode == OpCodes.Add).ToList();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("\t\tshort opCodeValue = 0;");
            sb.AppendLine("\t\tswitch (opCodeValue)");
            sb.AppendLine("\t\t{");
            //when type object is specified the actual value passed an I4 pointing to a memory address.
            Type[] binaryArgs = new[] { typeof(object), typeof(object) };
            Type[] unaryArgs = new[] { typeof(object), typeof(object) };
            Type[] methodArgs = null;
            string binaryCall = $"\t\t\t\tstack.Push(IlEngineNative.{{0}}(stack.Pop(), stack.Pop()));";
            string unaryCall = $"\t\t\t\tstack.Push(IlEngineNative.{{0}}(stack.Pop()));";
            string methodName = "";
            string callSignature = "";
            MethodBuilder methodBuilder = null;
            ILGenerator il = null;
            foreach (var ilCode in ilCodes)
            {

                var opCodeValue = ilCode.OpCode.Value;
                var enumValue = (ILOpCodeValues)unchecked((ushort)(ilCode.OpCode.Value));
                methodName = $"{enumValue}";


                if (ilCode is IlEmitBinaryOpCode)
                {
                    methodArgs = binaryArgs;
                    callSignature = binaryCall;
                }
                else
                {
                    methodArgs = binaryArgs;
                    callSignature = binaryCall;
                }
                methodBuilder = typeBuilder.DefineMethod(methodName,
                          atts,
                          typeof(int),
                          methodArgs);

                il = methodBuilder.GetILGenerator();

                ilCode.EmitMethodCallInstructions(il);

                sb.AppendLine($"\t\t\tcase {opCodeValue}: //{ilCode.OpCode}");
                sb.AppendLine(string.Format(callSignature, methodName));
                sb.AppendLine($"\t\t\t\tgoto Inc;");

            }

            sb.AppendLine($"\t\t\tdefault: break;");
            sb.AppendLine("\t\t}");
            var callerSwitchStatement = sb.ToString();

            var result = typeBuilder.CreateType();
            //wrapperAssembly.Save(assemblyPath);
            return result;

        }

        public static List<IlEmitOpCodeInfo> GetCodesForMethodCall()
        {
            var result = new List<IlEmitOpCodeInfo>();

            //unary
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Box); requires token argument ->  [ERROR: INVALID TOKEN 0x0228028C] 
            result.Add(new IlEmitUnaryOpCode(OpCodes.Ckfinite));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I1));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I2));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I4));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I8));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I_Un));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I1));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I1_Un));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I2));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I2_Un));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I4));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I4_Un));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I8));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I8_Un));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U_Un));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U1));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U1_Un));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U2));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U2_Un));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U4));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U4_Un));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U8));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U8_Un));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_R4));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_R8));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_R_Un));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U1));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U2));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U4));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U8));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Neg));

            // test this, if it works off the stack don't have to handle reflection based array constructor
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Newarr)); -> requires token [ERROR: INVALID TOKEN 0x0228028C] 

            result.Add(new IlEmitUnaryOpCode(OpCodes.Not));

            //can't pass arguments
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Shl));
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Shr));
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Shr_Un));

            // result.Add(new IlEmitUnaryOpCode(OpCodes.Unbox)); requires token argument
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Unbox_Any));


            //binary
            result.Add(new IlEmitBinaryOpCode(OpCodes.Add));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Add_Ovf));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Add_Ovf_Un));

            result.Add(new IlEmitBinaryOpCode(OpCodes.And));

            result.Add(new IlEmitBinaryOpCode(OpCodes.Ceq));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Cgt));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Cgt_Un));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Clt));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Clt_Un));

            result.Add(new IlEmitBinaryOpCode(OpCodes.Div));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Div_Un));



            result.Add(new IlEmitBinaryOpCode(OpCodes.Mul));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Mul_Ovf));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Mul_Ovf_Un));


            result.Add(new IlEmitBinaryOpCode(OpCodes.Or));

            //result.Add(new IlEmitBinaryOpCode(OpCodes.Pop));


            result.Add(new IlEmitBinaryOpCode(OpCodes.Rem));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Rem_Un));

            result.Add(new IlEmitBinaryOpCode(OpCodes.Sub));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Sub_Ovf));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Sub_Ovf_Un));

            result.Add(new IlEmitBinaryOpCode(OpCodes.Xor));
            return result;
        }
        public static List<IlEmitOpCodeInfo> GetCodesForPushPop(ILGenerator il)
        {
            var result = new List<IlEmitOpCodeInfo>();

            //unary
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Box, il.DefineLabel())); requires token argument ->  [ERROR: INVALID TOKEN 0x0228028C] 
            result.Add(new IlEmitUnaryOpCode(OpCodes.Ckfinite, il.DefineLabel()));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I1, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I2, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I4, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I8, il.DefineLabel()));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I_Un, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I1, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I1_Un, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I2, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I2_Un, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I4, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I4_Un, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I8, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I8_Un, il.DefineLabel()));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U_Un, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U1, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U1_Un, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U2, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U2_Un, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U4, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U4_Un, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U8, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U8_Un, il.DefineLabel()));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_R4, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_R8, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_R_Un, il.DefineLabel()));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U1, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U2, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U4, il.DefineLabel()));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U8, il.DefineLabel()));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Neg, il.DefineLabel()));

            // test this, if it works off the stack don't have to handle reflection based array constructor
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Newarr, il.DefineLabel())); -> requires token [ERROR: INVALID TOKEN 0x0228028C] 

            result.Add(new IlEmitUnaryOpCode(OpCodes.Not, il.DefineLabel()));

            //can't pass arguments
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Shl, il.DefineLabel()));
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Shr, il.DefineLabel()));
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Shr_Un, il.DefineLabel()));

            // result.Add(new IlEmitUnaryOpCode(OpCodes.Unbox, il.DefineLabel())); requires token argument
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Unbox_Any, il.DefineLabel()));


            //binary
            result.Add(new IlEmitBinaryOpCode(OpCodes.Add, il.DefineLabel()));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Add_Ovf, il.DefineLabel()));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Add_Ovf_Un, il.DefineLabel()));

            result.Add(new IlEmitBinaryOpCode(OpCodes.And, il.DefineLabel()));

            result.Add(new IlEmitBinaryOpCode(OpCodes.Ceq, il.DefineLabel()));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Cgt, il.DefineLabel()));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Cgt_Un, il.DefineLabel()));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Clt, il.DefineLabel()));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Clt_Un, il.DefineLabel()));

            result.Add(new IlEmitBinaryOpCode(OpCodes.Div, il.DefineLabel()));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Div_Un, il.DefineLabel()));



            result.Add(new IlEmitBinaryOpCode(OpCodes.Mul, il.DefineLabel()));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Mul_Ovf, il.DefineLabel()));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Mul_Ovf_Un, il.DefineLabel()));


            result.Add(new IlEmitBinaryOpCode(OpCodes.Or, il.DefineLabel()));

            //result.Add(new IlEmitBinaryOpCode(OpCodes.Pop, il.DefineLabel()));


            result.Add(new IlEmitBinaryOpCode(OpCodes.Rem, il.DefineLabel()));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Rem_Un, il.DefineLabel()));

            result.Add(new IlEmitBinaryOpCode(OpCodes.Sub, il.DefineLabel()));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Sub_Ovf, il.DefineLabel()));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Sub_Ovf_Un, il.DefineLabel()));

            result.Add(new IlEmitBinaryOpCode(OpCodes.Xor, il.DefineLabel()));
            return result;
        }

        public static List<IlEmitOpCodeInfo> GetCodesForDynamicCsDynamic()
        {

            //dummy objects:


            var result = new List<IlEmitOpCodeInfo>();

            //var stack = new Stack<object>();


            //var val = stack.Pop();
            //IlInstruction current = (IlInstruction)val;

            //unary
            result.Add(new IlEmitUnaryOpCode(OpCodes.Box, "((object){0})")); //requires token argument ->  [ERROR: INVALID TOKEN 0x0228028C] 

            result.Add(new IlEmitUnaryOpCode(OpCodes.Ckfinite, "({0} is float ? float.IsInfinity((float){0}) : double.IsInfinity((double){0}))"));


            //result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I, "//noti)); <--//TODO: Pointer

            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I1, "unchecked(Convert.ToSByte({0}))"));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I2, "unchecked(Convert.ToInt16({0}))"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I4, "unchecked(Convert.ToInt32({0}))"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_I8, "unchecked(Convert.ToInt64({0}))"));

            //TODO: Pointer
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I, "stack.Push(Convert.ToSByte(stack.Pop()));"));
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I_Un, "stack.Push(Convert.ToSByte(stack.Pop()));"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I1, "Convert.ToSByte({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I1_Un, "Convert.ToSByte({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I2, "Convert.ToInt16({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I2_Un, "Convert.ToInt16({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I4, "Convert.ToInt32({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I4_Un, "Convert.ToInt32({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I8, "Convert.ToInt64({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_I8_Un, "Convert.ToInt64({0})"));

            //TODO: Pointer
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U));
            //result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U_Un));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U1, "Convert.ToByte({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U1_Un, "Convert.ToByte({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U2, "Convert.ToUInt16({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U2_Un, "Convert.ToUInt16({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U4, "Convert.ToUInt32({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U4_Un, "Convert.ToUInt32({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U8, "Convert.ToUInt64({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_Ovf_U8_Un, "Convert.ToUInt64({0})"));


            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_R4, "Convert.ToSingle({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_R8, "Convert.ToDouble({0})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_R_Un, "Convert.ToSingle({0})"));

            //result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U1, "unchecked(Convert.ToByte({0}))"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U2, "unchecked(Convert.ToUInt16({0}))"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U4, "unchecked(Convert.ToUInt32({0}))"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Conv_U8, "unchecked(Convert.ToUInt64({0}))"));


            result.Add(new IlEmitUnaryOpCode(OpCodes.Neg, "-((dynamic){0})"));

            // test this, if it works off the stack don't have to handle reflection based array constructor
            result.Add(new IlEmitUnaryOpCode(OpCodes.Newarr, " Array.CreateInstance((Type){1}, (int){0})"));  //-> requires token [ERROR: INVALID TOKEN 0x0228028C] 

            result.Add(new IlEmitUnaryOpCode(OpCodes.Not, "!((dynamic){0})"));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Pop));
            //can't pass arguments

            result.Add(new IlEmitUnaryOpCode(OpCodes.Shl, "((dynamic){0}) << ((int){1})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Shr, "((dynamic){0}) >> ((int){1})"));
            result.Add(new IlEmitUnaryOpCode(OpCodes.Shr_Un, "((dynamic){0}) >> ((int){1})"));

            result.Add(new IlEmitUnaryOpCode(OpCodes.Unbox, "Convert.ChangeType({0}, resolveType((int){1}))"));  //requires token argument
            result.Add(new IlEmitUnaryOpCode(OpCodes.Unbox_Any, "Convert.ChangeType({0}, resolveType((int){1}))"));  //requires token argument


            //binary
            string binaryFormat = "((dynamic){{0}}) {0} ((dynamic){{1}})";
            string binaryMsilBoolFormat = "Convert.ToInt32(" + binaryFormat + ")";

            result.Add(new IlEmitBinaryOpCode(OpCodes.Add, $"unchecked({string.Format(binaryFormat, " + ")})"));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Add_Ovf, string.Format(binaryFormat, "+")));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Add_Ovf_Un, string.Format(binaryFormat, "+")));

            result.Add(new IlEmitBinaryOpCode(OpCodes.And, string.Format(binaryFormat, "&")));

            result.Add(new IlEmitBinaryOpCode(OpCodes.Ceq, string.Format(binaryMsilBoolFormat, "==")));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Cgt, string.Format(binaryMsilBoolFormat, ">")));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Cgt_Un, string.Format(binaryMsilBoolFormat, "<")));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Clt, string.Format(binaryMsilBoolFormat, "<=")));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Clt_Un, string.Format(binaryMsilBoolFormat, "<=")));

            result.Add(new IlEmitBinaryOpCode(OpCodes.Div, string.Format(binaryFormat, "/")));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Div_Un, string.Format(binaryFormat, "/")));



            result.Add(new IlEmitBinaryOpCode(OpCodes.Mul, $"unchecked({string.Format(binaryFormat, " * ")})"));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Mul_Ovf, string.Format(binaryFormat, "*")));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Mul_Ovf_Un, string.Format(binaryFormat, "*")));


            result.Add(new IlEmitBinaryOpCode(OpCodes.Or, string.Format(binaryFormat, "|")));




            result.Add(new IlEmitBinaryOpCode(OpCodes.Rem, string.Format(binaryFormat, "%")));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Rem_Un, string.Format(binaryFormat, "%")));

            result.Add(new IlEmitBinaryOpCode(OpCodes.Sub, $"unchecked({string.Format(binaryFormat, "-")})"));
            result.Add(new IlEmitBinaryOpCode(OpCodes.Sub_Ovf, string.Format(binaryFormat, "-"))); ;
            result.Add(new IlEmitBinaryOpCode(OpCodes.Sub_Ovf_Un, string.Format(binaryFormat, "-")));

            result.Add(new IlEmitBinaryOpCode(OpCodes.Xor, string.Format(binaryFormat, "^")));
            return result;
        }
    }
    public abstract class IlEmitOpCodeInfo
    {
        public Label Label;
        public OpCode OpCode;
        public string CsExpression;
        public IlEmitOpCodeInfo(OpCode opCode)
        {
            this.OpCode = opCode;
        }
        public IlEmitOpCodeInfo(OpCode opCode, Label label)
        {
            this.OpCode = opCode;
            this.Label = label;

        }

        public abstract void EmitMethodCallInstructions(ILGenerator il);
        public abstract void EmitInstructions(ILGenerator il, MethodInfo pushMethod, MethodInfo popMethod);


    }


    /// <summary>
    /// Pops two values off the caller's stack, executes the <see cref="IlEmitUnaryOpCode.OpCode"/> and pushes the result back onto the callers stack
    /// </summary>
    public class IlEmitUnaryOpCode : IlEmitOpCodeInfo
    {

        public IlEmitUnaryOpCode(OpCode opCode) : base(opCode)
        {
        }

        public IlEmitUnaryOpCode(OpCode opCode, string csExpression) : base(opCode)
        {
            this.CsExpression = csExpression;
        }
        public IlEmitUnaryOpCode(OpCode opCode, Label label) : base(opCode, label)
        {
        }

        public override void EmitMethodCallInstructions(ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(this.OpCode);
            il.Emit(OpCodes.Ret);

        }

        /// <summary>
        /// Emits il to the <paramref name="ilGenerator"/> that to pops one value from the caller's stack using the <paramref name="popMethod"/> and then executes the <see cref="IlEmitUnaryOpCode.Opcode"/>.
        /// The result is then pushed onto the caller's stack using the <paramref name="pushMethod"/>.
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="pushMethod"></param>
        /// <param name="popMethod"></param>
        public override void EmitInstructions(ILGenerator ilGenerator, MethodInfo pushMethod, MethodInfo popMethod)
        {

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, popMethod);
            ilGenerator.Emit(this.OpCode);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, pushMethod);
            ilGenerator.Emit(OpCodes.Ret);
        }
    }


    /// <summary>
    /// Pops two values off the caller's stack, executes the <see cref="IlEmitBinaryOpCode.OpCode"/> and pushes the result back onto the callers stack
    /// </summary>
    public class IlEmitBinaryOpCode : IlEmitOpCodeInfo
    {

        public IlEmitBinaryOpCode(OpCode opCode) : base(opCode)
        {
        }
        public IlEmitBinaryOpCode(OpCode opCode, Label label) : base(opCode, label)
        {
        }
        public IlEmitBinaryOpCode(OpCode opCode, string csExpression) : base(opCode)
        {
            this.CsExpression = csExpression;
        }


        public override void EmitMethodCallInstructions(ILGenerator il)
        {

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Unbox_Any, typeof(int));
            //il.Emit(OpCodes.Ldarg_1);
            //il.Emit(OpCodes.Unbox_Any, typeof(int));

            il.Emit(OpCodes.Ret);

        }
        /// <summary>
        /// Emits il to the <paramref name="ilGenerator"/> that to pops two values from the caller's stack using the <paramref name="popMethod"/> and then executes the <see cref="IlEmitUnaryOpCode.Opcode"/>.
        /// The result is then pushed onto the caller's stack using the <paramref name="pushMethod"/>.
        public override void EmitInstructions(ILGenerator il, MethodInfo pushMethod, MethodInfo popMethod)
        {

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, popMethod);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, popMethod);
            il.Emit(this.OpCode);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, pushMethod);
            il.Emit(OpCodes.Ret);
        }
    }

}
