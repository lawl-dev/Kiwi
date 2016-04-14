using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class MemberExpressionSyntax : IExpressionSyntax
    {
        public MemberExpressionSyntax(Token memberName)
        {
            MemberName = memberName;
        }

        public Token MemberName { get; }
        public SyntaxType SyntaxType => SyntaxType.MemberExpressionSyntax;

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