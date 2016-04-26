using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundArrayAccessExpression : BoundExpression
    {
        public List<BoundExpression> Parameter { get; }

        public BoundArrayAccessExpression(List<BoundExpression> parameter, ArrayAccessExpressionSyntax syntax, IType type) : base(syntax, type)
        {
            Parameter = parameter;
        }
    }
}