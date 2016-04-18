using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundVariableDeclarationStatement : BoundStatement, IBoundMember
    {
        public BoundVariableDeclarationStatement(
            string identifier,
            VariableQualifier qualifier,
            BoundExpression boundExpression,
            VariableDeclarationStatementSyntax syntax) : base(syntax)
        {
            Identifier = identifier;
            Qualifier = qualifier;
            Type = boundExpression.Type;
            BoundExpression = boundExpression;
        }

        public string Identifier { get; }
        public VariableQualifier Qualifier { get; }
        public BoundExpression BoundExpression { get; }
        public IType Type { get; }
    }
}