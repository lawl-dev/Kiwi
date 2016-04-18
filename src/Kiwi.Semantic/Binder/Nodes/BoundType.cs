using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.CompilerGeneratedNodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public partial class BoundType : BoundNode, IBoundMember, IType
    {
        public BoundType(string name, ClassSyntax syntax) : base(syntax)
        {
            Name = name;
            Type = new TypeCompilerGeneratedType();
            FieldsInternal = new List<BoundField>();
            ConstructorsInternal = new List<BoundConstructor>();
            FunctionsInternal = new List<BoundFunction>();
        }

        public string Name { get; }

        public IReadOnlyCollection<BoundConstructor> Constructors => new ReadOnlyCollection<BoundConstructor>(ConstructorsInternal);

        
        
        public IType Type { get; internal set; }

        public IEnumerable<IField> Fields => FieldsInternal.Select(x => x);

        public IEnumerable<IFunction> Functions => FunctionsInternal.Select(x => x);
    }

    public partial class BoundType
    {
        internal List<BoundField> FieldsInternal { get; }
        internal List<BoundConstructor> ConstructorsInternal { get; set; }
        internal List<BoundFunction> FunctionsInternal { get; set; }
    }
}