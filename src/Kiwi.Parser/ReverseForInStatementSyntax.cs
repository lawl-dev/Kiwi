using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    public class ReverseForInStatementSyntax : ForInStatementSyntax
    {
        public ReverseForInStatementSyntax(IExpressionSyntax itemExpression, bool declareItemInnerScope, IExpressionSyntax collectionExpression, List<ISyntaxBase> body) : base(itemExpression, declareItemInnerScope, collectionExpression, body)
        {
        }
    }
}