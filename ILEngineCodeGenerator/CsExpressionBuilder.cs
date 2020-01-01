using System.Collections.Generic;
using System.Linq;

namespace ILEngine.CodeGenerator
{
    public class CsExpressionBuilder : ExpressionNode
    {
        public List<CsScopedExpression> Expressions = new List<CsScopedExpression>();
        public CsScopedExpression CreateScope()
        {
            var expression = new CsScopedExpression();
            expression.Parent = this;
            this.Expressions.Add(expression);
            return expression;
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            foreach (var expression in Expressions)
            {
                var expressionText = expression.ToString();
                sb.Append(expressionText);
            }
            var result = sb.ToString();
            return result;
        }
    }
}
