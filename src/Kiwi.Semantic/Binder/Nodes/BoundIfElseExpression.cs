using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundIfElseExpression : BoundExpression
    {
        public BoundExpression Condition { get; }
        public BoundExpression IfTrue { get; }
        public BoundExpression IfFalse { get; }

        public BoundIfElseExpression(BoundExpression boundCondition, BoundExpression ifTrue, BoundExpression ifFalse, IType type, IfElseExpressionSyntax ifElseExpressionSyntax) : base(ifElseExpressionSyntax, type)
        {
            Condition = boundCondition;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }
    }
}