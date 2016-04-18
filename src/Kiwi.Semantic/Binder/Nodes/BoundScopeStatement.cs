using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundScopeStatement:BoundStatement
    {
        public List<BoundStatement> Statements { get; }

        public BoundScopeStatement(List<BoundStatement> statements, ScopeStatementSyntax syntax) : base(syntax)
        {
            Statements = statements;
        }
    }
}