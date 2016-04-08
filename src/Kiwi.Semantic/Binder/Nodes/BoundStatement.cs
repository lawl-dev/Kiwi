using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public abstract class BoundStatement : BoundNode
    {
        protected BoundStatement(ISyntaxBase syntax) : base(syntax)
        {
        }
    }
}