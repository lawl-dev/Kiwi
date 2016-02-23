using Kiwi.Lexer;

namespace Kiwi.Parser
{
    internal class MemberAccessExpression : IExpressionSyntax
    {
        public Token MemberName { get; }

        public MemberAccessExpression(Token memberName)
        {
            MemberName = memberName;
        }
    }
}