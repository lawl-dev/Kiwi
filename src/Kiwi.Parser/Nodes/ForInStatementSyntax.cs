using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ForInStatementSyntax : IStatementSyntax
    {
        public IExpressionSyntax ItemExpression { get; }
        public bool DeclareItemInnerScope { get; }
        public IExpressionSyntax CollectionExpression { get; }
        public List<IStatementSyntax> Statements { get; }

        public ForInStatementSyntax(IExpressionSyntax itemExpression, bool declareItemInnerScope, IExpressionSyntax collectionExpression, List<IStatementSyntax> statements)
        {
            ItemExpression = itemExpression;
            DeclareItemInnerScope = declareItemInnerScope;
            CollectionExpression = collectionExpression;
            Statements = statements;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}