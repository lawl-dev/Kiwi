using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.CompilerGeneratedNodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundTypeExpression : BoundExpression
    {
        public BoundTypeExpression(IType referencedType, IdentifierExpressionSyntax syntax) : base(syntax, new TypeCompilerGeneratedType())
        {
            ReferencedType = referencedType;
        }

        public IType ReferencedType { get; }
    }
}