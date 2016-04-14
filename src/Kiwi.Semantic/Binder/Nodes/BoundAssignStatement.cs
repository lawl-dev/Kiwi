using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundAssignStatement : BoundStatement
    {
        public BoundAssignStatement(
            BoundExpression memberExpression,
            BoundExpression toAssignExpression,
            AssignmentOperators assignOperator,
            AssignmentStatementSyntax statementSyntax) : base(statementSyntax)
        {
            MemberExpression = memberExpression;
            ToAssignExpression = toAssignExpression;
            AssignOperator = assignOperator;
        }

        public BoundExpression MemberExpression { get; set; }
        public BoundExpression ToAssignExpression { get; set; }
        public AssignmentOperators AssignOperator { get; set; }
    }
}