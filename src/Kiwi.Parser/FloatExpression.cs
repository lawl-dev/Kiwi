using Kiwi.Lexer;

namespace Kiwi.Parser
{
    internal class FloatExpression : IExpressionSyntax
    {
        public Token Value { get; }

        public FloatExpression(Token value)
        {
            Value = value;
        }
    }
}