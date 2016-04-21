using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
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

        public BoundExpression BoundExpression { get; }
        public BoundScopeStatement BoundStatements { get; }
    }
}