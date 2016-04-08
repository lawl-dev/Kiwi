using Kiwi.Lexer;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundVariableDeclarationStatement : BoundStatement, IBoundMember
    {
        public Token Identifier { get; private set; }
        public VariableQualifier Qualifier { get; set; }
        public BoundExpression BoundExpression { get; set; }

        public BoundVariableDeclarationStatement(Token identifier, VariableQualifier qualifier, BoundExpression boundExpression, VariableDeclarationStatementSyntax syntax) : base(syntax)
        {
            Identifier = identifier;
            Qualifier = qualifier;
            Type = boundExpression.Type;
            BoundExpression = boundExpression;
        }

        public IType Type { get; set; }
    }
}