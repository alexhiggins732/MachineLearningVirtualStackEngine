using ILEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.Compilers
{
    public class ILMethodBuilder
    {
        public static MethodInfo Compile_LdNull()
        {
            var method = new ILMethod(nameof(ILOpCodeValues.Ldnull));
            method.AddInstructions(ILInstruction.Create(OpCodes.Ldnull), ILInstruction.Ret);
            return method.Compile();
        }

        public static MethodInfo Compile_Ldloc_0()
        {
            var method = new ILMethod(nameof(ILOpCodeValues.Ldloc_0), typeof(int));
            method.AddLocals(typeof(int));
            method.AddInstructions(
                ILInstruction.Create(OpCodes.Ldc_I4_1),
                ILInstruction.Create(OpCodes.Stloc_0),
                ILInstruction.Create(OpCodes.Ldloc_0), 
                ILInstruction.Ret);
            return method.Compile();
        }

        public static MethodInfo CompileBinary(ILOpCodeValues opCodeValue, Type returnType, params Type[] parameters)
        {
            var methodName = $"{opCodeValue}";
            var method = new ILMethod(methodName, returnType);

            method.AddParameters(parameters);
            method.AddInstructions(ILOpCodeValues.Ldarg_0,
                ILOpCodeValues.Ldarg_1,
                opCodeValue,
                ILOpCodeValues.Ret);

            return method.Compile();
        }

        public static MethodInfo Compile_AddIntLocals()
        {
            var method = ILMethodBuilder<int>.Create(nameof(ILOpCodeValues.Add));

            method.AddInstructions(ILOpCodeValues.Ldc_I4_1,
                ILOpCodeValues.Ldc_I4_1,
                ILOpCodeValues.Add,
                ILOpCodeValues.Ret);
            return method.Compile();
        }

        public static MethodInfo Compile_Add(Type returnType, params Type[] parameters)
        {
            var method = new ILMethod(nameof(ILOpCodeValues.Add), returnType);

            method.AddParameters(parameters);
            method.AddInstructions(ILOpCodeValues.Ldarg_0,
                ILOpCodeValues.Ldarg_1,
                ILOpCodeValues.Add,
                ILOpCodeValues.Ret);

            return method.Compile();
        }

        public static MethodInfo Compile_Add_Ovf()
        {
            var method = ILMethodBuilder<int>.Create(nameof(ILOpCodeValues.Add_Ovf));
            method.AddInstructions(ILInstruction.Create(OpCodes.Ldc_I4, int.MaxValue),
                ILInstruction.Create(OpCodes.Ldc_I4, int.MaxValue),
                ILInstruction.Create(OpCodes.Add_Ovf),
                ILInstruction.Ret);
            return method.Compile();
        }

        public static MethodInfo Compile_Add_Ovf(Type returnType, params Type[] parameters)
        {
            var method = new ILMethod(nameof(ILOpCodeValues.Add_Ovf), returnType);

            method.AddParameters(parameters);
            method.AddInstructions(ILOpCodeValues.Ldarg_0,
                ILOpCodeValues.Ldarg_1,
                ILOpCodeValues.Add_Ovf,
                ILOpCodeValues.Ret);

            return method.Compile();
        }
        public static MethodInfo Compile_Add_Ovf_Un()
        {
            var method = ILMethodBuilder<uint>.Create(nameof(ILOpCodeValues.Add_Ovf_Un));

            method.AddInstructions(ILOpCodeValues.Ldc_I4_1,
                ILOpCodeValues.Conv_Ovf_U4,
                ILOpCodeValues.Ldc_I4_1,
                ILOpCodeValues.Conv_Ovf_U4,
                ILOpCodeValues.Add_Ovf_Un,
                ILOpCodeValues.Ret);
            return method.Compile();
        }

        public static MethodInfo Compile_Add_Ovf_Un(Type returnType, params Type[] parameters)
        {
            var method = new ILMethod(nameof(ILOpCodeValues.Add_Ovf_Un), returnType);

            method.AddParameters(parameters);
            method.AddInstructions(ILOpCodeValues.Ldarg_0,
                ILOpCodeValues.Conv_Ovf_U4,
                ILOpCodeValues.Ldarg_1,
                ILOpCodeValues.Conv_Ovf_U4,
                ILOpCodeValues.Add_Ovf_Un,
                ILOpCodeValues.Ret);

            return method.Compile();
        }
    }
}
