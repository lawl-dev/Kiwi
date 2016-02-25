using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class FloatExpressionSyntax : IExpressionSyntax, IConstExpression
    {
        public Token Value { get; }

        public FloatExpressionSyntax(Token value)
        {
            Value = value;
        }
    }
}