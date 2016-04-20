using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.CompilerGeneratedNodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundIntExpression : BoundExpression
    {
        public int Value { get; }

        public BoundIntExpression(int value, IntExpressionSyntax expressionSyntax)
            : base(expressionSyntax, new IntCompilerGeneratedType())
        {
            Value = value;
        }
    }
}