using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ReverseForInStatementSyntax : ForInStatementSyntax
    {
        public ReverseForInStatementSyntax(IExpressionSyntax itemExpression, bool declareItemInnerScope, IExpressionSyntax collectionExpression, List<IStatementSyntax> statements) : base(itemExpression, declareItemInnerScope, collectionExpression, statements)
        {
        }
    }
}