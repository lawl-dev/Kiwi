using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundInvocationExpression : BoundExpression
    {
        public BoundInvocationExpression(
            BoundExpression toInvoke,
            List<BoundExpression> boundParameter,
            InvocationExpressionSyntax expressionSyntax,
            IType type) : base(expressionSyntax, type)
        {
            ToInvoke = toInvoke;
            BoundParameter = boundParameter;
        }

        public BoundExpression ToInvoke { get; set; }
        public List<BoundExpression> BoundParameter { get; set; }
    }
}