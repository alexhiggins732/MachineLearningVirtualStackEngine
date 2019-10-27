using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLearningBasic
{
    class Program
    {
        static void Main(string[] args)
        {
            QLearningProgram.Run();
        }
    }

    public class QLearningProgram
    {
        static Random rnd = new Random(1);
        public static void Run()
        {
            Console.WriteLine("Beginning q-learning maze");

            Console.WriteLine("Setting up state");

            int numStates = 12;
            var qMaze = QMaze.CreateDemo(12); //CreateMaze(numStates);
            double[][] rewardMatrix = CreateRewards(qMaze.NumStates);
            double[][] qualityMaxtrix = CreateQuality(qMaze.NumStates);

            int goal = 11;
            double gamma = .5;//discount factor
            double learnRate = .5;
            int maxEpochs = 1000;

            Train(qMaze, rewardMatrix, qualityMaxtrix, goal, gamma, learnRate, maxEpochs);

            Console.WriteLine("Done.");
            Print(qualityMaxtrix);

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
            Console.Write(curr + "->");
            while (curr != qMaze.Goal)
            {
                next = ArgMax(quality[curr]);
                Console.Write(next + "->");
                curr = next;
            }
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
        private static void Train(QMaze qMaze, double[][] rewards, double[][] quality, int goal, double gamma, double learnRate, int maxEpochs)
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

            for (int epoch = 0; epoch < maxEpochs; ++epoch)
            {
                int currState = rnd.Next(0, rewards.Length);
                //The number of training epochs must be determined by trial and error. 
                // An alternative design is to iterate until the values in
                // the Q matrix don’t change, or until they stabilize to very small changes
                //per iteration. 

                while (true)
                {
                    int nextState = GetRandNextState(currState, qMaze);
                    List<int> possNextNextStates = GetPossibleNextStates(nextState, qMaze);
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
                    quality[currState][nextState] =
                        ((1 - learnRate) * quality[currState][nextState]) +
                        (learnRate * (rewards[currState][nextState] + (gamma * maxQ)));
                    currState = nextState;
                    if (currState == goal) break;
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

        /// <summary>
        ///  moving to goal-cell 11 gives a reward of 10.0, but any other move gives a negative reward of -0.1
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        private static double[][] CreateRewards(int ns)
        {
            double[][] R = new double[ns][];
            for (int i = 0; i < ns; ++i) R[i] = new double[ns];
            R[0][1] = R[0][4] = R[1][0] = R[1][5] = R[2][3] = -0.1;
            R[2][6] = R[3][2] = R[3][7] = R[4][0] = R[4][8] = -0.1;
            R[5][1] = R[5][6] = R[5][9] = R[6][2] = R[6][5] = -0.1;
            R[6][7] = R[7][3] = R[7][6] = R[7][11] = R[8][4] = -0.1;
            R[8][9] = R[9][5] = R[9][8] = R[9][10] = R[10][9] = -0.1;
            R[7][11] = 10.0;  // Goal
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
                for (int i = 0; i < ns; ++i) FT[i] = new int[ns];
                FT[0][1] = FT[0][4] = FT[1][0] = FT[1][5] = FT[2][3] = 1;
                FT[2][6] = FT[3][2] = FT[3][7] = FT[4][0] = FT[4][8] = 1;
                FT[5][1] = FT[5][6] = FT[5][9] = FT[6][2] = FT[6][5] = 1;
                FT[6][7] = FT[7][3] = FT[7][6] = FT[7][11] = FT[8][4] = 1;
                FT[8][9] = FT[9][5] = FT[9][8] = FT[9][10] = FT[10][9] = 1;
                FT[11][11] = 1;  // Goal
                return FT;
            }
        }
    }
}

