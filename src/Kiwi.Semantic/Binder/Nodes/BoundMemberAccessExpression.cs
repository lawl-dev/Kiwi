using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundMemberAccessExpression : BoundExpression
    {
        public BoundMemberAccessExpression(ISyntaxBase syntax, BoundType type) : base(syntax, type)
        {
        }
    }
}