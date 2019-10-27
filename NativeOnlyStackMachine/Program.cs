using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeOnlyStackMachine
{
    class Program
    {
        static void BuildSystemAssembly()
        {
            VirtualAssemblyBuilder.BuildFromClr("System");
        }
        static void Main(string[] args)
        {
            MachineStackEngine.RunTest();
            BuildSystemAssembly();
            var machine = new InstructionEngine();
            InstructionBuilder builder = machine.CreateInstructionBuilder();
            var loop = new LoopInstruction();
            builder.Add(loop);

          
            var entryPoint = machine.Main();
            machine.AddInstructions(builder.Build());
        }
    }

    public class LoopInstruction : Instruction
    {
        public string Label;
        public int Address;
        public LoopInstruction()
        {
            this.Body = new List<Instruction>();
        }
    }

    public class InstructionBuilder
    {

        private Instruction root;
        private InstructionEngine engine;
        private List<Instruction> instructions;
        public InstructionBuilder(InstructionEngine engine)
        {
            this.engine = engine;
            root = engine.Main();

        }
        public void Add(Instruction instruction)
        {
            root.AddInstruction(instruction);
            instruction.Parent = root;
            
            engine.AddInstruction(instruction);
        }
        public List<Instruction> Build()
        {
            return instructions.ToList();
        }

    }

    public class MainInstruction :Instruction
    {
        public MainInstruction()
        {

        }
    }
    public class Instruction
    {
        public Instruction Parent;
        public List<Instruction> Body;
        protected Instruction()
        {

        }
        public Instruction(Instruction parentInstruction)
        {
            this.Parent = parentInstruction;
            Body = new List<Instruction>();
        }
        public static Instruction Noop()
        {
            return new Instruction();
        }

        internal void AddInstruction(Instruction instruction)
        {
            throw new NotImplementedException();
        }
    }

    public class InstructionEngine
    {
        public Dictionary<int, Instruction> InstructionTable;
        public InstructionEngine()
        {
            this.InstructionTable = new Dictionary<int, Instruction>();
            main = new MainInstruction();
        }
        public void Run()
        {
            var t = new List<Instruction>();
            while (true)
            {
                var current = t.Last();
                t.RemoveAt(t.Count - 1);
                ExecuteInstruction(t);
            }
        }

        public void AddInstruction(Instruction instruction)
        {
            InstructionTable.Add(InstructionTable.Count, instruction);
        }
        internal void AddInstructions(List<Instruction> instructions)
        {
            if (instructions == null || instructions.Count == 0)
                AddInstruction(Instruction.Noop());
            else
            {
                foreach (var instruction in instructions)
                {
                    AddInstruction(instruction);
                    AddInstructions(instruction.Body);
                }
            }

        }

        private void ExecuteInstruction(List<Instruction> t)
        {

        }

        private MainInstruction main;
        internal MainInstruction Main()
        {
            return main;
        }

        internal InstructionBuilder CreateInstructionBuilder()
        {
            return new InstructionBuilder(this);
        }
    }
}
