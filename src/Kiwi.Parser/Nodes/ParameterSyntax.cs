using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class ParameterSyntax : ISyntaxBase
    {
        public ParameterSyntax(TypeSyntax type, Token name)
        {
            Type = type;
            Name = name;
        }

        public TypeSyntax Type { get; }
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