using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundField : BoundNode, IBoundMember
    {
        public Token Name { get; private set; }
        public VariableQualifier Qualifier { get; internal set; }
        public BoundExpression Initializer { get; internal set; }

        public BoundField(Token name, FieldSyntax syntax) : base(syntax)
        {
            Name = name;
        }

        public IType Type { get; set; }
    }
}