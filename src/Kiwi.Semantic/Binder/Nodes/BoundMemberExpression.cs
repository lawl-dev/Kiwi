using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundMemberExpression : BoundExpression
    {
        public BoundMemberExpression(
            string name,
            IBoundMember boundMember,
            MemberExpressionSyntax expressionSyntax) : base(expressionSyntax, boundMember.Type)
        {
            Name = name;
            BoundMember = boundMember;
        }

        public string Name { get; }
        public IBoundMember BoundMember { get; }
    }
}