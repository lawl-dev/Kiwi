namespace Kiwi.Parser.Nodes
{
    public class InfixFunctionInvocationExpressionSyntax : IExpressionSyntax
    {
        public IExpressionSyntax Left { get; }
        public IdentifierExpressionSyntax Identifier { get; }
        public IExpressionSyntax Right { get; }

        public InfixFunctionInvocationExpressionSyntax(IExpressionSyntax left, IdentifierExpressionSyntax identifier, IExpressionSyntax right)
        {
            Left = left;
            Identifier = identifier;
            Right = right;
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