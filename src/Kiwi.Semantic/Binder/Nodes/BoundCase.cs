using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundCase : BoundNode
    {
        public BoundExpression BoundExpression { get; }
        public BoundStatement BoundStatement { get; }

        public BoundCase(BoundExpression boundExpression, BoundStatement boundStatement, CaseSyntax syntax) : base(syntax)
        {
            BoundExpression = boundExpression;
            BoundStatement = boundStatement;
        }
    }
}