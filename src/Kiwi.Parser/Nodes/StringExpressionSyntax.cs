using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class StringExpressionSyntax : IExpressionSyntax, IConstExpression
    {
        public Token Value { get; }

        public StringExpressionSyntax(Token value)
        {
            Value = value;
        }
    }
}