using System.Collections.Generic;

namespace Kiwi.Parser.Nodes
{
    public class ForStatementSyntax : IStatementSyntax
    {
        public ISyntaxBase InitStatement { get; }
        public IExpressionSyntax CondExpression { get; }
        public ISyntaxBase LoopExpression { get; }
        public List<IStatementSyntax> Statements { get; }
        public SyntaxType SyntaxType => SyntaxType.ForStatementSyntax;

        public ForStatementSyntax(IStatementSyntax initStatement, IExpressionSyntax condExpression, ISyntaxBase loopExpression, List<IStatementSyntax> statements)
        {
            InitStatement = initStatement;
            CondExpression = condExpression;
            LoopExpression = loopExpression;
            Statements = statements;
        }
        
        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }

        public TResult Accept<TResult>(ISyntaxVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}