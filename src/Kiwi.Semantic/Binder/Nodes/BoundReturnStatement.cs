using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundReturnStatement : BoundStatement
    {
        public BoundReturnStatement(BoundExpression expression, ReturnStatementSyntax syntax) : base(syntax)
        {
            Expression = expression;
        }

        public BoundExpression Expression { get; private set; }
    }
}