using System.Collections.Generic;
using Kiwi.Parser.Nodes;
using Kiwi.Semantic.Binder.Nodes;

namespace Kiwi.Semantic.Binder
{
    internal class BoundIfElseStatement : BoundStatement
    {
        public BoundIfElseStatement(
            BoundExpression boundExpression,
            List<BoundStatement> boundStatements,
            List<BoundStatement> elseBoundStatements,
            IfElseStatementSyntax statementSyntax) : base(statementSyntax)
        {
            BoundExpression = boundExpression;
            BoundStatements = boundStatements;
            ElseBoundStatements = elseBoundStatements;
        }

        public BoundExpression BoundExpression { get; set; }
        public List<BoundStatement> BoundStatements { get; set; }
        public List<BoundStatement> ElseBoundStatements { get; set; }
    }
}