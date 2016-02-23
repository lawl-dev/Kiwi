using Kiwi.Lexer;

namespace Kiwi.Parser
{
    public class IntExpressionSyntax : IExpressionSyntax
    {
        public Token Value { get; }

        public IntExpressionSyntax(Token value)
        {
            Value = value;
        }
    }
}