using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEngine.Models
{
    public struct ILMethod
    {
        public Type[] ParameterTypes { get; set; }
        public Type[] LocalTypes { get; set; }
        public string Name { get; set; }

        public List<ILInstruction> Instructions { get; set; }
        public Module Module { get; set; }

        public ILMethod(string methodName) : this(methodName, typeof(object)) { }

        public ILMethod(string methodName, Type returnType)
        {
            Name = methodName;
            ReturnType = returnType;
            LocalTypes = Type.EmptyTypes;
            ParameterTypes = Type.EmptyTypes;
            Instructions = new List<ILInstruction>();
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
        public void AddInstructions(params ILInstruction[] instructions)
        {
            Instructions.AddRange(instructions);
        }
        public void AddInstructions(params ILOpCodeValues[] instructions)
        {
            Instructions.AddRange(instructions.Select(x => ILInstruction.Create(x)));
        }

        public MethodInfo Compile()
        {
            var locals = LocalTypes.ToList();
            var instructions = Instructions.ToList();
            var tokenResolver = new ILInstructionResolver(Module);
            Dictionary<int, Label> labels = null;
            return DynamicCompiler.CompileMethod(Name, ReturnType, ParameterTypes, (gen) =>
            {
                locals.ForEach(x => gen.DeclareLocal(x));
                labels = instructions.Where(x => x.Label.HasValue).ToDictionary(x => (int)x.Label, x => gen.DefineLabel());
                instructions.ToList().ForEach(x => x.Emit(gen, tokenResolver, labels));
            });
        }
    }
}
