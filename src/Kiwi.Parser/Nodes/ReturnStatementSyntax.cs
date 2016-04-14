namespace Kiwi.Parser.Nodes
{
    public class ReturnStatementSyntax : IStatementSyntax
    {
        public ReturnStatementSyntax(IExpressionSyntax expression)
        {
            Expression = expression;
        }

        public IExpressionSyntax Expression { get; }
        public SyntaxType SyntaxType => SyntaxType.ReturnStatementSyntax;

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