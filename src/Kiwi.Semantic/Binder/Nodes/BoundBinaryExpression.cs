using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.CompilerGeneratedNodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundBinaryExpression : BoundExpression
    {
        public BoundExpression Left { get; }
        public BoundExpression Right { get; }
        public BinaryOperators BinaryOperators { get; }

        public BoundBinaryExpression(BoundExpression left, BoundExpression right, BinaryOperators binaryOperators, BinaryExpressionSyntax binaryExpressionSyntax, IType type) : base(binaryExpressionSyntax, type)
        {
            Left = left;
            Right = right;
            BinaryOperators = binaryOperators;
        }
    }
}