using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILEngine.CodeGenerator
{
    public class CsScopedExpression : ExpressionNode
    {
        public List<IExpressionNode> PreBodyExpressions = new List<IExpressionNode>();
        public List<IExpressionNode> Children = new List<IExpressionNode>();
        public List<IExpressionNode> PostBodyExpressions = new List<IExpressionNode>();

        public List<IExpressionNode> Scopes;


        public CsScopedExpression AddPreBodyExpression(IExpressionNode expression)
        {
            expression.Parent = this.Parent;
            PreBodyExpressions.Add(expression);
            return this;
        }
        public CsScopedExpression AddPostBodyExpression(IExpressionNode expression)
        {
            expression.Parent = this.Parent;
            PostBodyExpressions.Add(expression);
            return this;
        }

        private CsExpressionBuilder AddChildExpression(CsExpressionBuilder expression)
        {
            expression.Parent = expression.Parent ?? this.Parent;
            this.Children.Add(expression);
            return expression;
        }
        public CsExpressionBuilder CreateChildExpression()
        {
            var result = new CsExpressionBuilder();
            return AddChildExpression(result);
        }
        public CsScopedExpression CreateScopedChild() => CreateChildExpression().CreateScope();

        public CsScopedExpression AddPreBodyExpression(string value) => AddPreBodyExpression(new TextExpression(value));

        public CsScopedExpression AddNamespaceDeclaration(string value) => AddPreBodyExpression(new TextExpression($"namespace {value}\r\n"));
        public CsScopedExpression AddPostBodyExpression(string value) => AddPostBodyExpression(new TextExpression(value));

        public CsScopedExpression AddUsing(string importedNamesSpace)
        {
            return AddPreBodyExpression($"using {importedNamesSpace};\r\n");
        }
        public CsScopedExpression AddUsings(params string[] importedNamesSpace)
        {
            foreach (var importedNameSpace in importedNamesSpace)
            {
                AddUsing(importedNameSpace);
            }
            return this;
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var exp in PreBodyExpressions)
            {
                var expressionString = exp.ToString();
                sb.Append(expressionString);
            }

            foreach (var child in Children)
            {
                var expressionString = child.ToString();
                sb.Append(expressionString);
            }
            foreach (var exp in PostBodyExpressions)
            {
                var expressionString = exp.ToString();
                sb.Append(expressionString);
            }

            var result = sb.ToString();
            return result;
        }

    }


}
