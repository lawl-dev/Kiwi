using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class SignExpressionSyntax : IExpressionSyntax
    {
        public SignExpressionSyntax(Token @operator, IExpressionSyntax expression)
        {
            Operator = @operator;
            Expression = expression;
        }

        public Token Operator { get; }
        public IExpressionSyntax Expression { get; }

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