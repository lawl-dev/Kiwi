using Kiwi.Lexer;

namespace Kiwi.Parser
{
    public class BinaryExpressionSyntax : IExpressionSyntax
    {
        public IExpressionSyntax LeftExpression { get; }
        public IExpressionSyntax RightExpression { get; }
        public Token Operator { get; }

        public BinaryExpressionSyntax(IExpressionSyntax leftExpression, IExpressionSyntax rightExpression, Token @operator)
        {
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
            Operator = @operator;
        }
    }
}