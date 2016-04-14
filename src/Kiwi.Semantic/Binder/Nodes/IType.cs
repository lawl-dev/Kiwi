using System.Collections.Generic;

namespace Kiwi.Semantic.Binder.Nodes
{
    public interface IType
    {
        IReadOnlyCollection<IField> Fields { get; }
        IReadOnlyCollection<IFunction> Functions { get; }
    }
}