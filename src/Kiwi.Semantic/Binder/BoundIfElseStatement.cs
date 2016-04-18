using System.Collections.Generic;
using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder
{
    internal class BoundIfElseStatement : BoundStatement
    {
        public BoundIfElseStatement(
            BoundExpression boundExpression,
            BoundScopeStatement boundStatements,
            BoundScopeStatement elseBoundStatements,
            IfElseStatementSyntax statementSyntax) : base(statementSyntax)
        {
            BoundExpression = boundExpression;
            BoundStatements = boundStatements;
            ElseBoundStatements = elseBoundStatements;
        }

        public BoundExpression BoundExpression { get; set; }
        public BoundScopeStatement BoundStatements { get; set; }
        public BoundScopeStatement ElseBoundStatements { get; set; }
    }
}