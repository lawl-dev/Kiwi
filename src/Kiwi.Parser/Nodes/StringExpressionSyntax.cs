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

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}