using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundIntExpression : BoundExpression
    {
        public BoundIntExpression(IntExpressionSyntax expressionSyntax, IType boundType)
            : base(expressionSyntax, boundType)
        {
        }
    }
}