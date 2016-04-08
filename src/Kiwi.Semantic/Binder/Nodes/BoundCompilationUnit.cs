using System.Collections.Generic;
using Kiwi.Parser.Nodes;

namespace Kiwi.Semantic.Binder.Nodes
{
    public class BoundCompilationUnit : BoundNode
    {
        public BoundCompilationUnit(CompilationUnitSyntax syntax) : base(syntax)
        {
            Namespaces = new List<BoundNamespace>();
            BoundUsings = new List<BoundUsing>();
        }

        public List<BoundUsing> BoundUsings { get; set; }
        public List<BoundNamespace> Namespaces { get; set; }
    }
}