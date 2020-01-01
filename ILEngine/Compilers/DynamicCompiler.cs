using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine
{
    public class DynamicCompiler
    {
        private static ConcurrentDictionary<string, int> methodTracker = new ConcurrentDictionary<string, int>();

   
        public static MethodInfo CompileMethod(string MethodName, Type returnType, Type[] parameterTypes, Action<ILGenerator> emitAction)
        {
            int version = methodTracker.GetOrAdd(MethodName, 0);
            methodTracker[MethodName] += 1;

            var testName = "IlDynamicMethod" + MethodName;
            var asmName = new AssemblyName($"{testName}_v_{version}");
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
                wrapperModule.DefineType(MethodName, TypeAttributes.Public);

            //typeBuilder.DefineField("bigInt", typeof(BigInteger), FieldAttributes.Public);

            var mb = typeBuilder.DefineMethod(MethodName, MethodAttributes.Public | MethodAttributes.Static, returnType, parameterTypes);

            var gen = mb.GetILGenerator();

            emitAction(gen);
            var type = typeBuilder.CreateType();
            var compiledMethod = type.GetMethod(MethodName);
            var il = compiledMethod.GetMethodBody().GetILAsByteArray();
            var method = mb;
            return compiledMethod;
        }
    }
}
