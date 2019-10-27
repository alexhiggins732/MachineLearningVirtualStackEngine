using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IlVmModel
{
    public class IlVmAssemblyBuilder
    {
        public string AssemblyName { get; protected set; }
        public AssemblyBuilder AssemblyBuilder { get; set; }
        public string AssemblyPath { get; private set; }
        public ModuleBuilder ModuleBuilder { get; private set; }
        public IlVmAssemblyBuilder() : this("IlVm")
        {
            var asmName = new AssemblyName("MsilEngine");
            AppDomain domain = AppDomain.CurrentDomain;

            AssemblyBuilder =
               domain.DefineDynamicAssembly(asmName,
                   AssemblyBuilderAccess.RunAndSave);

            AssemblyPath = AssemblyPath ?? asmName.Name + ".dll";
            ModuleBuilder =
                 AssemblyBuilder.DefineDynamicModule(asmName.Name,
                    AssemblyPath, true);
        }
        public IlVmAssemblyBuilder(string assemblyName) => this.AssemblyName = assemblyName;

    }
    public class ILVmEngineCompiler : IlVmAssemblyBuilder
    {
        private TypeBuilder typeBuilder;
        public TypeBuilder TypeBuilder => typeBuilder ?? (typeBuilder = ModuleBuilder.DefineType(TypeName, TypeAttributes.Public));
        public string TypeName { get; protected set; }
        public ILVmEngineCompiler() : base() { this.TypeName = AssemblyName; }
        public ILVmEngineCompiler(string assemblyName) => this.AssemblyName = this.TypeName = assemblyName;
        public ILVmEngineCompiler(string assemblyName, string typeName) { this.AssemblyName = assemblyName; this.TypeName = typeName; }

        private Type createdType;
        public Type CreateType() => createdType ?? (createdType = TypeBuilder.CreateType());
        public MethodBuilder MethodBuilder(string MethodName,
            MethodAttributes atts = MethodAttributes.Public | MethodAttributes.Static,
            Dictionary<string, Type> MethodAguments = null
            )
        {


            // Define a type to contain the method.




            var argParamters = (MethodAguments ?? new Dictionary<string, Type>()).ToList();

            // public static void ExecuteMsil(object[] instructions, MsilMetaTable meta)
            MethodBuilder methodBuilder =
             TypeBuilder.DefineMethod(MethodName,
                                        atts,
                                        typeof(object),
                                        argParamters.Select(x => x.Value).ToArray());
            int parameterPos = 0;
            argParamters.ForEach(x =>
            {
                methodBuilder.DefineParameter(++parameterPos, ParameterAttributes.None, x.Key);
            });
            return methodBuilder;
        }

    }

    public class DefaultVmBuilder
    {
        public static void BuildVm()
        {
            var compiler = new ILVmEngineCompiler("VmILInterpreter");

            /*
             *   .field private class [System]System.Collections.Generic.Stack`1<object> stack
  .field private class [mscorlib]System.Func`1<bool>[] ilInstructionHandlers
  */


            var type = compiler.TypeBuilder;
            var stack = type.DefineField("Stack", typeof(Stack<object>), FieldAttributes.Public);
            var delegates = type.DefineField("Delegates", typeof(Delegate[]), FieldAttributes.Public);



            var ctor = type.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard, null);

            var ilCtor = ctor.GetILGenerator();
            ilCtor.Emit(OpCodes.Ldarg_0); //load this
            ilCtor.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)); //call object constructor

            //initialize Stack<object> stack
            ilCtor.Emit(OpCodes.Newobj, stack.FieldType.GetConstructor(Type.EmptyTypes)); //call object constructor
            ilCtor.Emit(OpCodes.Stfld, stack); //load this


            //initialize Delegate[] delegates
            ilCtor.Emit(OpCodes.Newarr, typeof(Delegate)); //call object constructor
            ilCtor.Emit(OpCodes.Stfld, stack); //load this

            var parameters = new Dictionary<string, Type>
            {
                {"instructions", typeof(int[]) }
            };

            var methodAttributes = MethodAttributes.Public;


            var methodBuilder = compiler.MethodBuilder("Execute", methodAttributes, parameters);
            var methodArguments = methodBuilder.GetParameters().ToList();
            var instructions = methodArguments[0];
            //todo: inline this with MSIL jumptable
            //var handlerParameters = new Dictionary<string, Type> { { "opcode", typeof(int) } };
            //MethodBuilder OpcodeHandler = null;
            //OpcodeHandler = compiler.MethodBuilder(nameof(OpcodeHandler), MethodAttributes.Public, handlerParameters);
            //OpcodeHandler.SetReturnType(typeof(bool));

            var il = methodBuilder.GetILGenerator();
            var comp = il.DefineLabel();
            var execute = il.DefineLabel();
            var ret = il.DefineLabel();

            var pos = il.DeclareLocal(typeof(int));
            pos.SetLocalSymInfo(nameof(pos));
            //var opcode = il.DeclareLocal(typeof(int));
            // opcode.SetLocalSymInfo(nameof(opcode));
            var max = il.DeclareLocal(typeof(int));
            max.SetLocalSymInfo(nameof(max));
            var hasjump = il.DeclareLocal(typeof(bool));
            hasjump.SetLocalSymInfo(nameof(hasjump));


            il.Emit(OpCodes.Ldc_I4_M1);
            il.Emit(OpCodes.Stloc, pos); //pos




            il.Emit(OpCodes.Ldlen);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Sub);
            il.Emit(OpCodes.Stloc, max); //max

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc, hasjump);

            il.MarkLabel(execute);

            //  il.Emit(OpCodes.Ldarg_1); //load instructions -> code below makes this index based
            //      because index args change for static vs instance
            // if not static

            EmitLoadArg(methodAttributes, methodArguments, instructions, il);




            il.Emit(OpCodes.Ldloc, pos); //pos
            il.Emit(OpCodes.Ldelem_I4); //load instructions[pos];
            // il.Emit(OpCodes.Stloc, opcode); //set opcode

            //il.Emit(OpCodes.Call, OpcodeHandler);

            var BuiltInOpcodes = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static).Select(x => (OpCode)x.GetValue(null)).ToList();
            var BuiltInOpcodesByValue = BuiltInOpcodes.ToDictionary(x => (int)x.Value, x => x);
            var BuiltInOpcodeLabels = BuiltInOpcodes.ToDictionary(x => (int)x.Value, x => il.DefineLabel());
            il.Emit(OpCodes.Switch, BuiltInOpcodeLabels.Select(x => x.Value).ToArray());
            foreach (var opcodeLabel in BuiltInOpcodeLabels)
            {
                var value = opcodeLabel.Key;
                var label = opcodeLabel.Value;

                var opcode = BuiltInOpcodesByValue[value];
                il.MarkLabel(label); // mark jmp address for instruction

                //TODO: Each opcode will have its own instructions for the vm:
                // EG: il.Emit(OpCodes.Add, comp); -> will pop the top two variables off the stack and call add.
                //  But the VM needs Pop the variables off the vm stack and push them onto the native stack before call the opcode.
                //  Additionally, after the native opcode is executed the any result to be popped off the native stack and pushed onto the vm stack.

                il.Emit(opcode);

            }


            il.Emit(OpCodes.Brfalse_S, comp);
            il.Emit(OpCodes.Ldarg_1); //load instructions
            il.Emit(OpCodes.Ldloc, pos); //pos
            il.Emit(OpCodes.Ldc_I4_1, 1); //pos
            il.Emit(OpCodes.Add, 1); //pos
            il.Emit(OpCodes.Stloc, pos); //pos
            il.Emit(OpCodes.Br, execute); // todo: 
            //il.Emit(OpCodes.Br, comp); // would be correct to check array bounds but since this is managed code...

            il.MarkLabel(comp); //continue executing?
            il.Emit(OpCodes.Ldloc, pos); //pos
            il.Emit(OpCodes.Ldloc, max);//pos
            il.Emit(OpCodes.Blt, execute);

            il.MarkLabel(ret);


            var compiledType = compiler.TypeBuilder.CreateType();

            compiler.AssemblyBuilder.Save(compiler.AssemblyPath);
            System.IO.File.Copy(compiler.AssemblyPath, @"C:\Users\alexander.higgins\source\repos\ILDisassembler\IlVmModel\bin\Debug\" + compiler.AssemblyPath, true);

        }

        /// <summary>
        /// Helper method for emitting indexed based argument loading instructions.
        /// </summary>
        /// <param name="methodAttributes">Method attributes needed to calculate offset for static vs instance methods</param>
        /// <param name="methodArguments">List of all instructions for this method, used to determin the absolute index of the argument</param>
        /// <param name="methodArgument">The method argument to generate the instruction for using the calculated relative index.</param>
        /// <param name="il">The il generator to emit the IL instructions to.</param>
        private static void EmitLoadArg(MethodAttributes methodAttributes, List<ParameterInfo> methodArguments, ParameterInfo methodArgument, ILGenerator il)
        {
            var instanceIndex = methodArguments.IndexOf(methodArgument) + 1;
            int staticOffset = ((int)(methodAttributes & MethodAttributes.Static)) >> 4; // 1 if static 0 if not
            var idx = instanceIndex - staticOffset; // addJust index if the method is static;
            
            // use optimized instructions if index<=3;
            switch (idx)
            {
                case 0:
                    il.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldarg_0);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (idx < 256)
                        il.Emit(OpCodes.Ldarg_S, idx);
                    else
                        il.Emit(OpCodes.Ldarg, idx);
                    break;
            }

        }

        /// <summary>
        /// Helper function to generatte indexed based argument storing instructions
        /// </summary>
        /// <param name="methodAttributes">Attributes to determine relative indexed based on static vs public</param>
        /// <param name="methodArguments">List of method arguments for the method used to determine the absolute method argument index</param>
        /// <param name="methodArgument">The method arument to create the instruction for</param>
        /// <param name="il">The IL generator to emit the store instruction to.</param>
        private static void EmitSetArg(MethodAttributes methodAttributes, List<ParameterInfo> methodArguments, ParameterInfo methodArgument, ILGenerator il)
        {
            var instanceIndex = methodArguments.IndexOf(methodArgument) + 1;
            int staticOffset = ((int)(methodAttributes & MethodAttributes.Static)) >> 4; // 1 if static 0 if not
            var idx = instanceIndex - staticOffset; // addJust index if the method is static;
            // no short hand for storing the args, it is indexed based using either a byte or int indexer
            if (idx < 256)
                il.Emit(OpCodes.Starg_S, idx);
            else
                il.Emit(OpCodes.Starg, idx);

        }

    }
}
