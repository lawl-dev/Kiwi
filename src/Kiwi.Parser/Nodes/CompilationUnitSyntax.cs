using System.Collections.Generic;
using System.Linq;

namespace Kiwi.Parser.Nodes
{
    public class CompilationUnitSyntax : ISyntaxBase
    {
        public List<UsingSyntax> Usings { get; set; }
        public List<NamespaceSyntax> Namespaces { get; set; }

        public CompilationUnitSyntax(List<UsingSyntax> usings, List<NamespaceSyntax> namespaces)
        {
            Usings = usings;
            Namespaces = namespaces;
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