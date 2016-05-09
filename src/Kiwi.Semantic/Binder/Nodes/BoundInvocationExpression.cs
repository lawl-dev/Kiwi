using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundInvocationExpression : BoundExpression
    {
        public BoundInvocationExpression(
            BoundExpression toInvoke,
            List<BoundExpression> boundParameter,
            IExpressionSyntax expressionSyntax,
            IType type) : base(expressionSyntax, type)
        {
            ToInvoke = toInvoke;
            BoundParameter = boundParameter;
        }

        public BoundExpression ToInvoke { get; }
        public List<BoundExpression> BoundParameter { get; }
    }
}