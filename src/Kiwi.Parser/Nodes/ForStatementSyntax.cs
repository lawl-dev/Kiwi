using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ForStatementSyntax : IStatementSyntax
    {
        public ISyntaxBase InitExpression { get; }
        public IExpressionSyntax CondExpression { get; }
        public ISyntaxBase LoopExpression { get; }
        public List<IStatementSyntax> Statements { get; }

        public ForStatementSyntax(ISyntaxBase initExpression, IExpressionSyntax condExpression, ISyntaxBase loopExpression, List<IStatementSyntax> statements)
        {
            InitExpression = initExpression;
            CondExpression = condExpression;
            LoopExpression = loopExpression;
            Statements = statements;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}