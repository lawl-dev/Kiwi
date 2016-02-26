using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
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