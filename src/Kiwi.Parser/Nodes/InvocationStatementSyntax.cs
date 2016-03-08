namespace Kiwi.Parser.Nodes
{
    public class InvocationStatementSyntax : IStatementSyntax
    {
        public InvocationExpressionSyntax InvocationExpression { get; private set; }

        public InvocationStatementSyntax(InvocationExpressionSyntax invocationExpression)
        {
            InvocationExpression = invocationExpression;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}