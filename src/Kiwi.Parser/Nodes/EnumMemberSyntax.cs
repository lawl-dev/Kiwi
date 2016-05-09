using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class EnumMemberSyntax : ISyntaxBase
    {
        public EnumMemberSyntax(Token name, IExpressionSyntax initializer)
        {
            Name = name;
            Initializer = initializer;
        }

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