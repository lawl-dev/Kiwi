using System;
using System.Collections.Generic;
using System.Linq;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder.CompilerGeneratedNodes
{
    internal class FunctionCompilerGeneratedType : IType
    {
        public FunctionCompilerGeneratedType(List<IType> parameterTypes, IType returnType)
        {
            ParameterTypes = parameterTypes;
            ReturnType = returnType;
        }

        public List<IType> ParameterTypes { get; set; }
        public IType ReturnType { get; set; }

        public IEnumerable<IField> Fields => Enumerable.Empty<IField>();
        public IEnumerable<IFunction> Functions => Enumerable.Empty<IFunction>();
    }
}