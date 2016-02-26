using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class StringExpressionSyntax : IExpressionSyntax, IConstExpressionSyntax
    {
        public Token Value { get; }

        public StringExpressionSyntax(Token value)
        {
            Value = value;
        }
    }
}