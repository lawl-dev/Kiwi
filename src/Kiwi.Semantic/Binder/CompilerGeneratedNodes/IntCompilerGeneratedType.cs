using System.Collections.Generic;
using System.Linq;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder.CompilerGeneratedNodes
{
    public class IntCompilerGeneratedType : IType
    {
        public IEnumerable<IField> Fields => CreateFields();

        private static IEnumerable<IField> CreateFields()
        {
            yield return new CompilerGeneratedField("Max", VariableQualifier.Const, new IntCompilerGeneratedType());
        }

        public IEnumerable<IFunction> Functions => Enumerable.Empty<IFunction>();
    }
}