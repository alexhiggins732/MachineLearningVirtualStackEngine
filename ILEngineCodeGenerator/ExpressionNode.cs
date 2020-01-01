using System;

namespace ILEngine.CodeGenerator
{
    public class ExpressionNode : IExpressionNode
    {
        public Func<string> Body { get; set; }
        public IExpressionNode Parent { get; set; }
        //Returns the depthc of the node in the expression tree or -1 if the expression the root of the tree. All children within the root will have a depth of zero.
        public int NodeDepth => this.Parent == null ? -1 : this.Parent.NodeDepth + 1;
        public override string ToString() => Body();
    }


}
