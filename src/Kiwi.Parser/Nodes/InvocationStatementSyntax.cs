namespace Kiwi.Parser.Nodes
{
    public class InvocationStatementSyntax : IStatementSyntax
    {
        public InvocationStatementSyntax(InvocationExpressionSyntax invocationExpression)
        {
            InvocationExpression = invocationExpression;
        }

        public InvocationExpressionSyntax InvocationExpression { get; private set; }

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }

        public TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}