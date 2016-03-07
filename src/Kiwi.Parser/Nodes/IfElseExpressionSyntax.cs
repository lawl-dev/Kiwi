namespace Kiwi.Parser.Nodes
{
    public class IfElseExpressionSyntax : IExpressionSyntax
    {
        public IExpressionSyntax Condition { get; }
        public IExpressionSyntax IfTrueExpression { get; }
        public IExpressionSyntax IfFalseExpression { get; }

        public IfElseExpressionSyntax(IExpressionSyntax condition, IExpressionSyntax ifTrueExpression, IExpressionSyntax ifFalseExpression)
        {
            Condition = condition;
            IfTrueExpression = ifTrueExpression;
            IfFalseExpression = ifFalseExpression;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}