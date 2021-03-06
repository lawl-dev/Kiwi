using System.Collections.Generic;

namespace Kiwi.Semantic.Binder.Nodes
{
    public interface IFunction : IBoundMember
    {
        string Name { get; }
        IEnumerable<IParameter> Parameter { get; }
        IType ReturnType { get; }
    }
}