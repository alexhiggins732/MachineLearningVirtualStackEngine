using System.Collections.Generic;
namespace ILEngine
{
    public struct IlCallStack
    {
        private Stack<object> evaluationStack;
        public Stack<object> EvaluationStack => evaluationStack ?? (evaluationStack = new Stack<object>());
    }
}
