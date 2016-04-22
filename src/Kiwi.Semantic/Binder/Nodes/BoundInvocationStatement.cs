using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundInvocationStatement : BoundStatement
    {
        public BoundInvocationExpression InvocationExpression { get; }

        public BoundInvocationStatement(BoundInvocationExpression invocationExpression, InvocationStatementSyntax syntax) : base(syntax)
        {
            InvocationExpression = invocationExpression;
        }
    }
}