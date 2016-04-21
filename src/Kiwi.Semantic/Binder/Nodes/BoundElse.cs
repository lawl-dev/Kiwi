using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundElse : BoundNode
    {
        public BoundStatement BoundStatement { get; }

        public BoundElse(BoundStatement boundStatement, ElseSyntax @else) : base(@else)
        {
            BoundStatement = boundStatement;
        }
    }
}