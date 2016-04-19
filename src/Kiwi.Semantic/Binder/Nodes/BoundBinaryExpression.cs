using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.CompilerGeneratedNodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundBinaryExpression : BoundExpression
    {
        public BoundExpression Left { get; }
        public BoundExpression Right { get; }
        public BinaryOperators Operator { get; }

        public BoundBinaryExpression(BoundExpression left, BoundExpression right, BinaryOperators @operator, BinaryExpressionSyntax binaryExpressionSyntax, IType type) : base(binaryExpressionSyntax, type)
        {
            Left = left;
            Right = right;
            Operator = @operator;
        }
    }
}