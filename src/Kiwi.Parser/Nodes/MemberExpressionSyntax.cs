using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class MemberExpressionSyntax : IExpressionSyntax
    {
        public Token MemberName { get; }

        public MemberExpressionSyntax(Token memberName)
        {
            MemberName = memberName;
        }
    }
}