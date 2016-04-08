using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundType : BoundNode, IBoundMember, IType
    {
        public Token Name { get; }
        public ReadOnlyCollection<BoundField> Fields => new ReadOnlyCollection<BoundField>(FieldsInternal); 
        public ReadOnlyCollection<BoundConstructor> Constructors => new ReadOnlyCollection<BoundConstructor>(ConstructorsInternal); 
        public ReadOnlyCollection<BoundFunction> Functions => new ReadOnlyCollection<BoundFunction>(FunctionsInternal); 

        internal List<BoundField> FieldsInternal { get; }
        internal List<BoundConstructor> ConstructorsInternal { get; set; }
        internal List<BoundFunction> FunctionsInternal { get; set; }

        public BoundType(Token name, ClassSyntax syntax) : base(syntax)
        {
            Name = name;
            Type = new StandardType(StandardTypes.Type);
            FieldsInternal = new List<BoundField>();
            ConstructorsInternal = new List<BoundConstructor>();
            FunctionsInternal = new List<BoundFunction>();
        }

        public IType Type { get; set; }
    }
}