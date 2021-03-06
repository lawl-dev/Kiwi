using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class IdentifierExpressionSyntax : IExpressionSyntax
    {
        public IdentifierExpressionSyntax(Token name)
        {
            Name = name;
        }

        public Token Name { get; }

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