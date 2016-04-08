using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundMemberExpression : BoundExpression
    {
        public Token MemberName { get; private set; }
        public IBoundMember BoundMember { get; private set; }

        public BoundMemberExpression(Token memberName, IBoundMember boundMember, MemberExpressionSyntax expressionSyntax) : base(expressionSyntax, boundMember.Type)
        {
            MemberName = memberName;
            BoundMember = boundMember;
        }
    }
}