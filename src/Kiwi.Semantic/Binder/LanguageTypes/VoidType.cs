using System.Collections.Generic;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder.LanguageTypes
{
    internal class VoidType : IType
    {
        public IReadOnlyCollection<IField> Fields { get; }
        public IReadOnlyCollection<IFunction> Functions { get; }
    }
}