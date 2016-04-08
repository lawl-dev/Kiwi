using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public interface IBoundMember
    {
        IType Type { get; set; }
    }
}