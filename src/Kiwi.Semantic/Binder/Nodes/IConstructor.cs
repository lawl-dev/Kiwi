using System.Collections.Generic;

namespace Kiwi.Semantic.Binder.Nodes
{
    public interface IConstructor
    {
        IEnumerable<IParameter> Parameter { get; }
    }
}