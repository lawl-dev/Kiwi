using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class MemberAccessExpressionSyntax : IExpressionSyntax
    {
        public MemberAccessExpressionSyntax(IExpressionSyntax owner, Token name)
        {
            Owner = owner;
            Name = name;
        }

        public IExpressionSyntax Owner { get; }
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