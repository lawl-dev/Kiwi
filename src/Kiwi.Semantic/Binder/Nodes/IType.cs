using System.Collections.Generic;

namespace Kiwi.Semantic.Binder.Nodes
{
    public interface IType
    {
        IEnumerable<IField> Fields { get; }
        IEnumerable<IFunction> Functions { get; }
    }
}