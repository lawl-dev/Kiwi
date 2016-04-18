using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundMemberAccessExpression : BoundExpression
    {
        public string Name { get; }
        public IBoundMember Member { get; }

        public BoundMemberAccessExpression(string name, IBoundMember member, MemberAccessExpressionSyntax expressionSyntax) : base(expressionSyntax, member.Type)
        {
            Name = name;
            Member = member;
        }
    }
}