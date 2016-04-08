using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public abstract class BoundNode
    {
        public ISyntaxBase Syntax { get; }

        protected BoundNode(ISyntaxBase syntax)
        {
            Syntax = syntax;
        }
    }
}