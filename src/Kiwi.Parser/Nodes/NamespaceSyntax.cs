using System.Collections.Generic;
using System.Linq;
using Kiwi.Lexer;

namespace Kiwi.Parser.Nodes
{
    public class NamespaceSyntax : ISyntaxBase
    {
        public Token NamespaceName { get; private set; }
        public List<ISyntaxBase> Member { get; }

        public List<ClassSyntax> ClassMember => Member.OfType<ClassSyntax>().ToList();

        public NamespaceSyntax(Token namespaceName, List<ISyntaxBase> member)
        {
            NamespaceName = namespaceName;
            Member = member;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}