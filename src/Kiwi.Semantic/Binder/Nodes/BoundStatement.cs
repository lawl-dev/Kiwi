using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public abstract class BoundStatement : BoundNode
    {
        protected BoundStatement(IStatementSyntax syntax) : base(syntax)
        {
        }
    }
}