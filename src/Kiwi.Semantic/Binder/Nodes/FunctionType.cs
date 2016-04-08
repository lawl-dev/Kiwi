using System.Collections.Generic;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class FunctionType : IType
    {
        public List<IType> ParameterTypes { get; set; }
        public IType ReturnType { get; set; }

        public FunctionType(List<IType> parameterTypes, IType returnType)
        {
            ParameterTypes = parameterTypes;
            ReturnType = returnType;
        }
    }
}