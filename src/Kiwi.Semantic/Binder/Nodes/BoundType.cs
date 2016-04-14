using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.LanguageTypes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundType : BoundNode, IBoundMember, IType
    {
        public BoundType(string name, ClassSyntax syntax) : base(syntax)
        {
            Name = name;
            Type = new TypeType();
            FieldsInternal = new List<BoundField>();
            ConstructorsInternal = new List<BoundConstructor>();
            FunctionsInternal = new List<BoundFunction>();
        }

        public string Name { get; }

        public IReadOnlyCollection<BoundConstructor> Constructors
            => new ReadOnlyCollection<BoundConstructor>(ConstructorsInternal);

        internal List<BoundField> FieldsInternal { get; }
        internal List<BoundConstructor> ConstructorsInternal { get; set; }
        internal List<BoundFunction> FunctionsInternal { get; set; }

        public IType Type { get; internal set; }

        public IReadOnlyCollection<IField> Fields
            => new ReadOnlyCollection<IField>(FieldsInternal.Cast<IField>().ToList());

        public IReadOnlyCollection<IFunction> Functions
            => new ReadOnlyCollection<IFunction>(FunctionsInternal.Cast<IFunction>().ToList());
    }
}