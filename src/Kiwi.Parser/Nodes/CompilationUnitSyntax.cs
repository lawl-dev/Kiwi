using System.Collections.Generic;
using System.Linq;

namespace Kiwi.Parser.Nodes
{
    public class CompilationUnitSyntax : ISyntaxBase
    {
        public List<ISyntaxBase> MemberSyntax { get; }
        public List<UsingSyntax> UsingMember => MemberSyntax.OfType<UsingSyntax>().ToList(); 
        public List<NamespaceSyntax> NamespaceMember => MemberSyntax.OfType<NamespaceSyntax>().ToList(); 

        public CompilationUnitSyntax(List<ISyntaxBase> memberSyntax)
        {
            MemberSyntax = memberSyntax;
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}