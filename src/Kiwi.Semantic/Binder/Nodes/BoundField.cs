using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundField : BoundNode, IBoundMember, IField
    {
        public BoundField(string name, FieldSyntax syntax) : base(syntax)
        {
            Name = name;
        }

        public BoundExpression Initializer { get; internal set; }

        public IType Type { get; set; }
        public string Name { get; }
        public VariableQualifier Qualifier { get; internal set; }
    }
}