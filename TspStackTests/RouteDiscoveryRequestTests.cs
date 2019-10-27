using Microsoft.VisualStudio.TestTools.UnitTesting;
using TspStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TspStack.Tests
{
    [TestClass()]
    public class RouteDiscoveryRequestTests
    {
        [TestMethod()]
        public void RouteDiscoveryRequestTest()
        {
            var network = new Network(3);
            var discoveryRequest = new RouteDiscoveryRequest(network);

            Assert.IsNotNull(discoveryRequest.Network);
            Assert.IsNotNull(discoveryRequest.Network.Nodes);
            Assert.IsTrue(discoveryRequest.Network.Nodes.Count == network.Nodes.Count);

            Assert.IsNotNull(discoveryRequest.RouteStack);
            Assert.IsTrue(discoveryRequest.RouteStack.Count == network.Nodes.Count);
        }

        [TestMethod()]
        public void RouteDiscoveryRequestStackTest()
        {
            var network = new Network(3);
            var discoveryRequest = new RouteDiscoveryRequest(network);
            var stack = discoveryRequest.RouteStack.UnvisitedNodeIndexes();
            Assert.IsTrue(stack.Count() == network.Nodes.Count);
            Assert.IsTrue(discoveryRequest.Network.Nodes.Count == network.Nodes.Count);
        }
    }
}