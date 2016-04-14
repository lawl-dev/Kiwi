using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundMemberExpression : BoundExpression
    {
        public BoundMemberExpression(
            string memberName,
            IBoundMember boundMember,
            MemberExpressionSyntax expressionSyntax) : base(expressionSyntax, boundMember.Type)
        {
            MemberName = memberName;
            BoundMember = boundMember;
        }

        public string MemberName { get; private set; }
        public IBoundMember BoundMember { get; private set; }
    }
}