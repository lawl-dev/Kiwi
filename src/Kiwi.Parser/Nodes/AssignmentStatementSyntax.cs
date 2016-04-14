using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class AssignmentStatementSyntax : IStatementSyntax
    {
        public AssignmentStatementSyntax(IExpressionSyntax member, Token @operator, IExpressionSyntax toAssign)
        {
            Member = member;
            Operator = @operator;
            ToAssign = toAssign;
        }

        public IExpressionSyntax Member { get; }
        public Token Operator { get; }
        public IExpressionSyntax ToAssign { get; }

        public SyntaxType SyntaxType => SyntaxType.AssignmentStatementSyntax;

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