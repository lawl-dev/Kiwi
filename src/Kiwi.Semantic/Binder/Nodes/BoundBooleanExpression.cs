using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundBooleanExpression : BoundExpression
    {
        public BoundBooleanExpression(bool value, IType type, BooleanExpressionSyntax syntax) : base(syntax, type)
        {
            Value = value;
        }

        public bool Value { get; }
    }
}