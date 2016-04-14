using System.Collections.Generic;
using System.Collections.ObjectModel;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder.LanguageTypes
{
    internal class ArrayType : IType
    {
        public ArrayType(IType type, int dimension)
        {
            Type = type;
            Dimension = dimension;
        }

        public IType Type { get; set; }
        public int Dimension { get; set; }

        public IReadOnlyCollection<IField> Fields { get; }

        public IReadOnlyCollection<IFunction> Functions => CreateFunctions();

        private static ReadOnlyCollection<SpecialFunction> CreateFunctions()
        {
            var functions = new List<SpecialFunction>();
            functions.Add(CreateLengthFunction());
            return functions.AsReadOnly();
        }

        private static SpecialFunction CreateLengthFunction()
        {
            var parameters = new List<SpecialParameter>();
            parameters.Add(new SpecialParameter("dimension", new IntSpecialType()));
            return new SpecialFunction("Length", parameters, new IntSpecialType());
        }
    }
}