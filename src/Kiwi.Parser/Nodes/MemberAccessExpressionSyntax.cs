using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class MemberAccessExpressionSyntax : IExpressionSyntax
    {
        public IExpressionSyntax Owner { get; }
        public Token MemberName { get; }

        public MemberAccessExpressionSyntax(IExpressionSyntax owner, Token memberName)
        {
            Owner = owner;
            MemberName = memberName;
        }
    }
}