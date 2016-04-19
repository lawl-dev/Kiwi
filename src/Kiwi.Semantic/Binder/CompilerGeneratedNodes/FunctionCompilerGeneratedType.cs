using System;
using System.Collections.Generic;
using System.Linq;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder.CompilerGeneratedNodes
{
    public class FunctionCompilerGeneratedType : CompilerGeneratedTypeBase
    {
        public FunctionCompilerGeneratedType(List<IType> parameterTypes, IType returnType)
        {
            ParameterTypes = parameterTypes;
            ReturnType = returnType;
        }

        public List<IType> ParameterTypes { get; set; }
        public IType ReturnType { get; set; }

        public override IEnumerable<IField> Fields => Enumerable.Empty<IField>();
        public override IEnumerable<IFunction> Functions => Enumerable.Empty<IFunction>();
        public override IEnumerable<IConstructor> Constructors => Enumerable.Empty<IConstructor>();
    }
}