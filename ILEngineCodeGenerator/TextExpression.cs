using System.Text;

namespace ILEngine.CodeGenerator
{
    public class TextExpression : ExpressionNode
    {
        public TextExpression(string expressionText)
        {
            this.Body = () =>
             {
                 var sb = new StringBuilder();
                 sb.AppendIndented(expressionText, this.NodeDepth);
                 return sb.ToString();
             };
        }

        public override string ToString() => Body();

    }


}
