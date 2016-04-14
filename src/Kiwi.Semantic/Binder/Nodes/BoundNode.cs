using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public abstract class BoundNode
    {
        protected BoundNode(ISyntaxBase syntax)
        {
            Syntax = syntax;
        }

        public ISyntaxBase Syntax { get; }
    }
}