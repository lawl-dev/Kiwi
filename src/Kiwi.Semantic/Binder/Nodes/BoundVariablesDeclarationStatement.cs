using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundVariablesDeclarationStatement : BoundStatement
    {
        public BoundVariablesDeclarationStatement(
            List<BoundVariableDeclarationStatement> boundVariableDeclarationStatements,
            VariablesDeclarationStatementSyntax statementSyntax) : base(statementSyntax)
        {
            BoundVariableDeclarationStatements = boundVariableDeclarationStatements;
        }

        public List<BoundVariableDeclarationStatement> BoundVariableDeclarationStatements { get; }
    }
}