using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class IntExpressionSyntax : IExpressionSyntax
    {
        public Token Value { get; }

        public IntExpressionSyntax(Token value)
        {
            Value = value;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}