using System.Collections.Generic;
using System.Linq;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class EnumSyntax : ISyntaxBase
    {
        public EnumSyntax(Token enumName, List<EnumMemberSyntax> member)
        {
            EnumName = enumName;
            Member = member;
        }

        public Token EnumName { get; private set; }
        public List<EnumMemberSyntax> Member { get; }

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