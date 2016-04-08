using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class StringExpressionSyntax : IExpressionSyntax
    {
        public Token Value { get; }

        public StringExpressionSyntax(Token value)
        {
            Value = value;
        }

        public SyntaxType SyntaxType => SyntaxType.StringExpressionSyntax;

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