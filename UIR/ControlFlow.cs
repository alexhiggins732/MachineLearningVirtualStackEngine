using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIR
{


    public class StackLoop : IEnumerable<Action>
    {
        private Stack<Action> instructions;
        private Action Initializer;
        private Func<bool> Evaluate;
        private Action Body;
        private Action Increment;
        public StackLoop(Action initializer, Func<bool> evaluator, Action body, Action incrementor)
        {

            Evaluate = evaluator;
            Body = body;
            Increment = incrementor;
            Initializer = initializer;

            Init();
        }

        public IEnumerator<Action> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
   


        private Action Loop = null;
        private Action LoopInc = null;
        private Action LoopBody = null;
        private Action LoopInit = null;
        private Action LoopEvaluate = null;
        private void Init()
        {
            instructions = new Stack<Action>();

            LoopInc = () => { Increment(); instructions.Push(Loop); };
            LoopBody = () => { Body(); LoopInc(); };
            LoopInit = () => { Initializer(); instructions.Push(Loop); };
            LoopEvaluate = () => { if (Evaluate()) instructions.Push(LoopBody); };
            Loop = () => instructions.Push(LoopEvaluate);
            // LoopInit => Loop
            // Loop => Evaluate(), LoopBody
            Reset();

        }
        private void Reset()
        {
            instructions.Clear();
            instructions.Push(LoopInit);
        }

        internal void Execute(Action instruction)
        {
            instruction();
        }

        public class Enumerator : IEnumerator<Action>
        {
            private StackLoop loop;

            public Enumerator(StackLoop loop)
            {
                this.loop = loop;
            }

            public Action Current { get; private set; } = null;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                loop = null;
                Current = null;
            }

            public bool MoveNext()
            {
                var result = loop.instructions.Count > 0;
                if (result)
                {
                    Current = loop.instructions.Pop();
                }
                else { Current = null; }
                return result;

            }

            public void Reset()
            {
                loop.Reset();
            }
        }
    }
    public class StackLoopDemo
    {
        private Stack<dynamic> instructions;



        public StackLoopDemo()
        {
            instructions = new Stack<dynamic>();
            dynamic LoopState = null;

            Action Initializer = () => LoopState = 0;
            Func<bool> EvaluationExpression = () => LoopState < 10;
            Action LoopAction = () => Console.WriteLine(LoopState);
            Action LoopIncExpression = () => LoopState++;


            Action Loop = null;
            Action LoopInc = () => { LoopIncExpression(); instructions.Push(Loop); };
            // Can refactor this into a class LoopAction { Action ReturnInstruction {get;set;} ctor(Action nextInstruction) => ReturnInstruction = nextInstruction; }
            // allow caller to implement break or return inside of LoopAction by setting ReturnInstruction to relevant break/ret statement;

            Action LoopBody = () => { LoopAction(); LoopInc(); };
            Action LoopInit = () => { Initializer(); instructions.Push(Loop); };
            Action LoopEvaluate = () => { if (EvaluationExpression()) instructions.Push(LoopBody); };
            Loop = () => instructions.Push(LoopEvaluate);

            instructions.Push(LoopInit);
            //////Func<bool> evaluateLoopState = () => LoopState < 10;

            //////Action executeLoopBody = () =>
            ////// {
            //////     Console.WriteLine(LoopState);
            //////     instructions.Push(Loop);
            ////// };
            //////Action LoopInc = null;
            //////Action evaluateLoopStateInstruction = () =>
            //////{
            //////    dynamic stateEvaluator = instructions.Pop();
            //////    dynamic evalation = stateEvaluator();
            //////    if (evalation()) instructions.Push(executeLoopBody);
            //////};

            //////Loop = () => instructions.Push(evaluate);
            ////////{
            ////////    instructions.Push(evaluateLoopStateInstruction);
            ////////    instructions.Push(LoopInc);
            ////////};

            //////instructions.Push(init);

            // instructions: 
            //  init: set loop state; return loop;
            //  loop: return evaluate;
            //  evaluate: if (evaluateexpression()) return loopbody;
            //  loopbody: loopaction(); return loopinc
            //  loopinc: incexpression(); return loop;
            //incr();
        }
        public dynamic Pop() => instructions.Pop();

        public void Push(dynamic dynamic) => instructions.Push(dynamic);

        public dynamic Current = null;
        public bool MoveNext()
        {
            bool result = instructions.Count > 0;
            if (result)
                Current = Pop();
            //Execute(Pop());
            return result;
        }
        internal void Execute(dynamic current)
        {
            current();
        }
    }
    public class ControlFlowUsingStackInstructions
    {
        public static void PseudoCode()
        {
            var loopStack = new StackLoopDemo();
            dynamic current = null;
            while ((current = loopStack.Pop()))
            {
                loopStack.Execute(current);
            }
        }

    }
    public class ControlFlow
    {
        public static void Test()
        {

            List<InstructionElement> instructions = GetInstructions();


        }

        private static List<InstructionElement> GetInstructions()
        {

            var result = new List<InstructionElement>();

            result.Add(InstructionElement.Noop);
            result.Add(InstructionElement.Loop);
            for (var i = 0; i < result.Count - 1; i++)
                result[i].Next = result[i + 1];


            return result;
        }
    }
    public class InstructionEngine
    {
        public void Execute(List<InstructionElement> instructions)
        {
            var current = instructions.First();

            Execute(current);// two paths, recursive => Execute(current) & Execute(current?.Next?? InstructionElement.Ret)

            //Path 1: recursive. Every instruction requires a recursive stack frame/
            Action<InstructionElement> RecursiveExecute = null;
            RecursiveExecute = (instruction) => { Console.WriteLine(instruction); RecursiveExecute(instruction?.Next ?? InstructionElement.Noop); };

            Action<InstructionElement> ExecuteSingle = (instruction) => Console.WriteLine(instruction);
            //Path 2: Iterate. Requires execution engine that can enumerate instructions. Chickent v Egg.
            instructions.ForEach(x => ExecuteSingle(x));

            //Path 3: interate with recursion: Requires execution engine that can enumerate instructions.
            instructions.ForEach(x => RecursiveExecute(x));


            // Logic=> Init(current, destination); LoopBody = ()=>  MeasureError && LoopBody()

            //appears to be stack based;
            dynamic currentState = (object)null;
            dynamic destinationState = (object)null;

            Action<dynamic, dynamic> InitLoop = (a, b) => { currentState = a; destinationState = b; };
            Func<dynamic, dynamic, bool> MeasureError = (a, b) => a.CompareTo(b) == 0;

            Func<InstructionElement, bool> ExecuteControlled = null;
            ExecuteControlled = (instruction) => { Console.WriteLine(instruction); return MeasureError(currentState, destinationState) || ExecuteControlled(instruction); };



            //set current
            //set target

            //LoopContainer = () => {MeasureError() !=0 && LoopBody(); Loop()};
            //Loop => () => {BoolExpression()? LoopBody(); Loop() : Ret();};

            //Initially there is no ability enumerate a set even the set of numbers.
            //given boolean logic map outputs from fixed[input]=> set[0,..n] can repeatedly evaluate input to generate next set but need to swith from one map to the other which again requires 
            // ability to enumerate set.
            // Example; Fixed input [set boolean pairs]-> [0,0],[0,1],[1,0],[1,1]; 16 mappings exist for which each input pair maps to an output bit set{0..15}
            //  Maybe need to hand code instruction that can be extended to higher instructions for enumeration:
            //  op_enum_unary_bits_0_15:
            //      setloc.0 = 0
            //      setloc.1 [0,0];
            //      setloc.2 [0,1];
            //      setloc.3 [1,0];
            //      setloc.4 [1,1];

            //      yield return [eval(loc.0, loc.1),eval(loc.0, loc.2),eval(loc.0, loc.3),eval(loc.0, loc.4)]
            //      inc loc.0;//i=1;
            //      yield return [eval(loc.0, loc.1),eval(loc.0, loc.2),eval(loc.0, loc.3),eval(loc.0, loc.4)]
            //      inc loc.0;//i=2;
            //      yield return [eval(loc.0, loc.1),eval(loc.0, loc.2),eval(loc.0, loc.3),eval(loc.0, loc.4)]
            //      inc loc.0;//i=3;
            //      yield return [eval(loc.0, loc.1),eval(loc.0, loc.2),eval(loc.0, loc.3),eval(loc.0, loc.4)]

            //  -> but yeild requires a state machine as a compiler service.
            //  
            //  State machine:
            //      set.loc.0: @var_0; // i=> current
            //      set.loc.1: @var_1; // i=> dest
            //      




        }

        public static void TestLambaIterativeLoop()
        {
            var loopState = 0;
            var loopTarget = 10;
            Func<bool> LoopControl = () => loopState < loopTarget;


            Action LoopBody = () => { Console.WriteLine(loopState++); };
            Action LoopExit = () => { Console.WriteLine("\tRet"); };
            Action LoopEnter = null;
            Action Loop = () => (LoopControl() ? LoopEnter : LoopExit)();
            LoopEnter = () => { LoopBody(); Loop(); };


            Loop();
            //    if (LoopControl())
            //{
            //    LoopEnter();
            //    LoopBody();
            //    LoopExit();
            //}

        }

        private void Execute(InstructionElement current)
        {
            Console.WriteLine(current);
            if (current.Next != null)
            {
                Execute(current.Next);
            }
        }
    }
    public class InstructionElement
    {
        public InstructionElement Next;

        public static readonly InstructionElement Noop = new InstructionElement("Noop");
        public static readonly InstructionElement Loop = new InstructionElement("Loop");
        public static readonly InstructionElement Ret = new InstructionElement("Ret");
        public string Name { get; }

        public InstructionElement(string name)
        {
            this.Name = name;
        }
        public override string ToString() => Name;
    }
}
