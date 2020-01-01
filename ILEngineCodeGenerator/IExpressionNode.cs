using System;

namespace ILEngine.CodeGenerator
{
    public interface IExpressionNode
    {
        int NodeDepth { get; }
        Func<string> Body { get; set; }

        IExpressionNode Parent { get; set; }
    }


}
