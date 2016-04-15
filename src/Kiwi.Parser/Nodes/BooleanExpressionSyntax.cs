using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class BooleanExpressionSyntax : IExpressionSyntax
    {
        public BooleanExpressionSyntax(Token value)
        {
            Value = value;
        }

        public Token Value { get; }

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