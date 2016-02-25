using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    public class RangeExpressionSyntax : IExpressionSyntax
    {
        public IntExpressionSyntax FromIntExpression { get; }
        public Token ToIntExpression { get; }

        public RangeExpressionSyntax(IntExpressionSyntax fromIntExpression, Token toIntExpression)
        {
            FromIntExpression = fromIntExpression;
            ToIntExpression = toIntExpression;
        }
    }
}