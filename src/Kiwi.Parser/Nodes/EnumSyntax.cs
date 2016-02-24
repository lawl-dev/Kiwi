using System.Collections.Generic;
using System.Linq;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    internal class EnumSyntax : ISyntaxBase
    {
        public Token EnumName { get; private set; }
        public List<ISyntaxBase> Member { get; }
        public List<EnumMemberSyntax> EnumMember => Member.OfType<EnumMemberSyntax>().ToList(); 

        public EnumSyntax(Token enumName, List<ISyntaxBase> member)
        {
            EnumName = enumName;
            Member = member;
        }
    }
}