using System.Collections.Generic;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder.LanguageTypes
{
    internal class FunctionType : IType
    {
        public FunctionType(List<IType> parameterTypes, IType returnType)
        {
            ParameterTypes = parameterTypes;
            ReturnType = returnType;
        }

        public List<IType> ParameterTypes { get; set; }
        public IType ReturnType { get; set; }

        public IReadOnlyCollection<IField> Fields { get; }
        public IReadOnlyCollection<IFunction> Functions { get; }
    }
}