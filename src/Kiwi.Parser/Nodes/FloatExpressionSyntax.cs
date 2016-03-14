using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class FloatExpressionSyntax : IExpressionSyntax
    {
        public Token Value { get; }

        public FloatExpressionSyntax(Token value)
        {
            Value = value;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}