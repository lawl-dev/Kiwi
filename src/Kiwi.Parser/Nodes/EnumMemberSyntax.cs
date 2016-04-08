using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class EnumMemberSyntax : ISyntaxBase
    {
        public Token MemberName { get; private set; }
        public IExpressionSyntax Initializer { get; private set; }
        public SyntaxType SyntaxType => SyntaxType.EnumMemberSyntax;

        public EnumMemberSyntax(Token memberName, IExpressionSyntax initializer)
        {
            MemberName = memberName;
            Initializer = initializer;
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