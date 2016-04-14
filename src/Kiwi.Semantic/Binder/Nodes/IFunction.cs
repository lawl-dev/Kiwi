using System.Collections.Generic;

namespace Kiwi.Semantic.Binder.Nodes
{
    public interface IFunction
    {
        string Name { get; }
        IEnumerable<IParameter> Parameter { get; set; }
        IType ReturnType { get; set; }
    }
}