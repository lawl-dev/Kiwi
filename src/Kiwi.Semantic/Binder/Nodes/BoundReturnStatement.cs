using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    internal class BoundReturnStatement : BoundStatement
    {
        public BoundReturnStatement(BoundExpression boundExpression, ReturnStatementSyntax syntax) : base(syntax)
        {
            BoundExpression = boundExpression;
        }

        public BoundExpression BoundExpression { get; private set; }
    }
}