using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.CompilerGeneratedNodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundIntExpression : BoundExpression
    {
        public BoundIntExpression(IntExpressionSyntax expressionSyntax)
            : base(expressionSyntax, new IntCompilerGeneratedType())
        {
        }
    }
}