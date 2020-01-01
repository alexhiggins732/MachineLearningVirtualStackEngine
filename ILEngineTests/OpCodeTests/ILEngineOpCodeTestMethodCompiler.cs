using ILEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.Tests
{

    public class ILEngineOpCodeTestMethodCompiler
    {
        public static MethodInfo CompileMethod(OpCode opCode, MethodInfo existingMethod, Action<ILGenerator> emitAction)
        {
            var parameterTypes = existingMethod.GetParameters().Select(x => x.ParameterType).ToArray();
            return CompileMethod(opCode, existingMethod.ReturnType, parameterTypes, emitAction);
        }
        public static MethodInfo CompileMethod(OpCode opCode, Type returnType, Type[] parameterTypes, Action<ILGenerator> emitAction)
        {
            var testName = "IlUnitTest" + opCode.Value.ToString();
            var asmName = new AssemblyName(testName);
            AppDomain domain = AppDomain.CurrentDomain;

            AssemblyBuilder wrapperAssembly =
                domain.DefineDynamicAssembly(asmName,
                    AssemblyBuilderAccess.RunAndSave);

            // wrapperAssembly.ModuleResolve += WrapperAssembly_ModuleResolve;
            var assemblyPath = asmName.Name + ".dll";

            ModuleBuilder wrapperModule =
                wrapperAssembly.DefineDynamicModule(asmName.Name,
                   assemblyPath);



            // Define a type to contain the method.
            TypeBuilder typeBuilder =
                wrapperModule.DefineType("testName", TypeAttributes.Public);

            //typeBuilder.DefineField("bigInt", typeof(BigInteger), FieldAttributes.Public);

            var mb = typeBuilder.DefineMethod("test", MethodAttributes.Public | MethodAttributes.Static, returnType, parameterTypes);

            var gen = mb.GetILGenerator();

            emitAction(gen);
            var type = typeBuilder.CreateType();
            var compiledMethod = type.GetMethod("test");
            var il = compiledMethod.GetMethodBody().GetILAsByteArray();
            var method = mb;
            return compiledMethod;
        }

    }

}
