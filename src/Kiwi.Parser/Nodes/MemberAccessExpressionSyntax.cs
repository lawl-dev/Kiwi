using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class MemberAccessExpressionSyntax : IExpressionSyntax
    {
        public Token MemberName { get; }

        public MemberAccessExpressionSyntax(Token memberName)
        {
            MemberName = memberName;
        }
    }
}