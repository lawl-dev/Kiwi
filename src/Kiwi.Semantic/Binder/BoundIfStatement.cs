using System.Collections.Generic;
using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder
{
    internal class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(
            BoundExpression boundExpression,
            BoundScopeStatement boundStatements,
            IfStatementSyntax statementSyntax) : base(statementSyntax)
        {
            BoundExpression = boundExpression;
            BoundStatements = boundStatements;
        }

        public BoundExpression BoundExpression { get; set; }
        public BoundScopeStatement BoundStatements { get; set; }
    }
}