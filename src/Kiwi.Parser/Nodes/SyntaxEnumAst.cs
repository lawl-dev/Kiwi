using System.Collections.Generic;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    internal class SyntaxEnumAst : SyntaxAstBase
    {
        public Token EnumName { get; set; }
        public List<SyntaxAstBase> MemberSyntax { get; set; }

        public SyntaxEnumAst(Token enumName, List<SyntaxAstBase> memberSyntax)
        {
            EnumName = enumName;
            MemberSyntax = memberSyntax;
        }
    }
}