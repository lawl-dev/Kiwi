using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundBooleanExpression : BoundExpression
    {
        public BoundBooleanExpression(BooleanExpressionSyntax syntax) : base(syntax, new StandardType(StandardTypes.Bool))
        {
        }
    }
}