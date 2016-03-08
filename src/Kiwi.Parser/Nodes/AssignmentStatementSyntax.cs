using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class AssignmentStatementSyntax : IStatementSyntax
    {
        public IExpressionSyntax Member { get; }
        public Token Operator { get; }
        public IExpressionSyntax ToAssign { get; }

        public AssignmentStatementSyntax(IExpressionSyntax member, Token @operator, IExpressionSyntax toAssign)
        {
            Member = member;
            Operator = @operator;
            ToAssign = toAssign;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}