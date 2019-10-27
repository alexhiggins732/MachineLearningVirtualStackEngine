using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TspStack
{
    public class TspNetworkSolver
    {
        public static void Run()
        {
            int nodeCount = 3;
            var network = new Network(nodeCount);
            var routeDiscoveryRequest = network.CreateDiscoveryRequest();
            var originator = network.Nodes.First();
            originator.SendRequest(routeDiscoveryRequest);
        }
    }
    public class Network : INetwork
    {

        public Network(int nodeCount)
        {
            this.Nodes = new List<IRouteNode>();
            for (var n = 0; n < nodeCount; n++)
            {
                Nodes.Add(new RouteNode(n));
            }
        }

        public Network(List<IRouteNode> nodes)
        {
            this.Nodes = nodes.ToList();
        }

        public List<IRouteNode> Nodes { get; private set; }

        public IRouteDiscoveryRequest CreateDiscoveryRequest()
        {
            return new RouteDiscoveryRequest(this);
        }
    }

    public class RouteNode : IRouteNode
    {
        public int NodeIndex { get; set; }

        public RouteNode(int nodeIndex)
        {
            this.NodeIndex = nodeIndex;
        }

        public void SendRequest(IRouteDiscoveryRequest routeDiscoveryRequest)
        {
            var stack = routeDiscoveryRequest.RouteStack;
            stack.Add(NodeIndex);
            foreach (var nodeIndex in stack.UnvisitedNodeIndexes())
            {
                var node = routeDiscoveryRequest.Network.Nodes[nodeIndex];
                node.SendRequest(routeDiscoveryRequest);
            }
            stack.Remove(NodeIndex);
        }


    }

    public interface INetwork
    {
        List<IRouteNode> Nodes { get; }
    }

    public class RouteDiscoveryRequest : IRouteDiscoveryRequest
    {

        public RouteDiscoveryRequest(Network network)
        {
            this.Network = network;
            this.OriginatingNode = network.Nodes.First();
            this.RouteStack = new BitArrayStack(network.Nodes.Count);
        }
        public INetwork Network { get; set; }

        public IRouteStack RouteStack { get; set; }
        public IRouteNode OriginatingNode { get; set; }
        public IRouteNode PreviousNode { get; set; }
        public int DistanceToOriginator { get; set; }
        public int DistanceToPrevious { get; set; }
    }

    public class BitArrayStack : IRouteStack
    {
        private BitArray bits;
        public int Count => bits.Count;
        public int CurrentDepth { get; private set; }
        public bool this[int index] { get => bits[index]; set => bits[index] = value; }

        private IEnumerable<int> range;

        public BitArrayStack(int nodeCount) { bits = new BitArray(nodeCount); range = Enumerable.Range(0, nodeCount); }

        public void Add(int i)
        {
            if (bits.Get(i))
                throw new InvalidOperationException($"{i} is already on the stack");
            bits.Set(i, true);
            CurrentDepth++;
        }

        public void Remove(int i)
        {
            if (bits.Get(i)==false)
                throw new InvalidOperationException($"{i} is not on the stack");
            bits.Set(i, false);
            CurrentDepth--;
        }

        public IEnumerable<int> UnvisitedNodeIndexes()
        {
            return range.Where(i => bits[i] == false);
        }

        public IEnumerable<int> VisitedNodeIndexes()
        {
            return range.Where(i => bits[i]);
        }
    }

    public interface IRouteNode
    {
        int NodeIndex { get; set; }
        void SendRequest(IRouteDiscoveryRequest routeDiscoveryRequest);
    }
    public interface IRouteStack
    {
        void Add(int i);
        void Remove(int i);
        IEnumerable<int> UnvisitedNodeIndexes();
        IEnumerable<int> VisitedNodeIndexes();
        int Count { get; }
        int CurrentDepth { get; }
        bool this[int index] { get; set; }

    }


    public interface IRouteDiscoveryRequest
    {
        INetwork Network { get; set; }
        IRouteStack RouteStack { get; set; }
        IRouteNode OriginatingNode { get; set; }
        IRouteNode PreviousNode { get; set; }
        int DistanceToOriginator { get; set; }//distance to originator
        int DistanceToPrevious { get; set; }//distance to previous node
    }

    /*
     *Functionality: 
     *  Requirements of send,recieve differ depend on algorithms order of invoking calls.
     *  Requirements could be entirely different if we assume parralel invocation:
     *      EG:, GPU.ParralelSearch(0,N) invoke a root search from each node at the same time.
      Node[0] sends requests to Nodes[1]..Nodes[1..N-1];
            Node.CreateRequest();
            Node.SendRequest();

            N[1] recieves request:
                Incoming Total Distance-> Who is responsible?
                Incoming Hop Distance-> Whote is response?
                N[1] Sends requests to Nodes[2]..Nodes[1..N-1];
                    Does N[1] send request back to N[0]? aren't we trying to prevent?

             N[2] recieves request:
                Incoming Total Distance-> Who is responsible?
                Incoming Hop Distance-> Who is response?
                N[2] Sends requests to Nodes[3]..Nodes[1..N-1];
                    Does N[2] send request back to N[0] and N[1]? aren't we trying to prevent?
                    
                Assumption is (but not clear) nodes need to store partial distances as "cache"
                    For example, if we allow 0...N to be originators to assure outbound is cleared
                        We get to SearchFrom(2) at which point (2) still has 0 in its outbound.
                        .. Likewise, with same logic 1 potentially will have (0) as an outbound.

                        In optimal search solution 0,2,1,3 is unsearched (depending on order).


                        No? -> N[2] can determine distance to it from originator via callstack.
                                it can store the callstack as a key.
                                    Receive[0] stack [0,1] <-- eliminates need to send to 0.
                                    Receive[1] stack [0] <-- eliminates need to send to 0.
                                -> Will this result in excessive memory use and is it even needed?
                        Yes?
                            -> Benefit is single logic flow (SendRequest) logic to update stack
                                -> Will this result in excessive memory usage?
                                -> Is it needed.
            ...

           N[n-1] recieves request.

        
      */
}
