using System.Collections.Generic;

namespace ILEngine
{
    public class ILOperandStack
    {
        public ILogger logger = null;
        Stack<object> stack = null;
        public ILOperandStack()
        {
            this.stack = new Stack<object>();
            this.logger = new NullLogger();
        }
        public void Push(object obj)
        {
            logger.Log($"{nameof(ILOperandStack)}.{nameof(Push)}({obj})");
            stack.Push(obj);
        }
        public object Pop()
        {
            object result = stack.Pop();
            logger.Log($"return {nameof(ILOperandStack)}.{nameof(Pop)}() = {result}");
            return result;
        }
        public object Peek()
        {
            object result = stack.Peek();
            logger.Log($"return {nameof(ILOperandStack)}.{nameof(Peek)}() = {result}");
            return result;
        }
        public int Count
        {
            get
            {
                return stack.Count;
            }
        }



    }
}
