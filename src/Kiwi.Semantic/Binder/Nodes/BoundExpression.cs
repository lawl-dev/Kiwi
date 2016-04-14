using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public abstract class BoundExpression : BoundNode
    {
        protected BoundExpression(ISyntaxBase syntax, IType type) : base(syntax)
        {
            Type = type;
        }

        public IType Type { get; private set; }
    }
}