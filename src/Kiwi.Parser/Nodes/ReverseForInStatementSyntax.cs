using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ReverseForInStatementSyntax : ForInStatementSyntax
    {
        public ReverseForInStatementSyntax(IExpressionSyntax itemExpression, bool declareItemInnerScope, IExpressionSyntax collectionExpression, List<ISyntaxBase> body) : base(itemExpression, declareItemInnerScope, collectionExpression, body)
        {
        }
    }
}