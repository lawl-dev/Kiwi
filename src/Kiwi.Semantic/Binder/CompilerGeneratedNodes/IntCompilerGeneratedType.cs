using System.Collections.Generic;
using System.Linq;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder.CompilerGeneratedNodes
{
    public class IntCompilerGeneratedType : CompilerGeneratedTypeBase
    {
        private static IEnumerable<IField> CreateFields()
        {
            yield return new CompilerGeneratedField("Max", VariableQualifier.Const, new IntCompilerGeneratedType());
        }
        public override IEnumerable<IField> Fields => CreateFields();
        public override IEnumerable<IFunction> Functions => Enumerable.Empty<IFunction>();
        public override IEnumerable<IConstructor> Constructors => Enumerable.Empty<IConstructor>();
    }

    public abstract class CompilerGeneratedTypeBase : IType
    {
        public abstract IEnumerable<IField> Fields { get; }
        public abstract IEnumerable<IFunction> Functions { get; }
        public abstract IEnumerable<IConstructor> Constructors { get; }
    }
}