using Dapper;
using ILEngine;
using ILEngine.Implementations;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UIR
{
    public class QOpCodeLearingGenerator
    {
        public static List<OpCode> OpCodes = null;
        public static List<ushort> OpValuesUnsigned = null;
        public static List<short> OpCodeValues = null;
        public static Random rnd = null;
        public static List<int> OpIndexes;
        public static int RetIndex;
        public static Dictionary<string, OpCode> OpCodesByName;
        static QOpCodeLearingGenerator()
        {
            var opDict = OpCodesByName = OpCodeLookup.OpCodesByName;
            int retIndex = -1;
            int idx = 0;
            OpCodes = opDict.Select(x =>
            {
                if (x.Key == "Ret")
                {
                    retIndex = idx;
                }
                idx++;
                return x.Value;
            }).ToList();
            RetIndex = retIndex;
            OpValuesUnsigned = OpCodes.Select(x => (ushort)x.Value).ToList();
            OpCodeValues = OpCodes.Select(x => (short)x.Value).ToList();
            OpIndexes = Enumerable.Range(0, OpCodes.Count).ToList();

        }





    }

    public class Tester
    {
        internal static void TestStackFrameBuilder()
        {
            var opcodes = new List<OpCode>
            {
                OpCodes.Nop,
            };


            var result = IlStackFrameBuilder.BuildAndExecute(opcodes);

            opcodes.Add(OpCodes.Ldarg_0);
           
            var result2 = IlStackFrameBuilder.BuildAndExecute(opcodes, args:new object[] { 1 });
            System.Diagnostics.Debug.Assert(((int)result2.ReturnResult) == 1);
        }
    }

    public class QOpCodeLearningProgram
    {
        static Random rnd = new Random(1);
        public static void Run()
        {
            var t = Type.GetType("System.Object");
            var defaults = Activator.CreateInstance(t);
            var floatName = typeof(float).FullName;
            Tester.TestStackFrameBuilder();
            Console.WriteLine("Beginning q-learning maze");

            Console.WriteLine("Setting up state");



            int numStates = QOpCodeLearingGenerator.OpCodes.Count;
            var qMaze = QMaze.CreateDemo(numStates); //CreateMaze(numStates);
            qMaze.Start = 0;
            double[][] rewardMatrix = CreateRewards(qMaze.NumStates);
            double[][] qualityMaxtrix = CreateQuality(qMaze.NumStates);

            qMaze.Goal = QOpCodeLearingGenerator.RetIndex;// 11;
            double gamma = .5;//discount factor
            double learnRate = .5;
            int maxEpochs = 100000;

            var args = new dynamic[] { "hello world" };
            var expected = args[0];
            Train(qMaze, rewardMatrix, qualityMaxtrix, qMaze.Goal, gamma, learnRate, maxEpochs, expected, args);

            Console.WriteLine("Done.");
            //Print(qualityMaxtrix);

            Console.WriteLine("Solution");

            Walk(qMaze, qualityMaxtrix);

            Console.WriteLine("End demo");
            Console.ReadLine();

        }

        /// <summary>
        /// After the quality matrix has been computed, 
        /// it can be used to find an optimal path from any starting state to the goal state.
        /// the method assumes that the goal state is reachable from the starting state
        /// </summary>
        /// <param name="qMaze"></param>
        /// <param name="quality"></param>
        private static void Walk(QMaze qMaze, double[][] quality)
        {
            int curr = qMaze.Start; int next;

            /* original algorithm assumed fixed start. removed due to needing random sequence.
             * this however prevents the ability to walk the maze after the solution is found because.
             * the alogrithm does not set quality values for the starting rank.
             * instead, will use no-op as the fixed staring pointing*/
            /*
           int maxI = 0;
           int maxK = 0;
           var bestQ = double.MinValue;
           for (var i = 0; i < quality.Length; i++)
           {
               for (var k = 0; k < quality[i].Length; k++)
               {
                   if (quality[i][k] > bestQ)
                   {
                       bestQ = quality[i][k];
                       maxI = i;
                       maxK = k;
                   }
               }
           }
           var opCodeI = QOpCodeLearingGenerator.OpCodes[maxI];
           var opCodeK = QOpCodeLearingGenerator.OpCodes[maxK];
           */
            var opCode = QOpCodeLearingGenerator.OpCodes[curr];
            List<OpCode> solution = new List<OpCode>();
            solution.Add(opCode);
            Console.Write(opCode + "->");
            while (curr != qMaze.Goal)
            {
                next = ArgMax(quality[curr]);
                opCode = QOpCodeLearingGenerator.OpCodes[next];
                solution.Add(opCode);
                Console.Write(opCode + "->");
                curr = next;
            }

            var writer = new IlInstructionWriter(solution);
            List<IlInstruction> ilInstructions = writer.GetInstructionStream();
            var engine = new IlInstructionEngine();
            dynamic[] args = new dynamic[] { 1 };
            var result = engine.ExecuteTyped(ilInstructions, args: args);
            Console.WriteLine("done");
        }

        /// <summary>
        /// Used find the best next state for example For example, if a vector has values (0.5, 0.3, 0.7, 0.2) then ArgMax returns 2. 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static int ArgMax(double[] vector)
        {
            double maxVal = vector[0]; int idx = 0;
            for (int i = 0; i < vector.Length; ++i)
            {
                if (vector[i] > maxVal)
                {
                    maxVal = vector[i]; idx = i;
                }
            }
            return idx;
        }

        private static void Print(double[][] quality)
        {
            int ns = quality.Length;
            Console.WriteLine("[0] [1] . . [11]");
            for (int i = 0; i < ns; ++i)
            {
                for (int j = 0; j < ns; ++j)
                {
                    Console.Write(quality[i][j].ToString("F2") + " ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// The key update equation for Q-learning is based on the mathematical Bellman equation
        /// </summary>
        /// <param name="qMaze"></param>
        /// <param name="rewards"></param>
        /// <param name="quality"></param>
        /// <param name="goal"></param>
        /// <param name="gamma"></param>
        /// <param name="learnRate"></param>
        /// <param name="maxEpochs"></param>
        private static void Train(QMaze qMaze,
            double[][] rewards,
            double[][] quality,
            int goal,
            double gamma,
            double learnRate,
            int maxEpochs,
            dynamic expectedResult,
            params dynamic[] args)
        {
            /*
loop maxEpochs times
    set currState = a random state
    while currState != goalState
        pick a random next-state but don't move yet
        find largest Q for all next-next-states
        update Q[currState][nextState] using Bellman
        move to nextState
    end-while
end-loop
      */

            var stack = new Stack<object>();
            IComparable rewardValue = expectedResult;
            int maxOpCodeLength = 10;
            for (int epoch = 0; epoch < maxEpochs; ++epoch)
            {
                int startState = rnd.Next(0, rewards.Length);

                var currentOpCode = QOpCodeLearingGenerator.OpCodes[startState];


                Console.Title = $"Epoch {epoch} of {maxEpochs} : {currentOpCode.Name}";
                //Console.WriteLine($"testing {currentOpCode}");
                if (currentOpCode.Name == ILOpCodeValueNativeNames.Ldc_I4_1)
                {
                    string bp = "";
                }
                var l = new List<OpCode>();
                l.Add(currentOpCode);
                //The number of training epochs must be determined by trial and error. 
                // An alternative design is to iterate until the values in
                // the Q matrix don’t change, or until they stabilize to very small changes
                //per iteration. 
                int currState = startState;
                while (true && l.Count < maxOpCodeLength)
                {
                    int nextState = GetRandNextState(currState, qMaze);
                    var opCode = QOpCodeLearingGenerator.OpCodes[nextState];
                    l.Add(opCode);
                    //TODO: make this smarter
                    //List<int> possNextNextStates = GetPossibleNextStates(nextState, qMaze);
                    List<int> possNextNextStates = QOpCodeLearingGenerator.OpIndexes;
                    double maxQ = double.MinValue;
                    for (int j = 0; j < possNextNextStates.Count; ++j)
                    {
                        int nns = possNextNextStates[j];  // short alias
                        double q = quality[nextState][nns];
                        if (q > maxQ) maxQ = q;
                    }
                    /*
                     Imagine you’re in a maze. You see that you can go to three different rooms, A, B, C. 
                     You pick B, but don’t move yet. 
                     You ask a friend to go into room B and the friend tells you
                     that from room B you can go to rooms X, Y, Z and that of those
                     rooms Y has the best Q value. In other words, Y is the best next-next state.
                     * */


                    ////refactor to evaluate if would return reward.
                    /*
                    quality[currState][nextState] =
                        ((1 - learnRate) * quality[currState][nextState]) +
                        (learnRate * (rewards[currState][nextState] + (gamma * maxQ)));
                    */

                    double reward = -.1;
                    if (nextState == QOpCodeLearingGenerator.RetIndex)
                    {
                        var result = ExecuteOpCodes(l, timeoutSeconds: 3, args);
                        if (result.Error != null) // need to penalize errors.
                        {
                            reward = -.2;
                        }
                        if (result != null)
                        {
                            if (result.Success && result.Result != null)
                            {
                                var type = result.Result.GetType();
                                try
                                {
                                    if (result.Result == expectedResult)
                                    {
                                        reward = 1;
                                        var rewardSteps = string.Join(", ", l.ToArray());
                                        Console.WriteLine($"Found reward {rewardSteps}.");
                                    }
                                }
                                catch (Exception ex)
                                {

                                }


                            }
                        }
                    }

                    quality[currState][nextState] =
                        ((1 - learnRate) * quality[currState][nextState]) +
                        (learnRate * (reward + (gamma * maxQ)));



                    currState = nextState;
                    if (currState == goal)
                    {
                        break;
                    }
                    /*
                     The update equation has two parts. 
                     The first part, ((1 - lrnRate) * Q[currState][nextState]), is called the exploit component 
                     and adds a fraction of the old value. 

                    The second part, (lrnRate * (R[currState][nextState] + (gamma * maxQ))),
                    is called the explore component. 

                    Larger values of the lrnRate increase the influence of both current rewards and
                    future rewards (explore) at the expense of past rewards (exploit). 
                    The value of gamma, the discount factor, influences the importance of future rewards.
                     * */

                }
            }
        }


        public static void TestExecution()
        {
            ILRuntimeDbGenerator.DbSeeder.SeedOpCodesDocs();
            var args = (new dynamic[] { 1, 2 });
            var opcodes = new List<OpCode>();
            opcodes.Add(OpCodes.Ldarg_0);
            opcodes.Add(OpCodes.Ldarg_1);
            opcodes.Add(OpCodes.Add);
            opcodes.Add(OpCodes.Ret);
            var result = ExecuteOpCodes(opcodes, args);
        }
        private static ExecutionResult ExecuteOpCodes(List<OpCode> l, params dynamic[] args)
        {
            return ExecuteOpCodes(l, 3, args);
        }

        private static ExecutionResult ExecuteOpCodes(List<OpCode> l, int timeoutSeconds = 3, params dynamic[] args)
        {
            ExecutionResult result = new ExecutionResult();

            try
            {
                var executionOpCodes = l.ToArray().Concat(new[] { OpCodes.Ret }).ToList();
                var writer = new IlInstructionWriter(executionOpCodes);
                List<IlInstruction> instructions = writer.GetInstructionStream();
                result.ExecutionState = new ExecutionState() { ilInstructions = instructions, Arguments = args };
                TimeSpan timeout = TimeSpan.FromSeconds(timeoutSeconds);
                using (var src = new CancellationTokenSource())
                {

                    var startParams = new ParameterizedThreadStart(ExecuteInstructions);
                    var t = new Thread(startParams);
                    t.Start(result.ExecutionState);
                    var threadTask = Task.Run(() => t.Join());
                    Task.WaitAny(threadTask, Task.Delay(timeout, src.Token));
                    if (threadTask.IsCompleted)
                    {
                        src.Cancel();
                        if (result.ExecutionState.Error != null)
                        {

                            result.Error = result.ExecutionState.Error;
                            result.TimedOut = result.ExecutionState.Error is TimeoutException;
                            result.Success = false;
                        }
                        else
                        {
                            result.Success = true;
                            result.Result = result.ExecutionState.Result;
                        }

                    }
                    else
                    {
                        t.Abort();
                        throw new ExecutionEngineTimeoutException(result.ExecutionState, timeout);
                    }

                }

                //return result;
            }
            catch (ExecutionEngineTimeoutException eete)
            {
                result.Error = eete;
                result.Success = false;

                System.Diagnostics.Debug.WriteLine($"Error executing opcodes: {eete.Message}");
            }
            catch (ExecutionEngineException engExc)
            {
                result.Error = engExc;
                result.Success = false;
                System.Diagnostics.Debug.WriteLine($"Error executing opcodes: {engExc.InnerException.Message}");
            }
            catch (Exception ex)
            {
                result.Error = ex;
                result.Success = false;
                System.Diagnostics.Debug.WriteLine($"Error executing opcodes: {ex.Message}");
            }
            return result;
        }

        public class ExecutionResult : ExecutionState
        {

            public bool Success;

            public bool TimedOut;
            public ExecutionState ExecutionState;
        }

        public class ExecutionState
        {
            public List<IlInstruction> ilInstructions;
            public dynamic[] Arguments;
            public dynamic Result;
            public Exception Error;

        }
        public class ExecutionEngineTimeoutException : Exception
        {
            public ExecutionState State { get; }
            public TimeSpan Timeout { get; }
            public ExecutionEngineTimeoutException(ExecutionState state, TimeSpan timeout) : base($"The execution engined timedout after {timeout}")
            {
                this.State = state;
                this.Timeout = timeout;
            }
        }
        public class ExecutionEngineException : Exception
        {
            public ExecutionState State { get; }
            public ExecutionEngineException(string message, Exception innerException, ExecutionState state) : base(message, innerException)
            {
                this.State = state;
            }
        }
        private static void ExecuteInstructions(object obj)
        {
            var state = (ExecutionState)obj;
            var engine = new IlInstructionEngine();
            try
            {

                state.Result = engine.ExecuteTyped(state.ilInstructions, args: state.Arguments);
            }
            catch (Exception ex)
            {
                state.Error = ex;
                //throw new ExecutionEngineException("Engine execution failed", ex, state);
            }

        }

        /// <summary>
        ///  Q-learning is to find the value of the Q matrix. Initially, all Q values are set to 0.0 and the Q matrix is created like so:
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        private static double[][] CreateQuality(int ns)
        {
            double[][] Q = new double[ns][];
            for (int i = 0; i < ns; ++i)
                Q[i] = new double[ns];
            return Q;
        }

        /*
	    Q[0,...,N]
		    reward matrix is (1/n) for the sequence.
	    step 0:
		    choose:s
		    Q[s,[]];
			    naive algorithm puts -.1 at each block to penalize.
				    given random length can we use initital [1.0/depth] reward. No prioritization.?
				    int naive rewards stay static. but without a 'max step' policy how can we prioritize.
				    or does it matter. probably doesn't. a constant penalty for each step should suffice in the search for the highest quality solution
				    as long as we don't overflow the penalty.
                naive algorithm embeds the reward in the final state.
                    using a stack for qlearning we can't do this.
                    algo needs to decide:
                        1) ret with the proper value.
                        2) can we put ret {ret {returnresult}} as the reward somehow?
            we can like replace reward with a constant -0.1 step penalty.
                        
        */

        /// <summary>
        ///  moving to goal-cell 11 gives a reward of 10.0, but any other move gives a negative reward of -0.1
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        private static double[][] CreateRewards(int ns)
        {
            double[][] R = new double[ns][];
            for (int i = 0; i < ns; ++i)
            {
                var rewards = new double[ns];
                R[i] = rewards;
                for (var k = 0; k < ns; k++)
                {
                    rewards[k] = -.1;
                }
            }
            return R;
        }

        private static QMaze CreateMaze(int ns)
        {
            return QMaze.CreateDemo(ns);
        }

        /// <summary>
        /// Q-learning algorithm needs to know what states the system can transition to, given a current state. In this example, a state of the system is the same as the location in the maze so there are only 12 states
        /// </summary>
        /// <param name="s">State of system in demoe is same as location</param>
        /// <param name="qMaze"></param>
        /// <returns>For example, if the current state s is 5, then GetPossNextStates returns a List<int> collection holding (1, 6, 9)</returns>
        public static List<int> GetPossibleNextStates(int s, QMaze qMaze)
        {
            var FT = qMaze.FT;
            List<int> result = new List<int>();
            for (int j = 0; j < FT.Length; ++j)
                if (FT[s][j] == 1) result.Add(j);
            return result;
        }



        /// <summary>
        /// The Q-learning algorithm sometimes goes from the current state to a random next state.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="qMaze"></param>
        /// <returns></returns>
        /// <remarks>So, if the current state s is 5, then GetRandNextState returns either 1 or 6 or 9 with equal probability (0.33 each)</remarks>
        public static int GetRandNextState(int s, QMaze qMaze)
        {
            List<int> possNextStates = GetPossibleNextStates(s, qMaze);
            int ct = possNextStates.Count;
            int idx = rnd.Next(0, ct);
            return possNextStates[idx];
        }


        public class QMaze
        {
            /// <summary>
            /// Feasible Transistions
            /// </summary>
            public int[][] FT;

            /// <summary>
            /// Number of States;
            /// </summary>
            public int NumStates;

            /// <summary>
            /// Starting Cell
            /// </summary>
            public int Start;
            /// <summary>
            /// target cell
            /// </summary>
            public int Goal;

            public static QMaze CreateDemo(int ns)
            {

                return new QMaze
                {
                    NumStates = ns,
                    Start = 8,
                    Goal = 11,
                    FT = DemoTransitions(ns)
                };
            }

            /// <summary>
            /// The method returns a matrix that defines allowable moves. For example, you can move from cell 4 to cell 8, but you can’t move from cell 4 to cell 5 because there’s a wall in the way. Recall that C# initializes int arrays to 0, so CreateMaze needs to specify only allowable moves. Notice that you can’t move from a cell to itself, except for the goal-cell 11.
            /// </summary>
            /// <param name="ns"></param>
            /// <returns></returns>
            private static int[][] DemoTransitions(int ns)
            {
                int[][] FT = new int[ns][];
                for (var i = 0; i < ns; i++)
                {
                    FT[i] = new int[ns];
                    for (var k = 0; k < ns; k++)
                    {
                        FT[i][k] = 1;
                    }
                }
                return FT;
                /*
                int[][] FT = new int[ns][];
                for (int i = 0; i < ns; ++i) FT[i] = new int[ns];
                FT[0][1] = FT[0][4] = FT[1][0] = FT[1][5] = FT[2][3] = 1;
                FT[2][6] = FT[3][2] = FT[3][7] = FT[4][0] = FT[4][8] = 1;
                FT[5][1] = FT[5][6] = FT[5][9] = FT[6][2] = FT[6][5] = 1;
                FT[6][7] = FT[7][3] = FT[7][6] = FT[7][11] = FT[8][4] = 1;
                FT[8][9] = FT[9][5] = FT[9][8] = FT[9][10] = FT[10][9] = 1;
                FT[11][11] = 1;  // Goal
                return FT;
                */
            }
        }
    }

    // need environment;
    // opcodes[0,...,n] are possible choices.
    // quality must be total reward/number of steps.
    //      this requires brute force and there is no proio
}
