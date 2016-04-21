using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
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

        public BoundExpression BoundExpression { get; }
        public BoundScopeStatement BoundStatements { get; }
        public BoundScopeStatement ElseBoundStatements { get; }
    }
}