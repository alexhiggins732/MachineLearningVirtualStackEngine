using ILEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ILEngineTests
{

    public class IlMethodBuilder
    {
     
        public static MethodInfo CompileBinary(ILOpCodeValues opCodeValue, Type returnType, params Type[] parameters)
        {
            var methodName = $"{opCodeValue}";
            var method = new IlMethod(methodName, returnType);

            method.AddParameters(parameters);
            method.AddInstructions(ILOpCodeValues.Ldarg_0,
                ILOpCodeValues.Ldarg_1,
                opCodeValue,
                ILOpCodeValues.Ret);

            return method.Compile();
        }
        public static MethodInfo Compile_Add()
        {
            var method = IlMethodBuilder<int>.Create(nameof(ILOpCodeValues.Add));
            method.AddInstructions(ILOpCodeValues.Ldc_I4_0,
                ILOpCodeValues.Ldc_I4_0,
                ILOpCodeValues.Add,
                ILOpCodeValues.Ret);
            return method.Compile();
        }
        public static MethodInfo Compile_Add(Type returnType, params Type[] parameters)
        {
            var method = new IlMethod(nameof(ILOpCodeValues.Add), returnType);

            method.AddParameters(parameters);
            method.AddInstructions(ILOpCodeValues.Ldarg_0,
                ILOpCodeValues.Ldarg_1,
                ILOpCodeValues.Add,
                ILOpCodeValues.Ret);

            return method.Compile();
        }


        public static MethodInfo Compile_Add_Ovf()
        {
            var method = IlMethodBuilder<int>.Create(nameof(ILOpCodeValues.Add_Ovf));
            method.AddInstructions(ILOpCodeValues.Ldc_I4_0,
                ILOpCodeValues.Ldc_I4_0,
                ILOpCodeValues.Add_Ovf,
                ILOpCodeValues.Ret);
            return method.Compile();
        }

        public static MethodInfo Compile_Add_Ovf(Type returnType, params Type[] parameters)
        {
            var method = new IlMethod(nameof(ILOpCodeValues.Add_Ovf), returnType);

            method.AddParameters(parameters);
            method.AddInstructions(ILOpCodeValues.Ldarg_0,
                ILOpCodeValues.Ldarg_1,
                ILOpCodeValues.Add_Ovf,
                ILOpCodeValues.Ret);

            return method.Compile();
        }

        public static MethodInfo Compile_Add_Ovf_Un()
        {
            var method = IlMethodBuilder<int>.Create(nameof(ILOpCodeValues.Add_Ovf_Un));
            method.AddInstructions(ILOpCodeValues.Ldc_I4_0,
                ILOpCodeValues.Conv_Ovf_U4,
                ILOpCodeValues.Ldc_I4_0,
                ILOpCodeValues.Conv_Ovf_U4,
                ILOpCodeValues.Add_Ovf_Un,
                ILOpCodeValues.Ret);
            return method.Compile();
        }

        public static MethodInfo Compile_Add_Ovf_Un(Type returnType, params Type[] parameters)
        {
            var method = new IlMethod(nameof(ILOpCodeValues.Add_Ovf_Un), returnType);

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
    public class IlMethodBuilder<T>
    {
        public static IlMethod Create(string methodName) => new IlMethod(methodName, typeof(T));
    }
    public struct IlMethod
    {
        public Type[] ParameterTypes { get; set; }
        public Type[] LocalTypes { get; set; }
        public string Name { get; set; }

        public List<IlInstruction> Instructions { get; set; }
        public Module Module { get; set; }

        public IlMethod(string methodName) : this(methodName, typeof(object)) { }

        public IlMethod(string methodName, Type returnType)
        {
            Name = methodName;
            ReturnType = returnType;
            LocalTypes = Type.EmptyTypes;
            ParameterTypes = Type.EmptyTypes;
            Instructions = new List<IlInstruction>();
            Module = MethodBase.GetCurrentMethod().Module;
        }
        public Type ReturnType { get; set; }

        public void AddLocals(params Type[] types)
        {
            LocalTypes = LocalTypes.Concat(types).ToArray();
        }
        public void AddParameters(params Type[] types)
        {
            ParameterTypes = ParameterTypes.Concat(types).ToArray();
        }
        public void AddInstructions(params IlInstruction[] instructions)
        {
            Instructions.AddRange(instructions);
        }
        public void AddInstructions(params ILOpCodeValues[] instructions)
        {
            Instructions.AddRange(instructions.Select(x => IlInstruction.Create(x)));
        }

        internal MethodInfo Compile()
        {
            var locals = LocalTypes.ToList();
            var instructions = Instructions.ToList();
            var tokenResolver = new IlInstructionResolver(Module);
            return DynamicCompiler.CompileMethod(Name, ReturnType, ParameterTypes, (gen) =>
            {
                locals.ForEach(x => gen.DeclareLocal(x));
                instructions.ToList().ForEach(x => x.Emit(gen, tokenResolver));
            });
        }
    }
}
