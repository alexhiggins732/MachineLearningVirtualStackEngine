using System;
using System.Collections.Generic;
using System.Linq;

namespace TspStack
{
    partial class Program
    {
        public class TspGraphSolver
        {
            internal static void RunTest()
            {

                var N = 3;
                var graph = new Graph(N);
                Solve(graph);
            }

            private static void Solve(Graph graph)
            {

                foreach (var point in graph.Points)
                    while (point.Oubound.Count > 0)
                        SearchDeep(graph, point);
            }

            private static void SearchDeep(Graph graph, TspPoint point)
            {
                graph.TotalCalls++;
                graph.CallStack.Add(point.n);

                var ob = point.Oubound.First(x => !graph.CallStack.Contains(x.n));

            }

            public class Graph
            {
                public int TotalCalls;
                public List<int> CallStack;

                public Graph(int n)
                {
                    this.CallStack = new List<int>();
                    this.Points = new List<TspPoint>();
                    for (var i = 0; i < n; i++)
                        Points.Add(new TspPoint(i));
                    for (var i = 0; i < n; i++)
                        Points[i].InitVisits(this);
                }



                public List<TspPoint> Points { get; private set; }
            }

            public class TspPoint : IEquatable<TspPoint>
            {
                public List<TspPoint> Inbound;
                public List<TspPoint> Oubound;
                public int n;
                public TspPoint(int n)
                {
                    this.n = n;
                }

                internal void InitVisits(Graph graph)
                {
                    this.Inbound = graph.Points.Where(x => x != this).ToList();
                    this.Oubound = Inbound.ToList();
                }
                public override string ToString()
                {
                    return $"{n}";
                }

                #region Equality

                public override bool Equals(object obj)
                {
                    return base.Equals(obj);
                }

                public bool Equals(TspPoint other)
                {
                    return other != null &&
                           n == other.n;
                }

                public override int GetHashCode()
                {
                    return -1685217063 + n.GetHashCode();
                }



                public static bool operator ==(TspPoint point1, TspPoint point2)
                {
                    return EqualityComparer<TspPoint>.Default.Equals(point1, point2);
                }

                public static bool operator !=(TspPoint point1, TspPoint point2)
                {
                    return !(point1 == point2);
                }

                #endregion
            }

            
        }
    }
}
