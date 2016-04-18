using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.CompilerGeneratedNodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundStringExpression : BoundExpression
    {
        public BoundStringExpression(string value, StringExpressionSyntax expressionSyntax)
            : base(expressionSyntax, new StringCompilerGeneratedType())
        {
            Value = value;
        }

        public string Value { get; }
    }
}