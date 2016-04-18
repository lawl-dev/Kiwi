using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder.CompilerGeneratedNodes
{
    internal class ArrayCompilerGeneratedType : IType
    {
        public ArrayCompilerGeneratedType(IType type, int dimension)
        {
            Type = type;
            Dimension = dimension;
        }

        public IType Type { get; }
        public int Dimension { get; }

        public IEnumerable<IField> Fields => Enumerable.Empty<IField>();

        public IEnumerable<IFunction> Functions => CreateFunctions();
        public IEnumerable<IConstructor> Constructors => Enumerable.Empty<IConstructor>();

        private static IEnumerable<CompilerGeneratedFunction> CreateFunctions()
        {
            yield return CreateLengthFunction();
        }

        private static CompilerGeneratedFunction CreateLengthFunction()
        {
            var parameters = new List<SpecialParameter> { new SpecialParameter("dimension", new IntCompilerGeneratedType()) };
            return new CompilerGeneratedFunction("Length", parameters, new IntCompilerGeneratedType());
        }
    }
}