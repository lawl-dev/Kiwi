using System.Collections.Generic;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder.LanguageTypes
{
    public class TypeType : IType
    {
        public IReadOnlyCollection<IField> Fields { get; }
        public IReadOnlyCollection<IFunction> Functions { get; }
    }
}