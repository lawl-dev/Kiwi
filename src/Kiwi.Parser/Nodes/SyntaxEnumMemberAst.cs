using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    internal class SyntaxEnumMemberAst : SyntaxAstBase
    {
        private readonly SyntaxAstBase _initializer;
        public Token MemberName { get; set; }

        public SyntaxEnumMemberAst(Token memberName, SyntaxAstBase initializer)
        {
            _initializer = initializer;
            MemberName = memberName;
        }
    }
}