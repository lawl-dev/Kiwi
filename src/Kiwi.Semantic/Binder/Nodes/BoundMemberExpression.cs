using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundMemberExpression : BoundExpression
    {
        public BoundMemberExpression(
            string name,
            IBoundMember boundMember,
            IdentifierExpressionSyntax orTypeExpressionSyntax) : base(orTypeExpressionSyntax, boundMember.Type)
        {
            Name = name;
            BoundMember = boundMember;
        }

        public string Name { get; }
        public IBoundMember BoundMember { get; }
    }
}