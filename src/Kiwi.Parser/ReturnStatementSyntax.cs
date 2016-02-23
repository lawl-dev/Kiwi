namespace Kiwi.Parser
{
    public class ReturnStatementSyntax : IStatetementSyntax
    {
        public IExpressionSyntax Expression { get; }

        public ReturnStatementSyntax(IExpressionSyntax expression)
        {
            Expression = expression;
        }
    }
}