using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundStringExpression : BoundExpression
    {
        public BoundStringExpression(string value, IType boundType, StringExpressionSyntax expressionSyntax)
            : base(expressionSyntax, boundType)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}