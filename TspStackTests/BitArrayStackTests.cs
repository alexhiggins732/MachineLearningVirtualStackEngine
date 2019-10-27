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
    public class BitArrayStackTests
    {
        [TestMethod()]
        public void BitArrayStackTest()
        {
            var stack = new BitArrayStack(3);
            Assert.IsTrue(stack.Count == 3);
            Assert.IsTrue(stack[0] == false);
            Assert.IsTrue(stack[1] == false);
            Assert.IsTrue(stack[2] == false);
        }

        [TestMethod()]
        public void AddTest()
        {
            var stack = new BitArrayStack(3);
            stack.Add(0);
            Assert.IsTrue(stack[0] == true);
            Assert.IsTrue(stack[1] == false);
            Assert.IsTrue(stack[2] == false);

            stack.Add(1);
            Assert.IsTrue(stack[0] == true);
            Assert.IsTrue(stack[1] == true);
            Assert.IsTrue(stack[2] == false);

            stack.Add(2);
            Assert.IsTrue(stack[0] == true);
            Assert.IsTrue(stack[1] == true);
            Assert.IsTrue(stack[2] == true);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddDuplicate()
        {
            var stack = new BitArrayStack(3);
            stack.Add(0);
            stack.Add(0);
        }


        [TestMethod()]
        public void RemoveTest()
        {
            var stack = new BitArrayStack(3);
            stack.Add(0);
            stack.Add(1);
            stack.Add(2);
            Assert.IsTrue(stack[0] == true);
            Assert.IsTrue(stack[1] == true);
            Assert.IsTrue(stack[2] == true);

            stack.Remove(0);
            Assert.IsTrue(stack[0] == false);
            Assert.IsTrue(stack[1] == true);
            Assert.IsTrue(stack[2] == true);
            stack.Remove(1);
            Assert.IsTrue(stack[0] == false);
            Assert.IsTrue(stack[1] == false);
            Assert.IsTrue(stack[2] == true);

            stack.Remove(2);
            Assert.IsTrue(stack[0] == false);
            Assert.IsTrue(stack[1] == false);
            Assert.IsTrue(stack[2] == false);

        }


        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveDuplicate()
        {
            var stack = new BitArrayStack(3);
            stack.Add(0);
            stack.Remove(0);
            stack.Remove(0);
        }




        [TestMethod()]
        public void UnvisitedNodeIndexesTest()
        {
            var stack = new BitArrayStack(3);
            var unvisitedIndexes = stack.UnvisitedNodeIndexes();
            var expected = new[] { 0, 1, 2 };
            Assert.IsTrue(unvisitedIndexes.SequenceEqual(expected));
        }
    }
}