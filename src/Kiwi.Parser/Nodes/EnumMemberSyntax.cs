using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    internal class EnumMemberSyntax : ISyntaxBase
    {
        public Token MemberName { get; private set; }
        public IExpressionSyntax Initializer { get; private set; }

        public EnumMemberSyntax(Token memberName, IExpressionSyntax initializer)
        {
            MemberName = memberName;
            Initializer = initializer;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}