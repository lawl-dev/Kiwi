using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Parser
{
    public class ForInStatementSyntax : IStatetementSyntax
    {
        public IExpressionSyntax ItemExpression { get; }
        public bool DeclareItemInnerScope { get; }
        public IExpressionSyntax CollectionExpression { get; }
        public List<ISyntaxBase> Body { get; }

        public ForInStatementSyntax(IExpressionSyntax itemExpression, bool declareItemInnerScope, IExpressionSyntax collectionExpression, List<ISyntaxBase> body)
        {
            ItemExpression = itemExpression;
            DeclareItemInnerScope = declareItemInnerScope;
            CollectionExpression = collectionExpression;
            Body = body;
        }
    }
}