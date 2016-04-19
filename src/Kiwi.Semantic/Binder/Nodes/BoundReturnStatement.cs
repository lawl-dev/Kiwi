using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundReturnStatement : BoundStatement
    {
        public BoundReturnStatement(BoundExpression boundExpression, ReturnStatementSyntax syntax) : base(syntax)
        {
            BoundExpression = boundExpression;
        }

        public BoundExpression BoundExpression { get; private set; }
    }
}