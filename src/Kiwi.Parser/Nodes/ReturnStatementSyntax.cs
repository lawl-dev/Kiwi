namespace Kiwi.Parser.Nodes
{
    public class ReturnStatementSyntax : IStatementSyntax
    {
        public IExpressionSyntax Expression { get; }
        public SyntaxType SyntaxType => SyntaxType.ReturnStatementSyntax;

        public ReturnStatementSyntax(IExpressionSyntax expression)
        {
            Expression = expression;
        }
        
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