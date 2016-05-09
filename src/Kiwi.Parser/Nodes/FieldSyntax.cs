using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class FieldSyntax : ISyntaxBase
    {
        public FieldSyntax(Token qualifier, Token name, IExpressionSyntax initializer)
        {
            Qualifier = qualifier;
            Name = name;
            Initializer = initializer;
        }

        public Token Qualifier { get; }
        public Token Name { get; }
        public IExpressionSyntax Initializer { get; }

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