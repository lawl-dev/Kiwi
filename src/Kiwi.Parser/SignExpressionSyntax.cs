using Kiwi.Lexer;

namespace Kiwi.Parser
{
    public class SignExpressionSyntax : IExpressionSyntax
    {
        public Token Operator { get; }
        public IExpressionSyntax Expression { get; }

        public SignExpressionSyntax(Token @operator, IExpressionSyntax expression)
        {
            Operator = @operator;
            Expression = expression;
        }
    }
}