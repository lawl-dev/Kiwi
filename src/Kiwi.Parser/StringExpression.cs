using Kiwi.Lexer;

namespace Kiwi.Parser
{
    internal class StringExpression : IExpressionSyntax
    {
        public Token Value { get; }

        public StringExpression(Token value)
        {
            Value = value;
        }
    }
}