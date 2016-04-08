using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundReturnStatement : BoundStatement
    {
        public BoundExpression BoundExpression { get; private set; }

        public BoundReturnStatement(BoundExpression boundExpression, ReturnStatementSyntax syntax) : base(syntax)
        {
            BoundExpression = boundExpression;
        }
    }
}